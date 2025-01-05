using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject meteor; // Assign this in the Unity Editor
    // public float screenVerticalRange = 5f; // Adjust as needed
    public float verticalRange = 5f;
    public Transform starsContainer;
    public AsteroidBeltPool asteroidBeltPool;
    public MeteorGravity meteorGravity;
    // public LevelDetails levelDetails;
    public string currentSectionName; 
    public int currentLevelNumber = 0;
    private SaveLoadScript saveLoadScript;

    public void SetLevels(string section, int level)
    {
        currentSectionName = section;
        currentLevelNumber = level;
    }

    void Awake()
    {
        saveLoadScript = FindObjectOfType<SaveLoadScript>();
        RandomLevel();
    }

    void PlacePlanets()
    {

    }

    public void NextLevel()
    {
        int numberOfLevelsInSection = saveLoadScript.GetNumberOfLevelsForSection(currentSectionName);
        if (currentLevelNumber >= numberOfLevelsInSection)
        {
            Debug.Log("No more levels in this section");
            MenuController menuController = FindObjectOfType<MenuController>();
            menuController.OpenLevelSelect();
            return;
        }
        if (currentLevelNumber == 0)
        {
            Debug.Log("Random level");
            RandomLevel();
        }

        Debug.Log("Next level");
        LoadLevel(currentSectionName, currentLevelNumber + 1);
    }

    public void LoadLevel(string section, int level)
    {
        Debug.Log("Loading level " + level.ToString() + " in section " + section.ToString());
        currentLevelNumber = level;
        SetLevels(section, level);
        saveLoadScript.SetLevels(section, level);
        saveLoadScript.LoadLevel();
    }

    public void ResetLevel()
    {
        meteor.GetComponent<MeteorController>().ResetMeteor();
        ResetStars();
        // asteroidBeltPool.ResetAsteroidBelts();
    }

    public void ResetStars()
    {
        foreach (Transform starTransform in starsContainer)
        {
            starTransform.gameObject.SetActive(true);
        }
    }

    public void RandomLevel()
    {
        // Convert screen edges to world space
        Vector2 leftSide = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector2 rightSide = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));

        // Set initial position for the meteor
        Vector2 meteorPosition = new Vector2(leftSide.x + 3f, Random.Range(-verticalRange, verticalRange));
        meteor.GetComponent<MeteorController>().SetPosition(meteorPosition);

        // Positioning planets
        GameObject[] planets = GameObject.FindGameObjectsWithTag("planet");
        GameObject goalPlanet = GameObject.FindGameObjectWithTag("goal");

        // Set position for planets with tag 'Planet'
        foreach (var planet in planets)
        {
            float randomY = Random.Range(-verticalRange, verticalRange);
            planet.transform.position = new Vector2(Random.Range(leftSide.x / 2, rightSide.x / 2), randomY);
        }

        foreach (Transform starTransform in starsContainer)
        {
            starTransform.gameObject.SetActive(true);
            float randomY = Random.Range(-verticalRange, verticalRange);
            starTransform.position = new Vector2(Random.Range(leftSide.x / 2, rightSide.x / 2), randomY);
        }

        // Set position for the planet with tag 'GoalPlanet'
        if (goalPlanet != null)
        {
            goalPlanet.transform.position = new Vector2(rightSide.x - 1.1f, Random.Range(-verticalRange, verticalRange));
        }
        meteorGravity.RecalculateGravityContributors();
        meteorGravity.GetGravityContributors();
    }
}
