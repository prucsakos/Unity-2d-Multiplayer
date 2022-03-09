using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject PlayerObject;
    public GameObject BulletPrefab;
    public Vector3 mouseDir;
    
    private BoxCollider2D boxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;
    
    void Start()
    {
        boxCollider = PlayerObject.AddComponent<BoxCollider2D>();

    }

    
    private void FixedUpdate()
    {
        
    }

    internal void UpdateDirection(Vector3 dir)
    {
        mouseDir = dir;
    }

    internal void OnFired()
    {
        return;
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
