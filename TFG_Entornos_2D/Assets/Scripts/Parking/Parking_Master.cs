using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Barracuda;
using System.Net.Http;
using System.IO;
using UnityEditor;

public class Parking_Master : MonoBehaviour
{
    private int number_cars = 30;
    private HttpClient client;
    private string state;
    public GameObject spawnBox;
    private Bounds bounds;

    public class Car
    {
        public Parking_Move car_data;
        public int _id;

        public Car(GameObject aux_car, int id)
        {
            car_data = aux_car.GetComponent<Parking_Move>();
            car_data.model = ModelLoader.Load((NNModel)Resources.Load($"Parking/individual_{id}"));
            _id = id;
        }
    }

    public GameObject car_obj;
    private List<Car> cars = new List<Car>();
    // Start is called before the first frame update
    void Start()
    {
        bounds = spawnBox.GetComponent<Collider2D>().bounds;
        state = "playing";
        for (int i = 0; i < number_cars; i++)
        {
            float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
            float offsetY = Random.Range(-bounds.extents.y, bounds.extents.y);
            float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
            GameObject aux_car = Instantiate(car_obj);
            cars.Add(new Car(aux_car, i));
            aux_car.transform.position = bounds.center + new Vector3(offsetX, offsetY, offsetZ);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckAllDead())
        {
            if (state == "playing")
            {
                UpdateValues(cars);
                state = "breading";
            }
        }
    }

    private bool CheckAllDead()
    {
        bool all_dead = true;
        foreach (Car car in cars)
        {
            if (!car.car_data.isDead)
            {
                all_dead = false;
                break;
            }
        }
        return all_dead;
    }

    private async void UpdateValues(List<Car> carScores)
    {
        client = new HttpClient();
        var values = new Dictionary<string, string>
        {
        };
        for (int i = 0; i < number_cars; i++)
        {
            values[carScores[i]._id.ToString()] = carScores[i].car_data.distance.ToString();
        }
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://127.0.0.1:5000/home", content);
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
        AssetDatabase.Refresh();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
