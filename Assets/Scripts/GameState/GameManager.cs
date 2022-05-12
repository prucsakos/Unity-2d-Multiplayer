using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.AI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Transform MapCamera;
    public NetworkVariable<Vector3> CameraPos = new NetworkVariable<Vector3>(new Vector3(0,0,0));

    public static int[] EnemyCountPerLevel = new int[] { 2, 4, 6, 6 };

    public Transform SpawnedObjects;
    public List<Transform> SpawnedRoomList = new List<Transform>();
    public Transform SpawnedRooms;
    public Transform SpawnedNpcs;
    public Transform Chests;

    [SerializeField] public GameObject NpcHolderGameobject;
    [SerializeField] public GameObject npcPrefab;

    [SerializeField] public GameObject MapHolder;
    [SerializeField] public GameObject SpawnTile;
    [SerializeField] public NavMeshSurface2d MyNavMeshBuilder;

    public MapInfo SpawnInfo;

    public Transform ActiveBlockingGate;
    public MapInfo ActiveBlockingGateInfo;

    public Transform ActiveMap;
    public MapInfo ActiveMapInfo;

    public NetworkVariable<int> NetLevel = new NetworkVariable<int>();

    public bool IsRoundGoing = false;

    //private List<NetworkClient> ConnectedPlayers = new List<NetworkClient>();
    //private List<ulong> ConnectedPlayerIds = new List<ulong>();
    private List<EnemyAI> enemies;
    private void Awake()
    {
        
        Instance = this;
        enemies = new List<EnemyAI>();
        SpawnInfo = new MapInfo(SpawnTile.transform.Find("Metadata").gameObject);
        CameraPos.OnValueChanged += OnGlobalCameraPosChanged;

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost || IsServer)
        {
            StartRound();
            
        }
    }
    /*
    public void PlayerJoined(Damageable d)
    {
        d.Died += OnPlayerDied;
    }
    */
    private void OnPlayerDied(object sender, EventArgs e)
    {
        ulong ownerId = ((Damageable)sender).OwnerId;
        Destroy(((Damageable)sender).gameObject);
        if (IsEveryoneOut(ownerId))
        {
            ResetGame();
            ReviveEveryone();
        }
    }

    private void OnEnemyDied(object sender, EventArgs e)
    {
        Damageable damageable = (Damageable)sender;
        enemies.Remove(damageable.transform.Find("Script").GetComponent<EnemyAI>());
        Debug.Log($"Ellenfél meghalt, maradt: {enemies.Count}");

        damageable.Died -= OnEnemyDied;
        // SPAWN LOOT
        ItemWorldSpawner iws = new ItemWorldSpawner(damageable.transform.position);
        iws.GenerateDropForNPC();
        Destroy(damageable.gameObject);
        
        if(enemies.Count <= 0)
        {
            IsRoundGoing = false;
            StartRound();
        }
    }
    private void OnGlobalCameraPosChanged(Vector3 previousValue, Vector3 newValue)
    {
        MapCamera.position = newValue;
    }


    private void StartRound()
    {
        bool isBossRound = (NetLevel.Value % 4 == 0 && NetLevel.Value != 0) ? true : false;
        if (IsRoundGoing) return;
        IsRoundGoing = true;

        if(NetLevel.Value == 0)
        {
            InitFirstMap(isBossRound);
        } else
        {
            InitNextMap(isBossRound);
        }
        SpawnEnemies(isBossRound);
        NetLevel.Value += 1;
    }
    private void InitFirstMap(bool isBossRound)
    {
        Vector3 shiftmap;

        if(NetLevel.Value >= SpawnedRoomList.Count)
        {
            MapInfo mi_spawn = new MapInfo(SpawnTile.transform.Find("Metadata").gameObject);
            GameObject ArenaMap;
            if (isBossRound)
            {
                ArenaMap = ItemAssets.Instance.BossRoom.gameObject;
            }
            else
            {
                int mapind = UnityEngine.Random.Range(0, ItemAssets.Instance.SimpleRoomList.Length);
                ArenaMap = ItemAssets.Instance.SimpleRoomList[mapind].gameObject;
            }
            MapInfo mi_room = new MapInfo(ArenaMap.transform.Find("Metadata").gameObject);
            Vector3 arenaPos = ArenaMap.transform.position;
            Vector3 beginingPos = mi_room.Begining;
            shiftmap = arenaPos - beginingPos;
            Transform inst = Instantiate(ArenaMap, mi_spawn.Ending + shiftmap, Quaternion.identity, SpawnedRooms).transform;
            ActiveMap = inst;
            SpawnedRoomList.Add(inst);
            ActiveMapInfo = new MapInfo(ActiveMap.transform.Find("Metadata").gameObject);
            ActiveMap.GetComponent<NetworkObject>().Spawn();
        } else
        {
            ActiveMap = SpawnedRoomList[NetLevel.Value];
            ActiveMapInfo = new MapInfo(ActiveMap.transform.Find("Metadata").gameObject);
        }   


        GameObject BlockingGate = ItemAssets.Instance.BlockingGate.gameObject;
        MapInfo mi_block = new MapInfo(BlockingGate.transform.Find("Metadata").gameObject);
        Vector3 blockPos = BlockingGate.transform.position;
        Vector3 blockCenter = mi_block.Center;
        shiftmap = blockPos - blockCenter;
        ActiveBlockingGate = Instantiate(BlockingGate, ActiveMapInfo.Ending + shiftmap, Quaternion.identity, MapHolder.transform).transform;
        ActiveBlockingGateInfo = new MapInfo(ActiveBlockingGate.transform.Find("Metadata").gameObject);
        ActiveBlockingGate.GetComponent<NetworkObject>().Spawn();

        CameraPos.Value = new Vector3(ActiveMapInfo.Ending.x, ActiveMapInfo.Ending.y - 1f, -1);
        MyNavMeshBuilder.BuildNavMesh();
    }
    private void InitNextMap(bool isBossRound)
    {
        Destroy(ActiveBlockingGate.gameObject);
        Vector3 shiftmap;

        if(NetLevel.Value >= SpawnedRoomList.Count)
        {
            GameObject ArenaMap;
            if (isBossRound)
            {
            ArenaMap = ItemAssets.Instance.BossRoom.gameObject;
            }else
            {
                int mapind =  UnityEngine.Random.Range(0, ItemAssets.Instance.SimpleRoomList.Length);
                ArenaMap = ItemAssets.Instance.SimpleRoomList[mapind].gameObject;
            }

            MapInfo mi_room = new MapInfo(ArenaMap.transform.Find("Metadata").gameObject);
            Vector3 arenaPos = ArenaMap.transform.position;
            Vector3 beginingPos = mi_room.Begining;
            shiftmap = arenaPos - beginingPos;
            Transform inst = Instantiate(ArenaMap, ActiveMapInfo.Ending + shiftmap, Quaternion.identity, SpawnedRooms).transform;
            ActiveMap = inst;
            SpawnedRoomList.Add(inst);
            ActiveMapInfo = new MapInfo(ActiveMap.transform.Find("Metadata").gameObject);
            ActiveMap.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            ActiveMap = SpawnedRoomList[NetLevel.Value];
            ActiveMapInfo = new MapInfo(ActiveMap.transform.Find("Metadata").gameObject);
        }


        GameObject BlockingGate = ItemAssets.Instance.BlockingGate.gameObject;
        MapInfo mi_block = new MapInfo(BlockingGate.transform.Find("Metadata").gameObject);
        Vector3 blockPos = BlockingGate.transform.position;
        Vector3 blockCenter = mi_block.Center;
        shiftmap = blockPos - blockCenter;
        ActiveBlockingGate = Instantiate(BlockingGate, ActiveMapInfo.Ending + shiftmap, Quaternion.identity, MapHolder.transform).transform;
        ActiveBlockingGateInfo = new MapInfo(ActiveBlockingGate.transform.Find("Metadata").gameObject);
        ActiveBlockingGate.GetComponent<NetworkObject>().Spawn();

        CameraPos.Value = new Vector3(ActiveMapInfo.Ending.x, ActiveMapInfo.Ending.y - 1f, -1);
        MyNavMeshBuilder.BuildNavMesh();
    }
    private void SpawnEnemies(bool isBossRound)
    {
        int EnemiesToSpawn = !isBossRound ? UnityEngine.Random.Range(2, (int)(NetLevel.Value / 3) + 3) : 1;

        int SpawnerCount = ActiveMapInfo.EnemySpawnLocations.Count;
        while(EnemiesToSpawn > 0)
        {
            enemies.Add(SpawnNpc(ActiveMapInfo.EnemySpawnLocations[EnemiesToSpawn%SpawnerCount], NetLevel.Value, isBossRound));
            EnemiesToSpawn -= 1;
        }
    }
    private EnemyAI SpawnNpc(Vector3 SpawnPos, int level, bool isBoss)
    {
        GameObject npc = Instantiate(npcPrefab, SpawnPos, Quaternion.identity, NpcHolderGameobject.transform);
        EnemyAI EnemyLogic = npc.transform.Find("Script").GetComponent<EnemyAI>();
        Transform sprite = npc.transform.Find("Sprite");

        if (!isBoss)
        {
            int SpriteInd = UnityEngine.Random.Range(1, ItemAssets.Instance.SimpleEnemySprites.Length);
            EnemyLogic.SpriteIdNetvar.Value = SpriteInd;
            //EnemyLogic.IsBossNetvar.Value = false;
            //sprite.GetComponent<SpriteRenderer>().sprite = ItemAssets.Instance.SimpleEnemySprites[SpriteInd];
        } else
        {
            EnemyLogic.SpriteIdNetvar.Value = 0;
            //EnemyLogic.IsBossNetvar.Value = true;
            //sprite.GetComponent<SpriteRenderer>().sprite = ItemAssets.Instance.BossSprite;
        }

        Damageable EnemyDamagable = npc.GetComponent<Damageable>();

        // config npc
        EnemyLogic.TimeBetweenAttacks = (EnemyLogic.TimeBetweenAttacks / (1f + level * 0.1f)) < 0.1f ? 0.1f : (EnemyLogic.TimeBetweenAttacks / (1f + level * 0.1f));
        EnemyLogic.BulletVelocity = (EnemyLogic.BulletVelocity + level * 0.2f) > 3f ? 3f : (EnemyLogic.BulletVelocity + level * 0.2f);
        EnemyLogic.WeaponDamage = EnemyLogic.WeaponDamage + level;
        EnemyDamagable.SetHp(EnemyDamagable.MaxHP + level * 5);

        // boss
        if (isBoss)
        {
            EnemyLogic.TimeBetweenAttacks /= 1.5f;
            EnemyLogic.BulletVelocity *= 1.5f;
            EnemyLogic.WeaponDamage += 5;
            EnemyDamagable.SetHp(EnemyDamagable.GetMaxHP() * 3);
        }

        EnemyDamagable.Died += OnEnemyDied;

        npc.GetComponent<NetworkObject>().Spawn();

        return EnemyLogic;
    }
    private int GetEnemyCount(int lvl)
    {
        if (lvl >= EnemyCountPerLevel.Length) return EnemyCountPerLevel[EnemyCountPerLevel.Length - 1];
        return EnemyCountPerLevel[lvl];
    }

    // Connected Players List functions
    public void ClientJoined(ulong id)
    {
        NetworkClient nc = NetworkManager.Singleton.ConnectedClients[id];
        Debug.Log($"Client count: {NetworkManager.Singleton.ConnectedClients.Count}");
        // If host (first joiner)
        if (IsEveryoneOut())
        {
            ReviveEveryone();
        } else
        {
            if (IsEveryoneAlive(id))
            {
                Revive(nc);
            }
        }
        NetLevel.Value = NetLevel.Value;
    }

    private void RefreshNetworkList()
    {
        /*
        NetworkManager.
        foreach (NetworkClient item in ConnectedPlayers)
        {
            if(item == null)
            {
                ConnectedPlayers.Remove(item);
            }
        }
        */
    }
    private bool IsEveryoneOut(ulong exclude = ulong.MaxValue)
    {
        foreach (NetworkClient item in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (item.PlayerObject != null && item.ClientId != exclude)
            {
                Debug.Log($"IsEveryoneOut NOT OUT: {item.ClientId}");
                return false;
            }
        }
        return true;
    }
    private bool IsEveryoneAlive(ulong exclude = ulong.MaxValue)
    {

        foreach (NetworkClient item in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log($"ConnectedPlayers List item.playero: {item.PlayerObject}");
            if (item.PlayerObject == null && item.ClientId != exclude)
            {
                Debug.Log("IsEveryoneAlive: False");
                return false;
            }
        }
        Debug.Log("IsEveryoneAlive: True");
        return true;
    }
    private void ReviveEveryone()
    {
        // only if player object doesnt exist
        foreach (NetworkClient item in NetworkManager.Singleton.ConnectedClientsList)
        {
            Revive(item);
        }
    }
    private void Revive(NetworkClient nc)
    {
        Debug.Log($"Start Reviving ID: {nc.ClientId}");
        Transform obj = Instantiate(ItemAssets.Instance.PlayerPrefab, Vector3.zero, Quaternion.identity);
        obj.GetComponent<Damageable>().Died += OnPlayerDied;
        obj.GetComponent<Damageable>().OwnerId = nc.ClientId;
        NetworkObject no = obj.GetComponent<NetworkObject>();
        no.SpawnAsPlayerObject(nc.ClientId);
    }
    private void ResetGame()
    {
        foreach (Transform item in SpawnedNpcs)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in SpawnedObjects)
        {
            Destroy(item.gameObject);
        }
        Destroy(ActiveBlockingGate.gameObject);

        
        foreach (Transform item in Chests)
        {
            item.GetComponent<Chest>().isOpened.Value = false;
        }

        NetLevel.Value = 0;
        IsRoundGoing = false;
        enemies = new List<EnemyAI>();

        StartRound();
    }
}
