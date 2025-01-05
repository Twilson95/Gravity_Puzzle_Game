using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public class SaveLoadScript : MonoBehaviour
{
    public LevelData currentLevelData;
    public GameObject meteor;
    // public GameObject goalPlanet;
    public Transform planets;
    public Transform stars;
    public Transform asteroidBelts;
    public GameObject asteroidBeltPrefab;

    public string sectionName;
    public int levelNumber;

    public void SetLevels(string section, int level)
    {
        sectionName = section;
        levelNumber = level;
    }

    public void SaveLevel()
    {
        // levels are saved to the persistent file path during development and need manually copying to the resources folder for deployment.
        LevelData levelData = PopulateLevelData();

        string directoryPath = Path.Combine(Application.persistentDataPath, sectionName);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string path = Path.Combine(directoryPath, "level" + levelNumber + ".json");
        string jsonData = JsonUtility.ToJson(levelData, true);
        // string jsonData = JsonConvert.SerializeObject(levelData, settings);
        File.WriteAllText(path, jsonData);

        Debug.Log("Saved level to: " + path);
    }

    public void LoadLevel()
    {
        // check for the file in both the resources folder and persistent file path
        // When developing the persistent file path stores the levels and resources stores the levels for build
        string path = Path.Combine(Application.persistentDataPath, sectionName, "level" + levelNumber + ".json");
        string resourcesPath = $"{sectionName}/level{levelNumber}";

        TextAsset jsonFile = Resources.Load<TextAsset>(resourcesPath);

        if (File.Exists(path))
        {
            Debug.Log("Loading data from persistent path: " + path);
            string jsonData = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonData);

            DisableAllPlanets();
            ApplyLevelData(levelData);
        }
        else if (jsonFile != null)
        {
            Debug.Log("Loading data from Resources: " + resourcesPath);
            string jsonData = jsonFile.text;
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonData);

            DisableAllPlanets();
            ApplyLevelData(levelData);
        }
        else
        {
            Debug.LogError("Level file does not exist at path: " + path + " or in Resources: " + resourcesPath);
        }
    }

    public int GetNumberOfLevelsForSection(string sectionName)
    {
        // Check both the resources folder and persistent file path and return the max value
        TextAsset[] resourceFiles = Resources.LoadAll<TextAsset>(sectionName);
        int resourceLevelCount = resourceFiles.Length;

        string directoryPath = Path.Combine(Application.persistentDataPath, sectionName);
        int persistentLevelCount = 0;

        if (Directory.Exists(directoryPath))
        {
            persistentLevelCount = Directory.GetFiles(directoryPath, "*.json").Length;
        }

        int maxLevelCount = Mathf.Max(resourceLevelCount, persistentLevelCount);

        if (maxLevelCount == 0)
        {
            Debug.LogWarning("No levels found for section: " + sectionName);
        }

        return maxLevelCount;
    }

    public void DisableAllPlanets()
    {
        foreach (Transform planet in planets)
        {
            foreach (Transform moon in planet)
            {
                moon.gameObject.SetActive(false);
            }
            planet.gameObject.SetActive(false);
        }

        foreach (Transform star in stars)
        {
            star.gameObject.SetActive(false);
        }
    }

    void ApplyLevelData(LevelData levelData)
    {
        foreach (PlanetData planetData in levelData.planets)
        {
            Debug.Log(planetData.name);
            GameObject planet = GameObject.Find(planetData.name);

            foreach (Transform child in planets)
            {
                if (child.gameObject.name == planetData.name)
                {
                    planet = child.gameObject;
                }
            }

            if (!planet)
            {
                Debug.Log(planet);
                continue;
            }
            planet.transform.position = ToVector3(planetData.position);
            float planetScaleFloat = IntToFloat(planetData.scale);
            planet.transform.localScale = new Vector3(planetScaleFloat, planetScaleFloat, planetScaleFloat);
            planet.SetActive(true);

            // Handle moons
            foreach (MoonData moonData in planetData.moons)
            {
                GameObject moon = planet.transform.Find(moonData.name)?.gameObject;
                if (moon)
                {
                    OrbitScript orbitScript = moon.GetComponent<OrbitScript>();
                    orbitScript.radius = IntToFloat(moonData.radius);
                    orbitScript.speed = IntToFloat(moonData.speed);
                    moon.transform.position = ToVector3(moonData.position);
                    float moonScaleFloat = IntToFloat(moonData.scale);
                    moon.transform.localScale = new Vector3(moonScaleFloat, moonScaleFloat, moonScaleFloat);
                    moon.SetActive(true);
                }
            }

        }

        // Activate and position stars as per saved data
        foreach (Transform star in stars)
        {
            star.gameObject.SetActive(false);
        }
        int starsToLoad = levelData.stars.Count;

        for (int i = 0; i < starsToLoad; i++)
        {
            GameObject star = stars.GetChild(i).gameObject;
            star.SetActive(true);
            StarsData starData = levelData.stars[i];
            float starScaleFloat = IntToFloat(starData.scale);
            star.transform.localScale = new Vector3(starScaleFloat, starScaleFloat, starScaleFloat);
            star.transform.position = ToVector3(starData.position);
        }

        // clear belts
        foreach (Transform belt in asteroidBelts)
        {
            belt.gameObject.SetActive(false);
        }
        int beltsToLoad = levelData.asteroidBelts.Count;

        for (int i = 0; i < beltsToLoad; i++)
        {
            GameObject belt = asteroidBelts.GetChild(i).gameObject;
            AsteroidBelt beltComponent = belt.GetComponent<AsteroidBelt>();

            AsteroidBeltData beltData = levelData.asteroidBelts[i];
            float beltScaleFloat = IntToFloat(beltData.scale);
            belt.transform.localScale = new Vector3(beltScaleFloat, beltScaleFloat, beltScaleFloat);
            belt.transform.position = ToVector3(beltData.position);
            belt.transform.eulerAngles = beltData.rotation;

            beltComponent.width = IntToFloat(beltData.width);
            beltComponent.height = IntToFloat(beltData.height);
            beltComponent.speed = IntToFloat(beltData.speed);
            beltComponent.percentageGoingOneDirection = IntToFloat(beltData.percentageGoingOneDirection);
            beltComponent.levelSeed = beltData.levelSeed;

            belt.SetActive(true);
        }

        meteor.transform.position = ToVector3(levelData.meteorPosition);
        MeteorController meteorController = meteor.GetComponent<MeteorController>();
        meteorController.initialPosition = ToVector3(levelData.meteorPosition);
        meteorController.ResetMeteor();
        MeteorGravity meteorGravity = meteor.GetComponent<MeteorGravity>();
        meteorGravity.RecalculateGravityContributors();
    }

    LevelData PopulateLevelData()
    {
        LevelData levelData = new LevelData
        {
            planets = new List<PlanetData>(),
            asteroidBelts = new List<AsteroidBeltData>(),
            stars = new List<StarsData>(),
            meteorPosition = ToVector3Int(meteor.transform.position),
        };

        // Save planets and moons
        foreach (Transform planet in planets) // Replace Planet with your planet component
        {
            if (!planet.gameObject.activeInHierarchy)
            {
                continue;
            }
            var planetData = new PlanetData
            {
                name = planet.name,
                position = ToVector3Int(planet.transform.position),
                scale = FloatToInt(planet.transform.localScale.x), // Assuming uniform scaling
                moons = new List<MoonData>()
                // Debug.log("name: " + position.ToString());
            };

            foreach (Transform moon in planet) // Assuming a way to access moons from a planet
            {
                if (!moon.gameObject.activeInHierarchy)
                {
                    continue;
                }
                OrbitScript orbitDetails = moon.GetComponent<OrbitScript>();
                planetData.moons.Add(new MoonData
                {
                    name = moon.name,
                    position = ToVector3Int(moon.transform.position),
                    scale = FloatToInt(moon.transform.localScale.x),
                    radius = FloatToInt(orbitDetails.radius),
                    speed = FloatToInt(orbitDetails.speed)
                });
            }

            levelData.planets.Add(planetData);
        }

        // Save stars
        foreach (Transform star in stars)
        {
            levelData.stars.Add(new StarsData
            {
                position = ToVector3Int(star.transform.position),
                scale = FloatToInt(star.transform.localScale.x)
            });
        }

        foreach (Transform beltTransform in asteroidBelts.transform)
        {
            if (!beltTransform.gameObject.activeInHierarchy)
            {
                continue;
            }
            AsteroidBelt beltComponent = beltTransform.GetComponent<AsteroidBelt>();
            if (beltComponent == null)
            {
                continue;
            }
            AsteroidBeltData beltData = new AsteroidBeltData
            {
                numberOfAsteroids = beltComponent.numberOfAsteroids,
                width = FloatToInt(beltComponent.width),
                height = FloatToInt(beltComponent.height),
                speed = FloatToInt(beltComponent.speed),
                percentageGoingOneDirection = FloatToInt(beltComponent.percentageGoingOneDirection),
                levelSeed = beltComponent.levelSeed,
                position = ToVector3Int(beltTransform.position),
                scale = FloatToInt(beltTransform.localScale.x),
                rotation = RoundVector3(beltTransform.eulerAngles)
            };

            levelData.asteroidBelts.Add(beltData);
        }

        return levelData;
    }

    private Vector3 RoundVector3(Vector3 vector)
    {
        Vector3 roundedVector = new Vector3(
            Mathf.Round(vector.x * 1000f) * 0.001f,
            Mathf.Round(vector.y * 1000f) * 0.001f,
            Mathf.Round(vector.z * 1000f) * 0.001f
        );
        Debug.Log("Original: " + vector + ", Rounded: " + roundedVector);
        return roundedVector;
    }

    private float RoundFloat(float value)
    {
        float roundedValue = Mathf.Round(value * 1000f) * 0.001f;
        Debug.Log("Original: " + value + ", Rounded: " + roundedValue);
        return roundedValue;
    }

    private Vector2Int ToVector3Int(Vector3 vector)
    {
        return new Vector2Int(
            Mathf.RoundToInt(vector.x * 1000f),
            Mathf.RoundToInt(vector.y * 1000f)
        );
    }

    private Vector3 ToVector3(Vector2Int vector)
    {
        return new Vector3(
            vector.x / 1000f,
            vector.y / 1000f,
            0f
        );
    }

    private int FloatToInt(float value)
    {
        return Mathf.RoundToInt(value * 1000f);
    }

    private float IntToFloat(int value)
    {
        return value / 1000f;
    }
}


