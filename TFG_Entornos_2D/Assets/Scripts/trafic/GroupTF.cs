using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTF : MonoBehaviour
{
    public SpriteRenderer[] group_1;
    public SpriteRenderer[] group_2;

    public float greenTimeGroup1;
    public float redTimeGroup1;
    private bool change;
    // Start is called before the first frame update
    void Start()
    {
        change = true;
        if (Random.Range(0, 2) == 0)
        {
            for (int i = 0; i < group_1.Length;  i++)
            {
                group_1[i].color = Color.green;
                group_2[i].color = Color.red;
            }
        }
        else
        {
            for (int i = 0; i < group_1.Length; i++)
            {
                group_1[i].color = Color.red;
                group_2[i].color = Color.green;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (change)
        {
            StartCoroutine(ChangeColor());
        }
    }

    IEnumerator ChangeColor()
    {
        change = false;
        float timePassed = 0;
        if (group_1[0].color == Color.green)
        {
            while (timePassed < greenTimeGroup1)
            {
                yield return new WaitForSeconds(1f);
                timePassed++;
            }
            for (int i = 0; i < group_1.Length; i++)
            {
                group_1[i].color = Color.red;
            }
            yield return new WaitForSeconds(2f);
            group_2[0].color = Color.green;
            group_2[1].color = Color.green;
        }
        else
        {
            while (timePassed < redTimeGroup1)
            {
                yield return new WaitForSeconds(1f);
                timePassed++;
            }
            for (int i = 0; i < group_1.Length; i++)
            {
                group_2[i].color = Color.red;
            }
            yield return new WaitForSeconds(2f);
            group_1[0].color = Color.green;
            group_1[1].color = Color.green;
        }
        change = true;
    }
}
