using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
public class EnemyAI : NetworkBehaviour
{
    public NetworkVariable<int> SpriteIdNetvar = new NetworkVariable<int>();
    public bool IsBoss = false;
    public int SpriteId = 0;

    public bool HasTarget = false;
    public Transform Target;
    public NavMeshAgent Agent;
    public Transform AgentSprite;
    public GameObject Parent;
    public GameObject bullet;

    private Vector3 lastPos;

    public LayerMask LayerGround, LayerPlayer, LayerBlocking;
    public float DestinationReachedMargin = 0.5f;

    // Config
    public float ChaseSpeed;
    public float PatrolSpeed;
    public float TimeBetweenAttacks;
    public float BulletVelocity;
    public int WeaponDamage;

    public bool alreadyAttacked;
    public bool IsBotParamsSet = false;

    //Patroling
    public Vector3 WalkPoint;
    public bool WalkPointSet;
    public float WalkPointRange;

    //States
    public float SightRange, AttackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    public override void OnNetworkSpawn()
    {
        SpriteIdNetvar.OnValueChanged += OnSpriteIdChanged;
        AgentSprite.GetComponent<SpriteRenderer>().sprite = ItemAssets.Instance.SimpleEnemySprites[SpriteIdNetvar.Value];
        if (IsServer || IsHost)
        {
            Agent.updateUpAxis = false;
            Agent.updateRotation = false;
            Agent.speed = 0.5f;
            Agent.acceleration = 1f;

            lastPos = Vector3.zero;

            transform.rotation = Quaternion.identity;
        }
          
    }
	
    private void OnSpriteIdChanged(int previousValue, int newValue)
    {
        AgentSprite.GetComponent<SpriteRenderer>().sprite = ItemAssets.Instance.SimpleEnemySprites[SpriteIdNetvar.Value];
    }

    void Update()
    {
        if (IsServer)
        {
            doUpdate();
        }
    }

    void FindTarget()
    {
        GameObject[] founds = GameObject.FindGameObjectsWithTag("Player");
        if (founds.Length>0)
        {
            float minDist = float.MaxValue;
            GameObject minObj = null;
            foreach (var item in founds)
            {
                float d = Vector3.Distance(item.transform.position, transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    minObj = item;
                }
            }

            Target = minObj.transform;
            HasTarget = true;
        }
        
    }

    void setRotation()
    {

        var newPos = transform.position;
        var dir = newPos - lastPos;
        transform.up = dir;

        lastPos = newPos;
    }
    
    void doUpdate()
    {

        if (HasTarget)
        {
            if (Target == null) HasTarget = false;
        }
        
        PlayerInSightRange = Physics2D.OverlapCircle(transform.position, SightRange, LayerPlayer);
        PlayerInAttackRange = Physics2D.OverlapCircle(transform.position, AttackRange, LayerPlayer);

        if (!PlayerInSightRange && !PlayerInAttackRange) {
            HasTarget = false;
            if (Agent.speed != PatrolSpeed) Agent.speed = PatrolSpeed;
            Patroling();
        } 
        else
        {
            if(HasTarget == false)
            {
                FindTarget();
            }
            if (PlayerInSightRange && !PlayerInAttackRange)
            {
                if (Agent.speed != ChaseSpeed) Agent.speed = ChaseSpeed;
                ChasePlayer();
            }
            if (PlayerInSightRange && PlayerInAttackRange) AttackPlayer();
        }

        setRotation();

    }

    private void Patroling()
    {
        if (!WalkPointSet) SearchWalkPoint();
        if (WalkPointSet)
        {
            Agent.SetDestination(WalkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - WalkPoint;
        AgentSprite.up = new Vector3(WalkPoint.x, WalkPoint.y, 0) - new Vector3(transform.position.x, transform.position.y, 0);
        if (distanceToWalkPoint.magnitude < DestinationReachedMargin)
        {
            WalkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        MapInfo room = GameManager.Instance.ActiveMapInfo;
        if(room == null)
        {
            Debug.Log("MAPINFO NOT FOUND ON ENEMY AGENT");
            return;
        }
        float y_len = room.Ending.y - room.Begining.y;
        float x_middle = room.Begining.x;


        float randomX = Random.Range(x_middle - y_len/2, x_middle + y_len / 2);
        float randomY = Random.Range(-room.Begining.y, room.Ending.y);

        WalkPoint = new Vector3(randomX, randomY, 0);

        int MAX_SEARCH_PER_FRAME = 1;

        while(MAX_SEARCH_PER_FRAME>0 && Physics2D.Raycast(WalkPoint, transform.forward, 1f, LayerGround) && !Physics2D.Raycast(WalkPoint, transform.forward, 1f, LayerBlocking))
        {
            MAX_SEARCH_PER_FRAME--;

            randomX = Random.Range(-WalkPointRange, WalkPointRange);
            randomY = Random.Range(-WalkPointRange, WalkPointRange);

            WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        }

        WalkPointSet = true;
    }
    private void ChasePlayer()
    {
        Agent.SetDestination(Target.position);
        AgentSprite.up = new Vector3(Target.position.x, Target.position.y, 0) - new Vector3(transform.position.x, transform.position.y, 0);
    }
    private void AttackPlayer()
    {
        Agent.SetDestination(transform.position);
        AgentSprite.up = new Vector3(Target.position.x, Target.position.y, 0) - new Vector3(transform.position.x, transform.position.y, 0);

        AttackLogicServerRpc();
    }
    [ServerRpc]
    private void AttackLogicServerRpc()
    {
        if (!alreadyAttacked)
        {
            var dir = Target.position - transform.position;
            GameObject b = BulletMovement.SetupBulletInst(transform.position, dir, NetworkObjectId, bullet, BulletVelocity, WeaponDamage);
            AttackLogicClientRpc(transform.position, dir, NetworkObjectId, BulletVelocity, WeaponDamage);
            alreadyAttacked = true;
            Invoke(nameof(enableAttacked), TimeBetweenAttacks);
        }
    }
    [ClientRpc]
    private void AttackLogicClientRpc(Vector3 pos, Vector3 d, ulong id, float bulletVelocity, int weaponDamage)
    {
        if (IsHost) return;
        BulletMovement.SetupBulletInst(pos, d, id, bullet, bulletVelocity, weaponDamage);
    }
    private void enableAttacked()
    {
        alreadyAttacked = false;
    }
    
    public Transform SearchClosestPlayer()
    {
      GameObject[] founds = GameObject.FindGameObjectsWithTag("Actor");
      if (founds.Length == 0)
      {
           Debug.Log("Not found player");
           return null;
      } else
      {
          /// MIN SEARCH
          float minDist = float.MaxValue;
          Transform minTarg = null;
          foreach (var item in founds)
          {
              float d = Vector3.Distance(item.transform.position, transform.position);
              if (d < minDist)
              {
                  minDist = d;
                  minTarg = item.gameObject.transform;
              }
          }
            Debug.Log(minTarg.ToString());
            return minTarg;
      }

      
    }

}