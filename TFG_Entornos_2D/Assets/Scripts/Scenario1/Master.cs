using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Barracuda;
using System.Net.Http;
using System.IO;
using UnityEditor;


public class Master : MonoBehaviour
{

    private int number_cars = 20;
    private HttpClient client;
    private string state;

    public class Car
    {
        public CarMove car_data;
        public int _id;

        public Car(GameObject aux_car, int id)
        {
            car_data = aux_car.GetComponent<CarMove>();
            car_data.model = ModelLoader.Load((NNModel)Resources.Load($"Scenario1/individual_{id}"));
            _id = id;
        }
    }

    public GameObject car_obj;
    private List<Car> cars = new List<Car>();
    // Start is called before the first frame update
    void Start()
    {
        state = "playing";
        for (int i = 0; i < number_cars; i++)
        {
            GameObject aux_car = Instantiate(car_obj);
            cars.Add(new Car(aux_car, i));
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
        foreach(Car car in cars)
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
            values[carScores[i]._id.ToString()] = carScores[i].car_data.score.ToString();
        }
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://127.0.0.1:5000/home", content);
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
        AssetDatabase.Refresh();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}