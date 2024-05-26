using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ML_Agent : Agent
{
    [System.Serializable]
    public class AxelInfo
    {
        public string name;
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;

        public bool isTraction;
        public bool isSteering;
    }


    public List<AxelInfo> axels = new List<AxelInfo>();
    public float maxTorque;
    public float maxDegreeSteer;

    public float speed = 10f;
    public float rotationSpeed = 100f;

    public float distanceToTarget;

    public GameObject sensor1;
    public GameObject sensor2;
    public GameObject sensor3;
    public GameObject sensor4;
    public GameObject sensor5;
    public GameObject sensor6;
    public GameObject sensor7;
    public GameObject sensor8;
    public Transform target;

    public GameObject parkingFloor;
    public bool insideParking;

    public GameObject checkpoint;

    private Rigidbody rb;

    public GameObject spawnBox;

    public bool targetReach;

    private void Start()
    {
        targetReach = false;
        rb = GetComponent<Rigidbody>();
        distanceToTarget = GetDistance();
    }

    void FixedUpdate()
    {
        //Debug.Log(StepCount);
        //AddReward(-0.001f);
    }

    public override void OnEpisodeBegin()
    {
        targetReach = false;
        insideParking = false;
        rb.velocity = new Vector3(0, 0, 0);
        checkpoint.GetComponent<Collider>().enabled = true;
        foreach (AxelInfo axel in axels)
        {
            if (axel.isSteering)
            {
                axel.leftWheel.steerAngle = 0;
                axel.rightWheel.steerAngle = 0;
            }

            if (axel.isTraction)
            {
                axel.leftWheel.motorTorque = 0;
                axel.rightWheel.motorTorque = 0;
            }
        }
        RandomSpawn();
        distanceToTarget = GetDistance();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(sensor1.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor2.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor3.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor4.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor5.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor6.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor7.GetComponent<Sensor>().distance);
        sensor.AddObservation(sensor8.GetComponent<Sensor>().distance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float motor = maxTorque * moveX;
        float steering = maxDegreeSteer * moveY;

        foreach (AxelInfo axel in axels)
        {
            if (axel.isSteering)
            {
                axel.leftWheel.steerAngle = steering;
                axel.rightWheel.steerAngle = steering;
            }

            if (axel.isTraction)
            {
                axel.leftWheel.motorTorque = motor;
                axel.rightWheel.motorTorque = motor;
                //Debug.Log(axel.leftWheel.rpm);
            }
            //ApplyVisualChangeWheels(axel.leftWheel);
            //ApplyVisualChangeWheels(axel.rightWheel);
        }
        /*
        Vector3 movement = -transform.right * moveX * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        if (moveX != 0)
        {
            float rotationAmount = moveY * rotationSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(0f, rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * rotation);
        }
        */
        SetRewards();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }

    public void ApplyVisualChangeWheels(WheelCollider collider)
    {
        Vector3 positon;
        Quaternion rotation;

        Transform wheel = transform.Find("Wheels/" + collider.gameObject.name);

        collider.GetWorldPose(out positon, out rotation);
        rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, -90f);
        //wheel.transform.position = positon;
        wheel.transform.rotation = rotation;
    }

    private void SetRewards()
    {
        float currentDistance = GetDistance();
        AddReward(Mathf.Clamp(1f / currentDistance, 0, 1));
        /*
        if (currentDistance >= distanceToTarget)
        {
            //AddReward(-1f);
        }
        else
        {
            AddReward(1f / currentDistance);
        }
        if (!insideParking)
        {
            //AddReward(-1f);
        }
        */
        distanceToTarget = currentDistance;
    }

    private void RandomSpawn()
    {
        Bounds bounds = spawnBox.GetComponent<Collider>().bounds;
        float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
        float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
        float randomRotation = Random.Range(0f, 360f);

        transform.position = bounds.center + new Vector3(offsetX, -0.4f, offsetZ);
        transform.rotation = Quaternion.Euler(0, randomRotation, 0);
    }

    private float GetDistance()
    {
        Vector3 targetPos = target.localPosition;
        Vector3 agentPos = gameObject.transform.localPosition;
        targetPos.y = 0f;
        agentPos.y = 0f;

        return Vector3.Distance(agentPos, targetPos);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            AddReward(-1f);
            //EndEpisode();
        }

        if (col.gameObject == parkingFloor)
        {
            insideParking = true;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            AddReward(-1f);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject == parkingFloor)
        {
            insideParking = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        
        if (col.gameObject.tag == "Target")
        {
            AddReward(10f);
            targetReach = true;
        }
        else
        {
            if (col.gameObject.tag == "Checkpoint")
            {
                col.enabled = false;
                AddReward(3f);
            }
            else
            {
                AddReward(-1f); // Best Value = -1
                //EndEpisode();
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Target")
        {
            float angle = Vector3.Angle(-transform.right, -target.transform.right);
            AddReward(1f / (angle + 1f));
        }
        else
        {
            AddReward(-1f);
        }
    }
}
