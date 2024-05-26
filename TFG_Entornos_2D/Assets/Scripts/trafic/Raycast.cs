using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    private FollowLines car;
    public float distanceHit;
    public float targetDistanceHit;
    public LayerMask ignoreLayer;
    public Transform direction;
    private float normalSpeed;
    // Start is called before the first frame update
    void Start()
    {
        car = gameObject.GetComponent<FollowLines>();
        normalSpeed = car.speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(direction.position, -direction.right, 5f, ~ignoreLayer);
        if (hit.collider != null)
        {
            distanceHit = hit.distance;
            switch (hit.collider.gameObject.tag)
            {
                case "sem":
                    if (distanceHit < targetDistanceHit)
                    {
                        if (hit.collider.transform.parent != transform.parent.root)
                        {
                            if (hit.collider.gameObject.GetComponent<SpriteRenderer>().color == Color.red && hit.collider.transform.parent.GetComponent<LineRenderer>() == car.lr)
                            {
                                car.move = false;
                            }
                            else
                            {
                                car.move = true;
                            }
                        }
                        else
                        {
                            if (hit.collider.gameObject.GetComponent<SpriteRenderer>().color == Color.red)
                            {
                                car.move = false;
                            }
                            else
                            {
                                car.move = true;
                            }
                        }
                    }
                    else
                    {
                        car.move = true;
                        car.speed = normalSpeed;
                    }
                    break;
                case "car":
                    if (distanceHit < targetDistanceHit)
                    {
                        car.move = false;
                        /*
                        if (hit.collider.gameObject.GetComponent<FollowLines>().lr == gameObject.GetComponent<FollowLines>().lr)
                        {
                            car.move = false;
                        }
                        else
                        {
                            car.move = true;
                        }
                        */
                    }
                    else
                    {
                        car.move = true;
                        car.speed = Mathf.Clamp(distanceHit, 0, normalSpeed);
                    }
                    break;
            }
        }
        else
        {
            distanceHit = 2f;
            car.move = true;
            car.speed = normalSpeed;
        }
        //Debug.DrawRay(direction.position, -direction.right * distanceHit, Color.green);
    }
}
