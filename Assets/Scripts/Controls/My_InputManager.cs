using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My_InputManager : MonoBehaviour
{
    public PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckMovement();
        CheckFire();
        GetMouseInput();
    }

    private void GetMouseInput()
    {
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        pc.UpdateDirection(dir);
    }

    private void CheckFire()
    {
        if (Input.GetButton("Fire1")){
            pc.OnFired();
        }
    }

    private void CheckMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if(x != 0f || y != 0f)
        {
            pc.OnMove(x, y);
        }
    }
}
