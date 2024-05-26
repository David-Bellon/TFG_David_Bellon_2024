using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SimpleTF : Agent
{
    public int limitTime;
    private SpriteRenderer tf;
    private bool change;

    public GameObject main;
    private GetEnvData envData;

    private int action;
    // Start is called before the first frame update
    void Start()
    {
        envData = main.GetComponent<GetEnvData>();
        tf = gameObject.GetComponent<SpriteRenderer>();
        change = true;
        if (Random.Range(0, 2) == 0)
        {
            tf.color = Color.red;
        }
        else
        {
            tf.color = Color.green;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (change)
        {
            StartCoroutine(ChangeColor());
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (GameObject sem in envData.semaphores)
        {
            if (sem == gameObject)
            {
                sensor.AddObservation(1);
            }
            else
            {
                sensor.AddObservation(0);
            }

            if (sem.GetComponent<SpriteRenderer>().color == Color.red)
            {
                sensor.AddObservation(0);
            }
            else
            {
                sensor.AddObservation(1);
            }

            sensor.AddObservation(envData.carsStopedLine1);
            sensor.AddObservation(envData.carsStopedLine2);
            sensor.AddObservation(envData.carsStopedLine3);
            sensor.AddObservation(envData.carsStopedLine4);
            sensor.AddObservation(envData.carsStopedLine5);
            sensor.AddObservation(envData.carsStopedLine6);
            sensor.AddObservation(envData.carsStopedLine7);
            sensor.AddObservation(envData.carsStopedLine8);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        action = actions.DiscreteActions[0];
    }

    IEnumerator ChangeColor()
    {
        change = false;
        float timePassed = 0;
        if (tf.color == Color.green)
        {
            while (tf.color == Color.green)
            {
                yield return new WaitForSeconds(1f);
                timePassed++;
                if (timePassed > limitTime)
                {
                    if (action == 1)
                    {
                        tf.color = Color.red;
                        break;
                    }
                }
            }
        }
        else
        {
            while (tf.color == Color.red)
            {
                yield return new WaitForSeconds(1f);
                timePassed++;
                if (timePassed > limitTime)
                {
                    if (action == 1)
                    {
                        tf.color = Color.green;
                        break;
                    }
                }
            }
        }
        change = true;
        StartCoroutine(DelayReward());
    }

    IEnumerator DelayReward()
    {
        yield return new WaitForSeconds(4f);
        AddReward(envData.reward);
    }
}
