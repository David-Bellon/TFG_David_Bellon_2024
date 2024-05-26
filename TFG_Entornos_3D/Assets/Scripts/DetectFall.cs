using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectFall : MonoBehaviour
{
    public bool touchingGround = false;

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            touchingGround = true;
        }
    }

    public void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            touchingGround = false;
        }
    }
}
