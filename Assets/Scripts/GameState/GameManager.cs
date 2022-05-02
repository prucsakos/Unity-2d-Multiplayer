using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.AI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public static int[] EnemyCountPerLevel = new int[] { 2, 4, 6, 6 };

    [SerializeField] public GameObject NpcHolderGameobject;
    [SerializeField] public GameObject npcPrefab;


    [SerializeField] public GameObject MapHolder;
    [SerializeField] public GameObject SpawnTile;
    [SerializeField] public NavMeshSurface2d NavMeshBuilder;
    public MapInfo SpawnInfo;
    public Transform ActiveBlockingGate;
    public MapInfo ActiveBlockingGateInfo;
    public Transform ActiveMap;
    public MapInfo ActiveMapInfo;

    public int level = 0;
    public bool IsRoundGoing = false;

    private List<EnemyAI> enemies;
    private void Awake()
    {
        
        Instance = this;
        enemies = new List<EnemyAI>();
        SpawnInfo = new MapInfo(SpawnTile.transform.Find("Metadata").gameObject);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            StartRound();
        }
    }
    public void PlayerJoined(Damageable d)
    {
        d.Died += OnPlayerDied;
    }

    private void OnPlayerDied(object sender, EventArgs e)
    {
        //
    }

    private void OnEnemyDied(object sender, EventArgs e)
    {
        Damageable damageable = (Damageable)sender;
        enemies.Remove(damageable.GetComponent<EnemyAI>());

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

    private void StartRound()
    {
        if (IsRoundGoing) return;
        IsRoundGoing = true;
        if(level == 0)
        {
            InitFirstMap();
        } else
        {
            InitNextMap();
        }
        SpawnEnemies();
        level += 1;
    }
    private void InitFirstMap()
    {
        MapInfo mi_spawn = new MapInfo(SpawnTile.transform.Find("Metadata").gameObject);
        GameObject ArenaMap = ItemAssets.Instance.Room1.gameObject;
        MapInfo mi_room = new MapInfo(ArenaMap.transform.Find("Metadata").gameObject);
        Vector3 arenaPos = ArenaMap.transform.position;
        Vector3 beginingPos = mi_room.Begining;
        Vector3 shiftmap = arenaPos - beginingPos;
        ActiveMap = Instantiate(ArenaMap, mi_spawn.Ending + shiftmap, Quaternion.identity, MapHolder.transform).transform;
        ActiveMapInfo = new MapInfo(ActiveMap.transform.Find("Metadata").gameObject);
        ActiveMap.GetComponent<NetworkObject>().Spawn();

        GameObject BlockingGate = ItemAssets.Instance.BlockingGate.gameObject;
        MapInfo mi_block = new MapInfo(BlockingGate.transform.Find("Metadata").gameObject);
        Vector3 blockPos = BlockingGate.transform.position;
        Vector3 blockCenter = mi_block.Center;
        shiftmap = blockPos - blockCenter;
        ActiveBlockingGate = Instantiate(BlockingGate, ActiveMapInfo.Ending + shiftmap, Quaternion.identity, MapHolder.transform).transform;
        ActiveBlockingGateInfo = new MapInfo(ActiveBlockingGate.transform.Find("Metadata").gameObject);
        ActiveBlockingGate.GetComponent<NetworkObject>().Spawn();

        NavMeshBuilder.BuildNavMesh();
    }
    private void InitNextMap()
    {
        Destroy(ActiveBlockingGate.gameObject);

        GameObject ArenaMap = ItemAssets.Instance.Room1.gameObject;
        MapInfo mi_room = new MapInfo(ArenaMap.transform.Find("Metadata").gameObject);
        Vector3 arenaPos = ArenaMap.transform.position;
        Vector3 beginingPos = mi_room.Begining;
        Vector3 shiftmap = arenaPos - beginingPos;
        ActiveMap = Instantiate(ArenaMap, ActiveMapInfo.Ending + shiftmap, Quaternion.identity, MapHolder.transform).transform;
        ActiveMapInfo = new MapInfo(ActiveMap.transform.Find("Metadata").gameObject);
        ActiveMap.GetComponent<NetworkObject>().Spawn();

        GameObject BlockingGate = ItemAssets.Instance.BlockingGate.gameObject;
        MapInfo mi_block = new MapInfo(BlockingGate.transform.Find("Metadata").gameObject);
        Vector3 blockPos = BlockingGate.transform.position;
        Vector3 blockCenter = mi_block.Center;
        shiftmap = blockPos - blockCenter;
        ActiveBlockingGate = Instantiate(BlockingGate, ActiveMapInfo.Ending + shiftmap, Quaternion.identity, MapHolder.transform).transform;
        ActiveBlockingGateInfo = new MapInfo(ActiveBlockingGate.transform.Find("Metadata").gameObject);
        ActiveBlockingGate.GetComponent<NetworkObject>().Spawn();

        NavMeshBuilder.BuildNavMesh();
    }
    private void SpawnEnemies()
    {
        int EnemiesToSpawn = GetEnemyCount(level);
        int SpawnerCount = ActiveMapInfo.EnemySpawnLocations.Count;
        while(EnemiesToSpawn > 0)
        {
            enemies.Add(SpawnNpc(ActiveMapInfo.EnemySpawnLocations[EnemiesToSpawn%SpawnerCount]));
            EnemiesToSpawn -= 1;
        }
    }
    private EnemyAI SpawnNpc(Vector3 SpawnPos)
    {
        GameObject npc = Instantiate(npcPrefab, SpawnPos, Quaternion.identity, NpcHolderGameobject.transform);
        EnemyAI EnemyLogic = npc.GetComponent<EnemyAI>();

        // config npc
        npc.GetComponent<Damageable>().Died += OnEnemyDied;
        
        npc.GetComponent<NetworkObject>().Spawn();
        return EnemyLogic;
    }
    private int GetEnemyCount(int lvl)
    {
        if (lvl >= EnemyCountPerLevel.Length) return EnemyCountPerLevel[EnemyCountPerLevel.Length - 1];
        return EnemyCountPerLevel[lvl];
    }
}
