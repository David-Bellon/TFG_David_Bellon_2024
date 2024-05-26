using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageIntersection : MonoBehaviour
{
    public LineRenderer[] roads;
    public float[] changeRoadProb;

    void Start()
    {
        Debug.Assert(roads.Length == changeRoadProb.Length, "Dimensions must match");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        LineRenderer currentLine = col.gameObject.GetComponent<FollowLines>().lr;
        for (int i = 0; i < roads.Length; i++)
        {
            float randomProb = Random.Range(0f, 1f);
            if (randomProb <= changeRoadProb[i])
            {
                col.gameObject.GetComponent<FollowLines>().lr = roads[i];
                col.gameObject.GetComponent<FollowLines>().targetPoint = 0;
                break;
            }
        }
    }
}
