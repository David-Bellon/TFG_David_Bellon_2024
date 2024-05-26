using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTeams : MonoBehaviour
{
    Rigidbody rb;
    public GameObject enviroment;
    private EnvControllerTeams env;
    private Vector3 ballPosition;
    //private bool coroutineOn = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Random.Range(0, 2) == 0)
        {
            rb.AddForce(0, 0, 1 * -5f, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(0, 0, 1 * 5f, ForceMode.Impulse);
        }
        env = enviroment.GetComponent<EnvControllerTeams>();
        ballPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (transform.position.y >= 4f)
        {
            ResetBall();
        }

        /*
        if (rb.velocity.magnitude < 1 && !coroutineOn)
        {
            coroutineOn = true;
            StartCoroutine(CheckIfStop());
        }
        */
    }

    public void ResetBall()
    {
        transform.position = ballPosition;
        transform.rotation = Quaternion.identity;

        rb.velocity = new Vector3(0, 0, 0);

        if (Random.Range(0, 2) == 0)
        {
            rb.AddForce(0, 0, 1 * -5f, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(0, 0, 1 * 5f, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("blueGoal"))
        {
            env.ManageReward("blue");
        }
        else if (collision.gameObject.CompareTag("purpleGoal"))
        {
            env.ManageReward("purple");
        }
    }

    /*
    IEnumerator CheckIfStop()
    {
        bool reset = true;
        int count = 0;
        while (count < 5)
        {
            yield return new WaitForSeconds(1f);
            count++;
            if (rb.velocity.magnitude >= 1f)
            {
                reset = false;
                break;
            }
        }
        if (reset)
        {
            env.BallStoped();
        }
        coroutineOn = false;
    }
    */
}
