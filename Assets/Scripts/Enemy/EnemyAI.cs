using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyAI : NetworkBehaviour
{

    public bool hasTarget = false;
    public Transform target;
    public NavMeshAgent agent;

    private Vector3 lastPos;

    public LayerMask whatIsGround, whatIsPlayer, whatIsBlocking;
    public float destinationReachedMargin = 0.3f;


    //Patroling
    public float patrolVelocity;
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float chaseVelocity;
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject bullet;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("INIT");
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.updateUpAxis = false;
            agent.updateRotation = false;
            agent.speed = 0.5f;
            agent.acceleration = 1f;

            lastPos = Vector3.zero;

            transform.rotation = Quaternion.identity;
        }
          
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

            target = minObj.transform;
            hasTarget = true;
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

        if (hasTarget)
        {
            if (target == null) hasTarget = false;
        }
        
        playerInSightRange = Physics2D.OverlapCircle(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) {
            hasTarget = false;
            if (agent.speed != patrolVelocity) agent.speed = patrolVelocity;
            Patroling();
        } 
        else
        {
            if(hasTarget == false)
            {
                FindTarget();
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                if (agent.speed != chaseVelocity) agent.speed = chaseVelocity;
                ChasePlayer();
            }
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }

        setRotation();

    }

    /// enemy logics under
    
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if(distanceToWalkPoint.magnitude < destinationReachedMargin)
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y +  randomY, 0);

        int MAX_SEARCH_PER_FRAME = 2;

        while(MAX_SEARCH_PER_FRAME>0 && Physics2D.Raycast(walkPoint, transform.forward, 1f, whatIsGround) && !Physics2D.Raycast(walkPoint, transform.forward, 1f, whatIsBlocking))
        {
            MAX_SEARCH_PER_FRAME--;

            randomX = Random.Range(-walkPointRange, walkPointRange);
            randomY = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        }

        walkPointSet = true;
    }
    private void ChasePlayer()
    {
        Debug.Log("chase");
        agent.SetDestination(target.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.up = target.position - transform.position;

        AttackLogicServerRpc();
    }
    [ServerRpc]
    private void AttackLogicServerRpc()
    {
        if (!alreadyAttacked)
        {
            var dir = target.position - transform.position;
            GameObject b = BulletMovement.SetupBulletInst(transform.position, dir, NetworkObjectId, bullet);
            AttackLogicClientRpc(transform.position, dir, NetworkObjectId);
            alreadyAttacked = true;
            Invoke(nameof(enableAttacked), timeBetweenAttacks);
        }
    }
    [ClientRpc]
    private void AttackLogicClientRpc(Vector3 pos, Vector3 d, ulong id)
    {
        if (IsHost) return;
        BulletMovement.SetupBulletInst(pos, d, id, bullet);
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