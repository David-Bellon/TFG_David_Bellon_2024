using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GetEnvData : MonoBehaviour
{
    public List<GameObject> semaphores;
    public List<GameObject> cars;

    public Transform env;

    public int reward;

    public int carsStopedLine1;
    public int carsStopedLine2;
    public int carsStopedLine3;
    public int carsStopedLine4;
    public int carsStopedLine5;
    public int carsStopedLine6;
    public int carsStopedLine7;
    public int carsStopedLine8;

    // Start is called before the first frame update
    void Start()
    {
        reward = 0;
        semaphores = FindObjectWithTagInGameObject(env, "sem");
    }

    // Update is called once per frame
    void Update()
    {
        cars = FindObjectWithTagInGameObject(env, "car");
        int line1 = 0;
        int line2 = 0;
        int line3 = 0;
        int line4 = 0;
        int line5 = 0;
        int line6 = 0;
        int line7 = 0;
        int line8 = 0;
        reward = 0;
        foreach (GameObject car in cars)
        {
            int lineNumber = -1;
            try
            {
                lineNumber = int.TryParse(car.GetComponent<FollowLines>().lr.gameObject.name.Split("_")[1], out lineNumber) ? lineNumber : -1;
            }
            catch
            {
                lineNumber = -1;
            }
            switch (lineNumber)
            {
                case 1:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line1 += 1;
                        reward -= 1;
                    }
                    break;
                case 2:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line2 += 1;
                        reward -= 1;
                    }
                    break;
                case 3:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line3 += 1;
                        reward -= 1;
                    }
                    break;
                case 4:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line4 += 1;
                        reward -= 1;
                    }
                    break;
                case 5:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line5 += 1;
                        reward -= 1;
                    }
                    break;
                case 6:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line6 += 1;
                        reward -= 1;
                    }
                    break;
                case 7:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line7 += 1;
                        reward -= 1;
                    }
                    break;
                case 8:
                    if (!car.GetComponent<FollowLines>().move)
                    {
                        line8 += 1;
                        reward -= 1;
                    }
                    break;
                default:
                    break;
            }
        }
        carsStopedLine1 = line1;
        carsStopedLine2 = line2;
        carsStopedLine3 = line3;
        carsStopedLine4 = line4;
        carsStopedLine5 = line5;
        carsStopedLine6 = line6;
        carsStopedLine7 = line7;
        carsStopedLine8 = line8;
    }

    private List<GameObject> FindObjectWithTagInGameObject(Transform parent, string tag)
    {
        GameObject[] aux = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> return_data = new List<GameObject>();

        foreach(GameObject data in aux)
        {
            if (data.transform.root == parent)
            {
                return_data.Add(data);
            }
        }
        return return_data;
    }
}