[System.Serializable]
public class LevelData
{
    public List<PlanetData> planets;
    public List<AsteroidBeltData> asteroidBelts;
    public List<StarsData> stars;
    public Vector2Int meteorPosition;
    // public Vector2 goalPlanetPosition;
}

[System.Serializable]
public class PlanetData
{
    public string name;
    public Vector2Int position;
    public int scale;
    public List<MoonData> moons;
}

[System.Serializable]
public class MoonData
{
    public string name;
    public Vector2Int position;
    public int scale;
    public int radius;
    public int speed;
}

[System.Serializable]
public class StarsData
{
    public Vector2Int position;
    public int scale;
}

[System.Serializable]
public class AsteroidBeltData
{
    public int numberOfAsteroids;
    public int width;
    public int height;
    public int speed;
    public int percentageGoingOneDirection;
    public int levelSeed;
    public Vector2Int position;
    public int scale;
    public Vector3 rotation;
}



// [System.Serializable]
// public class LevelData
// {
//     public List<CelestialBodyData> celestialBodies;
//     public Vector2 meteorPosition;
//     // public Vector2 goalPlanetPosition; // Uncomment or remove based on need
// }

// [System.Serializable]
// public class CelestialBodyData
// {
//     public string type; // "planet", "moon", "star", "asteroidBelt"
//     public string name;
//     public Vector3 position;
//     public float scale;
//     public Vector3 rotation; // For 2D, you can use Vector2 or simply a float for z-axis rotation
//     // Additional fields specific to moons, belts, etc., can be added as needed
//     public float width; // Specific to asteroid belts
//     public float height; // Specific to asteroid belts
//     public float speed; // Specific to asteroid belts and moons
//     public float percentageGoingOneDirection; // Specific to asteroid belts
//     // Moons specific data like radius and orbit speed can be added if needed
// }



