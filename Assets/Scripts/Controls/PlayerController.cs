using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject PlayerObject;
    public GameObject BulletPrefab;
    public GameObject GunPrefab;
    public Vector3 mouseDir;
    public float lookAngle;
    
    private BoxCollider2D boxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;

    public bool canFire = true;
    
    void Start()
    {
        boxCollider = PlayerObject.AddComponent<BoxCollider2D>();

    }

    
    private void FixedUpdate()
    {
        updateAngle();
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

    internal void OnFired()
    {
        if (canFire)
        {
            GameObject go = Instantiate(BulletPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
            go.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
            BulletMovement bm = go.AddComponent<BulletMovement>();
            bm.velocity = 3f;
            bm.dmg = 20;

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sortingLayerName = "Bullet";

            Destroy(go, 5f);

            canFire = false;
            Invoke("AllowFire", 0.1f);
        }
    }

    private void AllowFire()
    {
        canFire = true;
    }

    public void OnMove(float x = 0f, float y = 0f)
    {
        
        moveDelta = new Vector3(x, y, 0);
        var _transfrom = PlayerObject.transform;

        if (moveDelta.x > 0)
        {
            _transfrom.localScale = Vector3.one;
        }
        else if (moveDelta.x < 0)
        {
            _transfrom.localScale = new Vector3(-1, 1, 1);
        }


        hit = Physics2D.BoxCast(_transfrom.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            transform.Translate(moveDelta.x * Time.deltaTime, 0f, 0f);
        }
        hit = Physics2D.BoxCast(_transfrom.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            transform.Translate(0f, moveDelta.y * Time.deltaTime, 0f);
        }
    }
}
