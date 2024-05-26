using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FootBallAgent : Agent
{
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Transform ball;
    public Transform enemyAgent;

    public Transform blueGoal;
    public Transform purpleGoal;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.velocity = new Vector3(0, 0, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(enemyAgent.localPosition);
        //sensor.AddObservation(ball.localPosition);
        sensor.AddObservation(transform.forward.z * ball.GetComponent<Rigidbody>().velocity);
        sensor.AddObservation(transform.forward.z * rb.velocity);
        //sensor.AddObservation(transform.forward.z * enemyAgent.GetComponent<Rigidbody>().velocity);
        //sensor.AddObservation(blueGoal.localPosition);
        //sensor.AddObservation(purpleGoal.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveVertical = actions.ContinuousActions[0];
        float moveHorizontal = actions.ContinuousActions[1];

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = Vector3.ClampMagnitude(movement, 1f);

        // Apply movement to the paddle's rigidbody
        rb.velocity = movement * speed * transform.forward.z;

        //AddReward(-0.0003f);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == ball.gameObject)
        {
            //AddReward(0.1f);
        }
    }
}
