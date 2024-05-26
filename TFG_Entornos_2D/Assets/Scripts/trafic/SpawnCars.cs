using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour
{
    public LineRenderer[] SpawningRoads;
    public GameObject carToSpawn;
    public float radious;
    private bool spawn;
    public ContactFilter2D contactFilter = new ContactFilter2D();

    public Transform env;
    // Start is called before the first frame update
    void Start()
    {
        spawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        spawn = false;
        int i = Random.Range(0, SpawningRoads.Length);
        Vector3 spawnPoint = SpawningRoads[i].GetPosition(0) + SpawningRoads[i].transform.position;
        Collider2D[] results = new Collider2D[5];
        int objectsDetected = Physics2D.OverlapCircle(spawnPoint, radious, contactFilter, results);
        if (objectsDetected == 0)
        {
            GameObject car = Instantiate(carToSpawn, spawnPoint, Quaternion.identity, env);
            car.GetComponent<FollowLines>().lr = SpawningRoads[i];
        }
        yield return new WaitForSeconds(0.2f);
        spawn = true;
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(transform.position, radious);
    }
}
