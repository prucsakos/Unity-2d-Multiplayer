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
    void Update()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if(x != 0f || y != 0f)
        {
            pc.move(x, y);
        }
    }
}
