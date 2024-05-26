using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnvControllerTeams : MonoBehaviour
{
    public GameObject ball;

    public List<GameObject> blueTeamList = new List<GameObject>();
    public List<GameObject> purpleTeamList = new List<GameObject>();

    public int maxSteps;
    private int step = 0;

    private SimpleMultiAgentGroup blueTean;
    private SimpleMultiAgentGroup purpleTeam;
    // Start is called before the first frame update
    void Start()
    {
        blueTean = new SimpleMultiAgentGroup();
        purpleTeam = new SimpleMultiAgentGroup();

        foreach (GameObject agent in blueTeamList)
        {
            blueTean.RegisterAgent(agent.GetComponent<FootBallAgent>());
        }

        foreach (GameObject agent in purpleTeamList)
        {
            purpleTeam.RegisterAgent(agent.GetComponent<FootBallAgent>());
        }
    }

    void FixedUpdate()
    {
        step += 1;
        if (step > maxSteps)
        {
            purpleTeam.GroupEpisodeInterrupted();
            blueTean.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    private void ResetScene()
    {
        ball.GetComponent<BallTeams>().ResetBall();
        step = 0;
    }

    public void ManageReward(string goalColor)
    {
        if (goalColor == "blue")
        {
            purpleTeam.AddGroupReward(1 - (float)(step / maxSteps));
            blueTean.AddGroupReward(-1f);
        }
        else
        {
            purpleTeam.AddGroupReward(-1f);
            blueTean.AddGroupReward(1 - (float)(step / maxSteps));
        }
        purpleTeam.EndGroupEpisode();
        blueTean.EndGroupEpisode();
        ResetScene();
    }
}
