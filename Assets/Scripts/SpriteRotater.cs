using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotater : MonoBehaviour
{

    public PlayerController pc;
    void Start()
    {
        pc = GetComponentInParent<PlayerController>();
    }
    void Update()
    {
        rotateToMouse();
    }

    private void rotateToMouse()
    {
        Vector2 md = pc.mouseDir;
        transform.up = new Vector2(md.x, md.y);
    }
}
