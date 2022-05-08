using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public static string[] BLOCKING_LAYER_NAMES = new string[] { "AI", "Actor", "Blocking" };

    public GameObject PlayerObject;
    public GameObject BulletPrefab;
    public GameObject GunPrefab;
    //public GameObject UI_INV_GO;
    public Vector3 mouseDir;
    public float lookAngle;

    public SpriteManager spriteManager;
    
    private BoxCollider2D boxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;

    private Damageable damageable;

    public Camera cam;

    public bool IsInventoryInit = false;
    public Inventory inventory;
    [SerializeField] public UIInventory uiInventory;
    [SerializeField] public UIXpBar UiXpBar;
    public bool isGUIOpened = false;

    // Networked Inventory
    public NetworkVariable<ItemStructNetcode> NetHelmet = new NetworkVariable<ItemStructNetcode>();
    public NetworkVariable<ItemStructNetcode> NetArmor = new NetworkVariable<ItemStructNetcode>();
    public NetworkVariable<ItemStructNetcode> NetWeapon = new NetworkVariable<ItemStructNetcode>();

    // STATS FROM INVENTORY.GetWeaponStats()
    public bool canFire = true;
    /*
    public float fireRate = 0.1f;
    public int bulletDmg = 1;
    public float bulletVelocity = 3f;
    public float bulletRange = 3f;
    */

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetHelmet.OnValueChanged += OnNetHelmetChanged;
        NetArmor.OnValueChanged += OnNetArmorChanged;
        NetWeapon.OnValueChanged += OnNetWeaponChanged;
        if (!IsInventoryInit)
        {
            inventory = new Inventory(UseItem);
            IsInventoryInit = true;
        }
        inventory.ClothesChanged += OnClothesChanged;
        if(IsHost || IsServer)
        {
            OnClothesChanged(inventory, EventArgs.Empty);
        }

        // TODO rajz, feliratkozni event változásra
    }

    private void OnNetWeaponChanged(ItemStructNetcode previousValue, ItemStructNetcode newValue)
    {
        spriteManager.SetWeapon(NetWeapon.Value);
    }

    private void OnNetArmorChanged(ItemStructNetcode previousValue, ItemStructNetcode newValue)
    {
        spriteManager.SetArmor(NetArmor.Value);
    }

    private void OnNetHelmetChanged(ItemStructNetcode previousValue, ItemStructNetcode newValue)
    {
        spriteManager.SetHelmet(NetHelmet.Value);
    }

    private void OnClothesChanged(object sender, EventArgs e)
    {
        if (IsLocalPlayer)
        {
            Item hItem = inventory.getHelmet();
            ItemStructNetcode helm;
            if (hItem == null)
            {
                helm = new ItemStructNetcode();
                helm.SetEmpty();
            } else
            {
                helm = new ItemStructNetcode(hItem);
            }

            Item aItem = inventory.getArmor();
            ItemStructNetcode arm;
            if (aItem == null)
            {
                arm = new ItemStructNetcode();
                arm.SetEmpty();
            }
            else
            {
                arm = new ItemStructNetcode(aItem);
            }

            Item wItem = inventory.getWeapon();
            ItemStructNetcode weapon;
            if (wItem == null)
            {
                weapon = new ItemStructNetcode();
                weapon.SetEmpty();
            }
            else
            {
                weapon = new ItemStructNetcode(wItem);
            }
            ChangePlayerClothesServerRpc(helm, arm, weapon);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void ChangePlayerClothesServerRpc(ItemStructNetcode helm, ItemStructNetcode arm, ItemStructNetcode weapon)
    {
        NetHelmet.Value = helm;
        NetArmor.Value = arm;
        NetWeapon.Value = weapon;
    }

    void Start()
    {
        PlayerObject = gameObject;
        gameObject.tag = "Player";
        boxCollider = PlayerObject.GetComponent<BoxCollider2D>();
        cam = GetComponentInChildren<Camera>();
        if (!IsLocalPlayer)
        {
            GetComponentInChildren<Camera>().enabled = false;
        }
        if (!IsInventoryInit)
        {
            inventory = new Inventory(UseItem);
            IsInventoryInit = true;
        }
        uiInventory.SetInventory(inventory);
        uiInventory.gameObject.SetActive(false);
        uiInventory.SetPlayer(this);
        UiXpBar.setXpBar(inventory);

        damageable = GetComponent<Damageable>();

        Vector3 spawnpos = GameManager.Instance.SpawnInfo.SpawnLocation;
        Vector3 ppos = transform.position;
        transform.Translate(spawnpos - ppos);
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            if (!isGUIOpened)
            {
            CheckInventory();
            CheckMovement();
            CheckFire();
            GetMouseInput();
            updateAngle();

            } else
            {
                CheckInventory();
                CheckMovement();
            }
        }
    }
    private void CheckInventory()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (!uiInventory.gameObject.activeSelf)
            {
                uiInventory.gameObject.SetActive(true);
                isGUIOpened = true;
            }
        } else
        {
            if (uiInventory.gameObject.activeSelf)
            {
                uiInventory.gameObject.SetActive(false);
                isGUIOpened = false;
            }
        }
    }
    private void GetMouseInput()
    {
        mouseDir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }
    private void CheckFire()
    {
        if (Input.GetButton("Fire1"))
        {
            OnFired();
        }
    }
    private void CheckMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (x != 0f || y != 0f)
        {
            float ms = inventory.MovementSpeed;
            if (x !=0f && y == 0f)
            {
                OnMove(x*ms , 0f);
            } else if(x == 0f && y != 0f)
            {
                OnMove(0f , y*ms);
            } else
            {
                OnMove(x*ms , y*ms);
            }
        }
    }
    private void updateAngle()
    {
        Vector2 _dir = new Vector2(mouseDir.x, mouseDir.y).normalized;
        lookAngle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
    }
    internal void UpdateDirection(Vector3 dir)
    {
        mouseDir = dir;
    }
    // Call server rpc. Instanciate (Spawn) bullet with Netw. Transform Obj..  Check collision on server only.
    internal void OnFired()
    {


        if (canFire)
        {
            ItemWeaponStats iws = inventory.GetWeaponStats();
            if (iws == null) return;
            ShootServerRpc(transform.position, lookAngle, NetworkObject.NetworkObjectId, iws.WeaponDamage, iws.BulletVelocity, iws.BulletRange);
            canFire = false;
            Invoke(nameof(AllowFire), iws.TimeBetweenAttacks);
        }
    }
    [ServerRpc]
    void ShootServerRpc(Vector3 v, float lookAngle, ulong id, int wd, float bv, float br)
    {
        SetupBulletInst(v, lookAngle, id, wd, bv, br);
        /*
        var bull = SetupBulletInst(v, lookAngle);
        
        if (bull.TryGetComponent<NetworkObject>(out NetworkObject no))
        {
            no.Spawn();
        }
        */
        ShootClientRpc(v, lookAngle, id, wd, bv, br);

    }
    [ClientRpc]
    void ShootClientRpc(Vector3 v, float lookAngle, ulong id, int wd, float bv, float br)
    {
        if (IsHost) return;
        SetupBulletInst(v, lookAngle, id, wd, bv, br);
        
    }
    GameObject SetupBulletInst(Vector3 v, float lookAngle, ulong id, int wd, float bv, float br)
    {
        GameObject go = Instantiate(BulletPrefab, gameObject.transform.position, Quaternion.identity);
        go.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
        // BulletMovement bm = go.AddComponent<BulletMovement>();
        BulletMovement bm = go.GetComponent<BulletMovement>();
        bm.srcObjId = id;
        bm.velocity = bv;
        bm.dmg = wd;
        
        
        Destroy(go, br);

        return go;
    }
    private void AllowFire()
    {
        canFire = true;
    }
    public void OnMove(float x = 0f, float y = 0f)
    {
        
        moveDelta = new Vector3(x, y, 0);
        var _transfrom = PlayerObject.transform;

        // Changin direction
        /*
        if (moveDelta.x > 0)
        {
            _transfrom.localScale = Vector3.one;
        }
        else if (moveDelta.x < 0)
        {
            _transfrom.localScale = new Vector3(-1, 1, 1);
        }
        */



        hit = Physics2D.BoxCast(_transfrom.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask(BLOCKING_LAYER_NAMES));
        if (hit.collider == null)
        {
            transform.Translate(moveDelta.x * Time.deltaTime, 0f, 0f);
        }
        hit = Physics2D.BoxCast(_transfrom.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask(BLOCKING_LAYER_NAMES));
        if (hit.collider == null)
        {
            transform.Translate(0f, moveDelta.y * Time.deltaTime, 0f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //pickup collectable items
        collision.TryGetComponent<ItemWorld>(out ItemWorld iw);
        if (iw != null && iw.IsPickupAble)
        {
            //touch item
            if(inventory.AddItem(iw.getItem())) iw.DestroySelf(); ;
            
        }
    }
    private void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.HealthPotion:
                inventory.RemoveItem(item);
                damageable.Regenerate(3);
                break;
            case ItemType.Coin:
                break;
            case ItemType.Pistol:
            case ItemType.AR:
            case ItemType.RocketLauncher:
            case ItemType.Head:
            case ItemType.Body:
                inventory.EquipItem(item);
                break;
            default:
                break;
        }
    }
    public void DropItem(Vector3 dropPos, Vector3 direction, Item item, bool onlyOne = true)
    {
        ItemStructNetcode itemStruct = new ItemStructNetcode();
        itemStruct.SetStruct(item);
        if (onlyOne)
        {
            itemStruct.amount = 1;
        }
        DropItemServerRpc( dropPos,  direction, itemStruct);
    }
    [ServerRpc(RequireOwnership =false)]
    public void DropItemServerRpc(Vector3 dropPos, Vector3 direction, ItemStructNetcode item)
    {
        Vector3 dir = new Vector3(direction.x, direction.y).normalized;
        Vector3 pos = dropPos; // + dir * 0.2f;
        ItemWorld iw = ItemWorld.SpawnItemWorld(pos, new Item(item.itemType, item.itemTier, item.amount));
        iw.GetComponent<Rigidbody2D>().AddForce(dir * 1.3f, ForceMode2D.Impulse);
        iw.ItemStructNetVar.Value = new ItemStructNetcode(item);
        iw.GetComponent<NetworkObject>().Spawn();
    }

    public void ResetCharacter()
    {
        Vector3 spawnpos = GameManager.Instance.SpawnInfo.SpawnLocation;
        Vector3 ppos = transform.position;
        transform.Translate(spawnpos - ppos);

        inventory.ResetInventory();
    }
}
