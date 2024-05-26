using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class Parking_Move : MonoBehaviour
{
    public float speed;
    public float rotation_factor;
    public GameObject sensor1;
    public GameObject sensor2;
    public GameObject sensor3;
    public GameObject sensor4;
    public GameObject sensor5;

    public bool isDead = false;
    public float timeAlive;
    public float distance;
    private float maxTimeAlive = 20;

    private float start_delay;
    public GameObject target;

    public Model model;
    private IWorker worker;
    // Start is called before the first frame update
    void Start()
    {
        start_delay = 0;
        timeAlive = 0;
        distance = Vector2.Distance(gameObject.transform.position, target.transform.position);

        worker = WorkerFactory.CreateWorker(model, WorkerFactory.Device.GPU);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            distance = Vector2.Distance(gameObject.transform.position, target.transform.position);
            start_delay += Time.deltaTime;
            if (start_delay > 1f)
            {
                timeAlive = timeAlive + Time.deltaTime;
                //float y = Input.GetAxis("Horizontal");
                //float x = Input.GetAxis("Vertical");
                Tensor input = new Tensor(1, 1, 1, 6);
                input[0] = distance;
                input[1] = sensor1.GetComponent<Sensor1>().distance;
                input[2] = sensor2.GetComponent<Sensor2>().distance;
                input[3] = sensor3.GetComponent<Sensor3>().distance;
                input[4] = sensor4.GetComponent<Sensor4>().distance;
                input[5] = sensor5.GetComponent<Sensor5>().distance;
                worker.Execute(input);
                Tensor output = worker.PeekOutput();
                float x = output[0];
                float y = output[1];
                Vector2 movement = new Vector2(x, 0);
                transform.Translate(movement * speed * Time.deltaTime);
                transform.Rotate(new Vector3(0f, 0f, -rotation_factor * y) * Time.deltaTime);
                input.Dispose();
                output.Dispose();
                if (distance <= 0.1f)
                {
                    isDead = true;
                    worker.Dispose();
                }
            }
            if (timeAlive > maxTimeAlive)
            {
                isDead = true;
                worker.Dispose();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isDead = true;
        worker.Dispose();
    }
}
