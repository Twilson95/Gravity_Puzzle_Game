using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBelt : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public int numberOfAsteroids = 50;
    public float width = 10f; // Width of the asteroid belt
    public float height = 5f; // Height of the asteroid belt
    public float speed = 5f;
    public float percentageGoingOneDirection = 50f; // Percentage of Asteroids going left
    public int levelSeed = 12345;

    private bool showAsteroids = false;
    private List<GameObject> asteroids = new List<GameObject>();

    void Start()
    {

    }

    void OnEnable()
    {
        SpawnAsteroids();
    }

    void OnDisable()
    {
        RemoveAsteroids();
    }

    public void ToggleAsteroids()
    {
        showAsteroids = !showAsteroids;
        if(showAsteroids)
        {
            SpawnAsteroids();
        }
        else{
            RemoveAsteroids();
        }
    }

    void RemoveAsteroids()
    {
        foreach(GameObject asteroid in asteroids)
        {
            asteroids = new List<GameObject>();
            Destroy(asteroid);
        }
    }

    void SpawnAsteroids()
    {
        Random.InitState(levelSeed);
        asteroids = new List<GameObject>();

        int asteroidsInOneDirection = Mathf.RoundToInt(numberOfAsteroids * (percentageGoingOneDirection / 100f));
        int asteroidsInOtherDirection = numberOfAsteroids - asteroidsInOneDirection;

        for (int i = 0; i < numberOfAsteroids; i++)
        {
            Vector3 spawnPosition = transform.position + 
            (transform.right * Random.Range(-height, height)) + 
            (transform.up * Random.Range(-width / 2, width / 2));

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity, transform);
            asteroid.GetComponent<Asteroid>().SetStats(speed, height, width);
            asteroid.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            asteroids.Add(asteroid);

            Vector2 directionVector = i < asteroidsInOneDirection ? -transform.right : transform.right;
            asteroid.GetComponent<Rigidbody2D>().linearVelocity = directionVector.normalized * speed;
        }
    }

}