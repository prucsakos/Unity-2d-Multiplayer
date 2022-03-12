using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public int dmg;
    public float velocity;
    public Rigidbody2D rb;
    public CircleCollider2D cc;
    //public Vector2 dir;

    private BulletMovement bm;

    // Start is called before the first frame update
    void Start()
    {
        bm = this;

        this.gameObject.layer = 11;

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        cc = gameObject.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(dir * velocity * Time.deltaTime);
        transform.Translate(1 * velocity * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Name: " + collision.name);
        Damageable d;
        if( collision.gameObject.TryGetComponent<Damageable>(out d))
        {
            d.TakeDMG(dmg);
            Destroy(gameObject);
            return;
        }
        if(collision.name == "Blocking")
        {
            Destroy(gameObject);
            return;
        }
    }

}
