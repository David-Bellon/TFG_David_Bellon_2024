using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Barracuda;
using System.Net.Http;
using System.IO;
using UnityEditor;
using System.Text;


[System.Serializable]
public class ObservationsList
{
    public List<Observation> observations;

    public ObservationsList(List<Observation> observations)
    {
        this.observations = observations;
    }
}

public class MasterPPO : MonoBehaviour
{
    private int number_cars = 10;
    private HttpClient client;
    private string state;
    public GameObject spawnBox;
    private Bounds bounds;

    public class Car
    {
        public CarMovePPO car_data;
        public int _id;

        public Car(GameObject aux_car, int id)
        {
            car_data = aux_car.GetComponent<CarMovePPO>();
            car_data.model = ModelLoader.Load((NNModel)Resources.Load($"individual"));
            _id = id;
        }
    }

    public GameObject car_obj;
    private List<Car> cars = new List<Car>();
    // Start is called before the first frame update
    void Start()
    {
        bounds = spawnBox.GetComponent<Collider>().bounds;
        state = "playing";
        for (int i = 0; i < number_cars; i++)
        {
            float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
            float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
            GameObject aux_car = Instantiate(car_obj);
            cars.Add(new Car(aux_car, i));
            aux_car.transform.position = bounds.center + new Vector3(offsetX, -1f, offsetZ);
            float randomRotation = Random.Range(0f, 360f);
            aux_car.transform.rotation = Quaternion.Euler(0, randomRotation, 0);
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
        List<Observation> observations = new List<Observation>();
        foreach (Car car in carScores)
        {
            observations.AddRange(car.car_data.observations);
        }

        ObservationsList observationsList = new ObservationsList(observations);

        client = new HttpClient();
        string json = JsonUtility.ToJson(observationsList);
        Debug.Log(json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://127.0.0.1:5000/home", content);
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
        AssetDatabase.Refresh();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
