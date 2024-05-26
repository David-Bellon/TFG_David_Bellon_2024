using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

public class Walk : Agent
{
    public GameObject hip;
    public GameObject head;
    public GameObject spine;
    public GameObject chest;
    public GameObject l_arm;
    public GameObject l_hand;
    public GameObject r_arm;
    public GameObject r_hand;
    public GameObject l_hip;
    public GameObject l_leg;
    public GameObject l_foot;
    public GameObject r_hip;
    public GameObject r_leg;
    public GameObject r_foot;
    public GameObject floor;

    public GameObject target;

    public bool fall;

    private List<GameObject> parts;
    private List<Vector3> partsRotation = new List<Vector3>();
    private List<Vector3> partsPosition = new List<Vector3>();

    public float angle;

    public float distance;

    public float maxPosSpring = 10000;
    public float maxPosDamper = 100;
    public float maxForce = 1000f;

    [Range(0.1f, 10)]
    public float targetSpeed = 5;

    public Vector3 avgVel;

    private bool firstReach;

    public TMP_Text clockText;
    private bool startInLeft;
    // Start is called before the first frame update
    void Start()
    {
        parts = new List<GameObject> { hip, head, spine, chest, l_arm, l_hand, r_arm, r_hand, l_hip, l_leg, l_foot, r_hip, r_leg, r_foot };
        angle = Vector3.Angle(hip.transform.right, floor.transform.up);
        foreach (var part in parts)
        {
            partsPosition.Add(part.transform.position);
            partsRotation.Add(part.transform.eulerAngles);
        }
        clockText.text = "457";
    }

    void Update()
    {
        //Debug.DrawRay(head.transform.position, head.transform.forward, Color.green);
    }

    void FixedUpdate()
    {
        var headDir = head.transform.forward;
        headDir.y = 0;
        float lookingTarget = Vector3.Dot((target.transform.position - hip.transform.position).normalized, headDir);
        lookingTarget = (lookingTarget + 1f) * 0.5f;
        AddReward(lookingTarget * MatchTargetVelocity());
    }

    void SpawnTarget()
    {
        target.GetComponent<TargetReach>().reached = false;
        startInLeft = true;
        if (firstReach)
        {
            if (startInLeft)
            {
                // Left to Right
                target.transform.localPosition = new Vector3(Random.Range(-10f, 10f), -1f, 0f);
                //target.transform.localPosition = new Vector3(0f, -1f, 0f);
            }
            else
            {
                // Right to Left
                target.transform.localPosition = new Vector3(Random.Range(-10f, 10f), -1f, 42f);
                //target.transform.localPosition = new Vector3(0f, -1f, 42f);
            }
        }
        else
        {
            if (startInLeft)
            {
                // Left to Right
                target.transform.localPosition = new Vector3(Random.Range(-10f, 10f), -1f, 42f);
                hip.transform.localPosition = new Vector3(0f, -0.14f, 0f);
                //target.transform.localPosition = new Vector3(0f, -1f, 42f);
            }
            else
            {
                // Right to Left
                target.transform.localPosition = new Vector3(Random.Range(-10f, 10f), -1f, 0f);
                hip.transform.localPosition = new Vector3(0f, -0.14f, 41.5f);
                //target.transform.localPosition = new Vector3(0f, -1f, 0f);
            }
        }
        distance = GetDistance();
    }

