using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor4 : MonoBehaviour
{
    public float distance;
    private Vector3 direction;
    // Update is called once per frame
    void FixedUpdate()
    {
        direction = transform.right - transform.up;
        direction.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100);
        if (hit.collider != null)
        {
            //Debug.Log(hit.collider);
            distance = hit.distance;
            //Debug.Log(distance);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction * distance);
    }
}
