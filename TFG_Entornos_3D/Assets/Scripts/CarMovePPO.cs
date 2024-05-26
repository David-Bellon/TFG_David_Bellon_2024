using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.Linq;

[System.Serializable]
public class Observation
{
    public float[] values;
    public int obs_number;
    public float reward;
    public float verticalInput;
    public float horizontalInput;
    public float vert_log_Input;
    public float hori_log_Input;

    public Observation(float[] values, int obs_number, float reward, float verticalInput, float horizontalInput, float vert_log_Input, float hori_log_Input)
    {
        this.values = values;
        this.obs_number = obs_number;
        this.reward = reward;
        this.verticalInput = verticalInput;
        this.horizontalInput = horizontalInput;
        this.vert_log_Input = vert_log_Input;
        this.hori_log_Input = hori_log_Input;
    }
}

// TO DETECT IF IN COLLISION CREATE A STATE VARIABLE AND IF INSIDE AFTER THE MOVE THE REWARD IS NEGATIVE

public class CarMovePPO : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    public GameObject sensor1;
    public GameObject sensor2;
    public GameObject sensor3;
    public GameObject sensor4;
    public GameObject sensor5;
    public GameObject sensor6;
    public GameObject sensor7;
    public GameObject sensor8;
    public GameObject high_sensor;

    public bool isDead = false;
    public float timeAlive;
    public float distance;
    private float maxTimeAlive = 20;

    private float start_delay;
    public GameObject target;

    private Rigidbody rb;
    public Model model;
    private IWorker worker;

    public List<Observation> observations = new List<Observation>();
    private int obs_number = 0;

    public float reward = 1f;

    private float verticalInput;
    private float horizontalInput;
    private bool newMove;
    private bool hasMoveDisFrame;

    private void Start()
    {
        newMove = true;
        start_delay = 0;
        timeAlive = 0;
        rb = GetComponent<Rigidbody>();
        distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        worker = WorkerFactory.CreateWorker(model, WorkerFactory.Device.GPU);
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            start_delay += Time.deltaTime;
            if (start_delay > 1f)
            {
                timeAlive = timeAlive + Time.deltaTime;

                Tensor input = new Tensor(1, 1, 1, 12);
                input[0] = transform.position.x;
                input[1] = transform.position.y;
                input[2] = target.transform.position.x;
                input[3] = target.transform.position.y;
                input[4] = sensor1.GetComponent<Sensor>().distance;
                input[5] = sensor2.GetComponent<Sensor>().distance;
                input[6] = sensor3.GetComponent<Sensor>().distance;
                input[7] = sensor4.GetComponent<Sensor>().distance;
                input[8] = sensor5.GetComponent<Sensor>().distance;
                input[9] = sensor6.GetComponent<Sensor>().distance;
                input[10] = sensor7.GetComponent<Sensor>().distance;
                input[11] = sensor8.GetComponent<Sensor>().distance;
                worker.Execute(input);
                Tensor output = worker.PeekOutput("value.3");
                Tensor log_prob = worker.PeekOutput("76");
                if (newMove)
                {
                    verticalInput = output[0];
                    horizontalInput = output[1];
                    newMove = false;
                    hasMoveDisFrame = true;
                    StartCoroutine(SkipFrames());
                }
                else
                {
                    hasMoveDisFrame = false;
                }
                // Get user input for movement
                //float horizontalInput = Input.GetAxis("Horizontal");
                //float verticalInput = Input.GetAxis("Vertical");
                // Move the car forward and backward
                MoveCar(verticalInput);

                if (verticalInput != 0)
                {
                    // Rotate the car left and right
                    if (verticalInput < 0)
                    {
                        RotateCar(-horizontalInput);
                    }
                    else
                    {
                        RotateCar(horizontalInput);
                    }

                }
                float currentDistance = Vector3.Distance(gameObject.transform.position, target.transform.position);
                if (currentDistance < distance)
                {
                    reward = 1f;
                }
                else
                {
                    reward = -2f;
                }
                distance = currentDistance;
                if (hasMoveDisFrame)
                {
                    observations.Add(new Observation(input.ToReadOnlyArray(), obs_number, reward, verticalInput, horizontalInput, log_prob[0], log_prob[1]));
                    obs_number += 1;
                }
                input.Dispose();
                output.Dispose();
                log_prob.Dispose();
            }
            if (timeAlive > maxTimeAlive)
            {
                isDead = true;
                worker.Dispose();
            }
        }
    }

    void MoveCar(float input)
    {
        // Calculate the movement vector
        Vector3 movement = -transform.right * input * speed * Time.deltaTime;

        // Apply the movement to the car's Rigidbody
        rb.MovePosition(rb.position + movement);
    }

    void RotateCar(float input)
    {
        // Calculate the rotation amount
        float rotationAmount = input * rotationSpeed * Time.deltaTime;

        // Create a rotation quaternion
        Quaternion rotation = Quaternion.Euler(0f, rotationAmount, 0f);

        // Apply rotation to the car's Rigidbody
        rb.MoveRotation(rb.rotation * rotation);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            observations.Last().reward = -20f;
            //isDead = true;
            //worker.Dispose();
        }
    }

    void OnCollisionExit(Collision col)
    {
        reward = 1f;
    }

    IEnumerator SkipFrames()
    {
        yield return new WaitForSeconds(.5f);
        newMove = true;
    }
}
