using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    // public float effectRadius = 1;
    // public float attraction = 1;
    // public float acceleration;

    private Transform planetsContainer;
    private GameObject[] gravityContributors;

    void Awake()
    {   
        planetsContainer = this.transform;
        FindGravityContributors();
    }

    public void FindGravityContributors()
    {
        // Initialize a new list for planets and moons that contribute to gravity
        List<GameObject> gravityContributorsList = new List<GameObject>();

        // Iterate through each child of the planetsContainer
        foreach (Transform planetTransform in planetsContainer)
        {
            // Check if the planet is active (enabled)
            if (!planetTransform.gameObject.activeInHierarchy)
            {
                return;
            }
            // Add the planet to the list of gravity contributors
            gravityContributorsList.Add(planetTransform.gameObject);
            
            // Iterate through each child of the planet, assuming these are moons
            foreach (Transform moonTransform in planetTransform)
            {
                // Check if the moon is active (enabled)
                if (moonTransform.gameObject.activeInHierarchy)
                {
                    // Add the moon to the list of gravity contributors
                    gravityContributorsList.Add(moonTransform.gameObject);
                }
            }
            
        }

        // Convert the List to an array if necessary for your existing logic
        gravityContributors = gravityContributorsList.ToArray();
    }

    public GameObject[] GetGravityContributors()
    {
        return gravityContributors;
    }
}
