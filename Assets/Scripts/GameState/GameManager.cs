using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.AI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public event EventHandler LevelChanged;

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

    public int level = 1;
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

    private void StartRound()
    {
        bool isBossRound = false;
        if (IsRoundGoing) return;
        IsRoundGoing = true;
        if(level == 0)
        {
            InitFirstMap();
        } else
        {
            if (level % 4 == 0) isBossRound = true;
            InitNextMap(isBossRound);
        }
        SpawnEnemies(isBossRound);
        level += 1;
        LevelChanged?.Invoke(this, EventArgs.Empty);
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
    private void InitNextMap(bool isBossRound)
    {
        Destroy(ActiveBlockingGate.gameObject);

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
    private void SpawnEnemies(bool isBossRound)
    {
        int EnemiesToSpawn = !isBossRound ? UnityEngine.Random.Range(2, (int)(level/3) + 3) : 1;

        int SpawnerCount = ActiveMapInfo.EnemySpawnLocations.Count;
        while(EnemiesToSpawn > 0)
        {
            enemies.Add(SpawnNpc(ActiveMapInfo.EnemySpawnLocations[EnemiesToSpawn%SpawnerCount], level, isBossRound));
            EnemiesToSpawn -= 1;
        }
    }
    private EnemyAI SpawnNpc(Vector3 SpawnPos, int level, bool isBoss)
    {
        GameObject npc = Instantiate(npcPrefab, SpawnPos, Quaternion.identity, NpcHolderGameobject.transform);
        EnemyAI EnemyLogic = npc.transform.Find("Script").GetComponent<EnemyAI>();

        if (!isBoss)
        {
            Transform sprite = npc.transform.Find("Sprite");
            sprite.localScale = new Vector3(1, 1, 1);
            int SpriteInd = UnityEngine.Random.Range(0, ItemAssets.Instance.SimpleEnemySprites.Length);
            EnemyLogic.SpriteIdNetvar.Value = SpriteInd;
            //sprite.GetComponent<SpriteRenderer>().sprite = ItemAssets.Instance.SimpleEnemySprites[SpriteInd];
        } else
        {
            Transform sprite = npc.transform.Find("Sprite");
            sprite.localScale = new Vector3(1, 1, 1);
            EnemyLogic.IsBossNetvar.Value = true;
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
}
