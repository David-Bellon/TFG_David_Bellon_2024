using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float distance;
    private Vector3 direction;
    public LayerMask IgnoreLayer;
    // Update is called once per frame
    void FixedUpdate()
    {
        direction = -transform.up;
        direction.Normalize();
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, ~IgnoreLayer))
        {
            //Debug.Log(hit.collider);
            distance = hit.distance;
            //Debug.Log(distance);
        }
        else
        {
            distance = 10;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction * distance);
    }
}
