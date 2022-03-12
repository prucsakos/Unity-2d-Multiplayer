using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookat;
    public float boundX = 0.15f;
    public float boundY = 0.05f;

    public void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        float deltaX = lookat.position.x - transform.position.x;
        if( Mathf.Abs(deltaX) > boundX)
        {
            if(deltaX > 0)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }

        float deltaY = lookat.position.y - transform.position.y;
        if (Mathf.Abs(deltaY) > boundY)
        {
            if (deltaY > 0)
            {
                delta.y = deltaY - boundY;
            }
            else
            {
                delta.y = deltaY + boundY;
            }
        }

        transform.position += delta;
    }
}
