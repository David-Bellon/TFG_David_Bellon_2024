using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReach : MonoBehaviour
{
    public bool reached = false;
    void OnCollisionEnter(Collision col)
    {
        reached = true;
    }
}
