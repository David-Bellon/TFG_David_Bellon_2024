using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class CarMove_Scenario2 : MonoBehaviour
{
    public float speed;
    public float rotation_factor;
    public GameObject sensor1;
    public GameObject sensor2;
    public GameObject sensor3;
    public GameObject sensor4;
    public GameObject sensor5;
    public GameObject sensor2_Rever;
    public GameObject sensor3_Rever;
    public GameObject sensor4_Rever;

    public bool isDead = false;
    public float score;

    private float start_delay;

    public Model model;
    private IWorker worker;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        start_delay = 0;
        worker = WorkerFactory.CreateWorker(model, WorkerFactory.Device.GPU);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            start_delay += Time.deltaTime;
            if (start_delay > 1f)
            {
                score = score + Time.deltaTime;
                //float y = Input.GetAxis("Horizontal");
                Tensor input = new Tensor(1, 1, 1, 16);
                input[0] = sensor1.GetComponent<Sensor1>().distance;
                input[1] = sensor2.GetComponent<Sensor2>().distance;
                input[2] = sensor3.GetComponent<Sensor3>().distance;
                input[3] = sensor4.GetComponent<Sensor4>().distance;
                input[4] = sensor5.GetComponent<Sensor5>().distance;
                input[5] = sensor1.GetComponent<Sensor1>().distance;
                input[6] = sensor5.GetComponent<Sensor5>().distance;
                input[7] = sensor1.GetComponent<Sensor1>().distance;
                input[8] = sensor5.GetComponent<Sensor5>().distance;
                input[9] = sensor1.GetComponent<Sensor1>().distance;
                input[10] = sensor5.GetComponent<Sensor5>().distance;
                input[11] = sensor1.GetComponent<Sensor1>().distance;
                input[12] = sensor5.GetComponent<Sensor5>().distance;
                input[13] = sensor2_Rever.GetComponent<Sensor2_Reversed>().distance;
                input[14] = sensor3_Rever.GetComponent<Sensor3_Reversed>().distance;
                input[15] = sensor4_Rever.GetComponent<Sensor4_Reversed>().distance;
                worker.Execute(input);
                Tensor output = worker.PeekOutput();
                float y = output[0];
                //float y = Random.Range(-1f, 1f);
                transform.Rotate(new Vector3(0f, 0f, -rotation_factor * y) * Time.deltaTime);
                Vector2 movement = new Vector2(1, 0);
                transform.Translate(movement * speed * Time.deltaTime);
                input.Dispose();
                output.Dispose();
            }

        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isDead = true;
        worker.Dispose();
    }
}
