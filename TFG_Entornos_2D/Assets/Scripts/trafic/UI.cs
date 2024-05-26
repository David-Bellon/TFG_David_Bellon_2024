using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public GameObject ui;
    public TMP_Text dataText; 
    private bool isActive;
    private GameObject[] activeCars;
    private GameObject[] lines;
    private bool linesShown;

    private float avgTimeAlive;
    private float getNewAvg;
    // Start is called before the first frame update
    void Start()
    {
        getNewAvg = 0;
        avgTimeAlive = 0f;
        linesShown = true;
        isActive = false;
        dataText.text = "";
        lines = GameObject.FindGameObjectsWithTag("line");
        ShowHideLines();
    }

    // Update is called once per frame
    void Update()
    {
        getNewAvg += Time.deltaTime;
        activeCars = GameObject.FindGameObjectsWithTag("car");
        if (Input.GetKeyDown("space"))
        {
            isActive  = !isActive;
            ui.SetActive(isActive);
        }
        
        if (isActive)
        {
            WriteData();
        }
    }

    private void WriteData()
    {
        string text = "Total Number of Active Cars: " + activeCars.Length.ToString() + "\n";
        text += "Mean time to Reach target: " + avgTimeAlive.ToString() + "s";
        if (getNewAvg > 10f)
        {
            avgTimeAlive = GetAverageTimeAlive();
            getNewAvg = 0;
        }
        dataText.text = text;
    }

    private float GetAverageTimeAlive()
    {
        float total = 0;
        foreach (GameObject car in activeCars)
        {
            total += car.GetComponent<FollowLines>().timeAlive;
        }
        return total / activeCars.Length;
    }

    public void ShowHideLines()
    {
        linesShown = !linesShown;
        foreach (GameObject line in  lines)
        {
            line.GetComponent<LineRenderer>().enabled = linesShown;
        }
    }
}
