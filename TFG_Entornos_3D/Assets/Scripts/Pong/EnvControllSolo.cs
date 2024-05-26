using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnvControllSolo : MonoBehaviour
{
    public GameObject ball;

    public GameObject purpleAgent;
    public GameObject blueAgent;

    public int maxSteps;
    private int step = 0;

    private FootBallAgent pAgent;
    private FootBallAgent bAgent;
    // Start is called before the first frame update
    void Start()
    {
        pAgent = purpleAgent.GetComponent<FootBallAgent>();
        bAgent = blueAgent.GetComponent<FootBallAgent>();
    }

    void FixedUpdate()
    {
        step += 1;
        if (step > maxSteps)
        {
            ResetScene();
        }
    }

    private void ResetScene()
    {
        pAgent.EndEpisode();
        bAgent.EndEpisode();
        ball.GetComponent<BallSolo>().ResetBall();
        step = 0;
    }

    public void ManageReward(string goalColor)
    {
        if (goalColor == "blue")
        {
            pAgent.SetReward(1f);
            bAgent.SetReward(-1f);
        }
        else
        {
            pAgent.SetReward(-1f);
            bAgent.SetReward(1f);
        }
        ResetScene();
    }
}
