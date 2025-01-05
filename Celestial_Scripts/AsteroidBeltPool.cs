using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBeltPool : MonoBehaviour
{
    public GameObject asteroidBeltPrefab; // Assign this in the inspector
    public int poolSize = 5;
    private List<GameObject> asteroidBelts;
    public bool showAsteroids = false;

    void Start()
    {
        asteroidBelts = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject belt = Instantiate(asteroidBeltPrefab, transform);
            belt.SetActive(false); // Start with all belts deactivated
            asteroidBelts.Add(belt);
        }
    }

    public GameObject GetAsteroidBelt()
    {
        foreach (GameObject belt in asteroidBelts)
        {
            if (!belt.activeInHierarchy)
            {
                belt.SetActive(true);
                return belt;
            }
        }

        // Optionally expand the pool if all belts are in use (not recommended for strict pooling)
        return null; // or handle expanding the pool here
    }

    public GameObject CreateNewAsteroidBelt()
    {
        GameObject belt = Instantiate(asteroidBeltPrefab, transform);
        asteroidBelts.Add(belt); // Keep track of this new belt
        return belt;
    }

    public void ResetAsteroidBelts()
    {
        foreach (GameObject belt in asteroidBelts)
        {
            belt.SetActive(false);
            
        }
    }

    public void ToggleAsteroids()
    {
        showAsteroids = !showAsteroids;
        // ASteroidBelt asteroidBelt = transform.GetChild(0).GetComponent<AsteroidBelt>()
        GameObject asteroidBelt = transform.GetChild(0).gameObject;
        if(showAsteroids)
        {
            asteroidBelt.SetActive(true);
        }
        else{
            asteroidBelt.SetActive(false);
        }
    }
}