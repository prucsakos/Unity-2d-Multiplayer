using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public GameObject PlayerObject;
    public GameObject BulletPrefab;
    public GameObject GunPrefab;
    //public GameObject UI_INV_GO;
    public Vector3 mouseDir;
    public float lookAngle;
    
    private BoxCollider2D boxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;

    public Camera cam;
    public Inventory inventory;
    [SerializeField] public UIInventory uiInventory;
    [SerializeField] public UIXpBar UiXpBar;
    public bool isGUIOpened = false;

    public bool canFire = true;
    public float fireRate = 0.1f;
    public int bulletDmg = 1;
    public float bulletVelocity = 3f;
    public float bulletRange = 3f;
    
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

        inventory = new Inventory(UseItem);
        uiInventory.SetInventory(inventory);
        uiInventory.gameObject.SetActive(false);
        uiInventory.SetPlayer(this);
        UiXpBar.setXpBar(inventory);

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
            ShootServerRpc(transform.position, lookAngle, NetworkObject.NetworkObjectId);

            canFire = false;
            Invoke(nameof(AllowFire), fireRate);
        }
    }
    [ServerRpc]
    void ShootServerRpc(Vector3 v, float lookAngle, ulong id)
    {
        SetupBulletInst(v, lookAngle, id);
        /*
        var bull = SetupBulletInst(v, lookAngle);
        
        if (bull.TryGetComponent<NetworkObject>(out NetworkObject no))
        {
            no.Spawn();
        }
        */
        ShootClientRpc(v, lookAngle, id);

    }
    [ClientRpc]
    void ShootClientRpc(Vector3 v, float lookAngle, ulong id)
    {
        if (IsHost) return;
        SetupBulletInst(v, lookAngle, id);
    }
    GameObject SetupBulletInst(Vector3 v, float lookAngle, ulong id)
    {
        GameObject go = Instantiate(BulletPrefab, gameObject.transform.position, Quaternion.identity);
        go.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
        // BulletMovement bm = go.AddComponent<BulletMovement>();
        BulletMovement bm = go.GetComponent<BulletMovement>();
        bm.srcObjId = id;
        if(IsServer) Debug.Log("Szerver átadja: " + bulletVelocity.ToString());
        else if(IsClient) Debug.Log("Cliens átadja: " + bulletVelocity.ToString());
        bm.velocity = bulletVelocity;
        bm.dmg = bulletDmg;
        
        
        Destroy(go, bulletRange);

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



        hit = Physics2D.BoxCast(_transfrom.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("AI", "Actor", "Blocking"));
        if (hit.collider == null)
        {
            transform.Translate(moveDelta.x * Time.deltaTime, 0f, 0f);
        }
        hit = Physics2D.BoxCast(_transfrom.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("AI", "Actor", "Blocking"));
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
            inventory.AddItem(iw.getItem());
            iw.DestroySelf();
        }
    }
    private void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.HealthPotion:
                inventory.RemoveItem(new Item { itemType = item.itemType, amount = 1 });
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
        ItemWorld iw = ItemWorld.SpawnItemWorld(pos, new Item() { itemType = item.itemType, amount = item.amount });
        iw.GetComponent<Rigidbody2D>().AddForce(dir * 1.3f, ForceMode2D.Impulse);
        iw.ItemStructNetVar.Value = new ItemStructNetcode(item);
        iw.GetComponent<NetworkObject>().Spawn();
    }
}
