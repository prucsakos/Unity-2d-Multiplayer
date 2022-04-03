using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class BulletMovement : NetworkBehaviour
{
    public int dmg;
    public float velocity;
    // public Rigidbody2D rb;
    // public CircleCollider2D cc;
    public ulong srcObjId;  // Network Id from NetworkObject
    //public Vector2 dir;


    // Start is called before the first frame update
    void Start()
    {
        
        gameObject.layer = 11;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Bullet";
        /*
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        cc = gameObject.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
        */
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(dir * velocity * Time.deltaTime);
        transform.Translate(velocity * Time.deltaTime, 0, 0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if instance is the shooter.
        if(collision.gameObject.TryGetComponent<NetworkObject>(out NetworkObject no))
        {
            if (no.NetworkObjectId == srcObjId)
            {
                //Debug.Log("Same Ids");
                return;
            }
        }
        if(IsServer) Debug.Log("Name: " + collision.name);
        // Check if instance is a Damageable
        if (collision.gameObject.TryGetComponent<Damageable>(out Damageable d))
        {
            if (IsServer) //Damage 
            {
                d.TakeDMG(dmg);
            }
            Destroy(gameObject);
            return;
        }
        // Check if instance is a Blocking layer.
        if (collision.name == "Blocking")
        {
            Destroy(gameObject);
            return;
        }
    }
    /*
    // Server call
    void DestroyCall()
    {
        Destroy(gameObject);
        DestroyEverywhere_ClientRpc();
    }

    [ClientRpc]
    void DestroyEverywhere_ClientRpc()
    {
        if (IsHost) return;
        Destroy(gameObject);
    }
    */
}