    public override void OnEpisodeBegin()
    {
        if (Random.Range(0, 2) == 0)
        {
            startInLeft = true;
        }
        else
        {
            startInLeft = false;
        }
        firstReach = false;
        fall = false;
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].transform.position = partsPosition[i];
            parts[i].transform.eulerAngles = partsRotation[i];
            Rigidbody r = parts[i].GetComponent<Rigidbody>();
            r.velocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            if (parts[i] != hip)
            {
                ConfigurableJoint joint = parts[i].GetComponent<ConfigurableJoint>();
                joint.targetRotation = Quaternion.Euler(0, 0, 0);
                var jd = new JointDrive
                {
                    positionSpring = maxPosSpring,
                    positionDamper = maxPosDamper,
                    maximumForce = maxForce
                };
                joint.slerpDrive = jd;
            }
        }
        hip.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 90f);
        SpawnTarget();
        angle = Vector3.Angle(hip.transform.right, floor.transform.up);
        clockText.text = (int.Parse(clockText.text) + 1).ToString();
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        var trgVel = (target.transform.position - hip.transform.position).normalized * targetSpeed;
        var avgVel = GetAvgVel();

        sensor.AddObservation(Vector3.Distance(trgVel, avgVel));
        angle = Vector3.Angle(hip.transform.right, floor.transform.up);
        sensor.AddObservation(distance);
        sensor.AddObservation(avgVel);
        sensor.AddObservation(angle);
        foreach (var part in parts)
        {
            if (part != hip)
            {
                sensor.AddObservation(part.GetComponent<DetectFall>().touchingGround);
                sensor.AddObservation(part.transform.rotation);
                sensor.AddObservation(part.transform.localPosition);
                float currentStrenght = part.GetComponent<ConfigurableJoint>().slerpDrive.maximumForce;
                sensor.AddObservation(currentStrenght/maxForce);
            }
        }
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(spine.transform.localPosition);
    }

    private Vector3 GetAvgVel()
    {
        int numberParts = 0;
        Vector3 vel = Vector3.zero;
        foreach (var part in parts)
        {
            vel += part.GetComponent<Rigidbody>().velocity;
            numberParts++;
        }
        avgVel = vel / numberParts;

        return vel / numberParts;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int i = -1;
        RotateJoin(head, actions.ContinuousActions[++i], actions.ContinuousActions[++i], 0);
        RotateJoin(chest, actions.ContinuousActions[++i], actions.ContinuousActions[++i], actions.ContinuousActions[++i]);
        RotateJoin(spine, actions.ContinuousActions[++i], actions.ContinuousActions[++i], actions.ContinuousActions[++i]);
        RotateJoin(r_arm, actions.ContinuousActions[++i], actions.ContinuousActions[++i], 0);
        RotateJoin(r_hand, actions.ContinuousActions[++i], 0, 0);
        RotateJoin(l_arm, actions.ContinuousActions[++i], actions.ContinuousActions[++i], 0);
        RotateJoin(l_hand, actions.ContinuousActions[++i], 0, 0);
        RotateJoin(r_hip, actions.ContinuousActions[++i], actions.ContinuousActions[++i], 0);
        RotateJoin(r_leg, actions.ContinuousActions[++i], 0, 0);
        RotateJoin(r_foot, actions.ContinuousActions[++i], actions.ContinuousActions[++i], actions.ContinuousActions[++i]);
        RotateJoin(l_hip, actions.ContinuousActions[++i], actions.ContinuousActions[++i], 0);
        RotateJoin(l_leg, actions.ContinuousActions[++i], 0, 0);
        RotateJoin(l_foot, actions.ContinuousActions[++i], actions.ContinuousActions[++i], actions.ContinuousActions[++i]);

        ApplyForcePart(head, actions.ContinuousActions[++i]);
        ApplyForcePart(chest, actions.ContinuousActions[++i]);
        ApplyForcePart(spine, actions.ContinuousActions[++i]);
        ApplyForcePart(r_arm, actions.ContinuousActions[++i]);
        ApplyForcePart(r_hand, actions.ContinuousActions[++i]);
        ApplyForcePart(l_arm, actions.ContinuousActions[++i]);
        ApplyForcePart(l_hand, actions.ContinuousActions[++i]);
        ApplyForcePart(r_hip, actions.ContinuousActions[++i]);
        ApplyForcePart(r_leg, actions.ContinuousActions[++i]);
        ApplyForcePart(r_foot, actions.ContinuousActions[++i]);
        ApplyForcePart(l_hip, actions.ContinuousActions[++i]);
        ApplyForcePart(l_leg, actions.ContinuousActions[++i]);
        ApplyForcePart(l_foot, actions.ContinuousActions[++i]);
        angle = Vector3.Angle(hip.transform.right, floor.transform.up);
        SetRewards();
    }

    private void RotateJoin(GameObject part, float x_force, float y_force, float z_force)
    {
        ConfigurableJoint joint = part.GetComponent<ConfigurableJoint>();
        x_force = (x_force + 1) * 0.5f;
        y_force = (y_force + 1) * 0.5f;
        z_force = (z_force + 1) * 0.5f;

        var x_rot = Mathf.Lerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, x_force);
        var y_rot = Mathf.Lerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, y_force);
        var z_rot = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z_force);

        joint.targetRotation = Quaternion.Euler(x_rot, y_rot, z_rot);
    }

    private void ApplyForcePart(GameObject part, float force)
    {
        ConfigurableJoint joint = part.GetComponent<ConfigurableJoint>();
        force = (force + 1) * 0.5f * maxForce;
        var jd = new JointDrive
        {
            positionSpring = maxPosSpring,
            positionDamper = maxPosDamper,
            maximumForce = force
        };
        joint.slerpDrive = jd;
    }

    private float GetDistance()
    {
        Vector3 actorPosition = hip.transform.localPosition;
        Vector3 targetPosition = target.transform.localPosition;
        actorPosition.y = 0;
        targetPosition.y = 0;
        return Vector3.Distance(actorPosition, targetPosition);
    }

    private float MatchTargetVelocity()
    {
        var trgVel = (target.transform.position - hip.transform.position).normalized * targetSpeed;
        var distanceVel = Mathf.Clamp(Vector3.Distance(GetAvgVel(), trgVel), 0, targetSpeed);

        return Mathf.Pow(1 - Mathf.Pow(distanceVel / targetSpeed, 2), 2);
    }

    public void SetRewards()
    {
        // Rewards Base if the agent falls
        foreach(var part in parts)
        {
            if (part != l_foot && part != r_foot)
            {
                fall = part.GetComponent<DetectFall>().touchingGround;
            }
            if (fall)
            {
                break;
            }
        }
        if (fall)
        {
            AddReward(-10f);
            EndEpisode();
        }
        /*
        else
        {
            AddReward(0.5f);
        }
        */
        // Rewards Base if the chest of the player is straight
        /*
        if (angle > 26f)
        {
            AddReward(-1f);
        }
        else
        {
            AddReward(0.3f);
        }
        */
        // Rewards if the agent is closer to the target
        if (target.GetComponent<TargetReach>().reached)
        {
            AddReward(10f);
            firstReach = !firstReach;
            SpawnTarget();
        }
        /*
        else
        {
            float currentDistance = GetDistance();
            if (currentDistance >= distance)
            {
                AddReward(-1f);
            }
            else
            {
                AddReward(1f / currentDistance);
            }
            distance = currentDistance;
        }
        */
        distance = GetDistance();
    }
}
