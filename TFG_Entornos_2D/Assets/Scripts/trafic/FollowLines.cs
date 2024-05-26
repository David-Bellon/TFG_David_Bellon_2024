using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLines : MonoBehaviour
{

    public LineRenderer lr;
    private int numberPoints;
    public int targetPoint;
    public float distance;
    public float speed;

    public bool move;

    public int timeAlive;
    private bool addSeconds;

    // Start is called before the first frame update
    void Start()
    {
        addSeconds = true;
        timeAlive = 0;
        move = true;
        numberPoints = lr.positionCount;
        targetPoint = 1;
        distance = Vector2.Distance(transform.position, lr.GetPosition(targetPoint) + lr.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (addSeconds)
        {
            StartCoroutine(CountSeconds());
        }

        if (move)
        {
            
            transform.position = Vector2.MoveTowards(transform.position, lr.GetPosition(targetPoint) + lr.transform.position, speed * Time.deltaTime);
            distance = Vector2.Distance(transform.position, lr.GetPosition(targetPoint) + lr.transform.position);
            if (distance < 0.02f)
            {
                targetPoint++;
            }
        }
        transform.right = transform.position - (lr.GetPosition(targetPoint) + lr.transform.position);
    }

    IEnumerator CountSeconds()
    {
        addSeconds = false;
        yield return new WaitForSeconds(1f);
        timeAlive = timeAlive + 1;
        addSeconds = true;
    }
}
