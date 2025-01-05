using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGravity : MonoBehaviour
{
    public Rigidbody2D meteorRigidbody;
    // public Transform gravityContributorsContainer;
    public GravityManager gravityManager;
    public float G = 6.674f; // Gravitational Constant
    
    private bool isGravityEnabled = false;
    public Transform planetsContainer;
    private GameObject[] gravityContributors;

    void Start()
    {
        RecalculateGravityContributors();
    }

    void FixedUpdate()
    {
        if (!isGravityEnabled) return;

        Vector2 totalGravity = GetNetGravityForce();
        meteorRigidbody.AddForce(totalGravity);
    }

    private Vector2 GetNetGravityForce()
    {
        Vector2 totalGravity = Vector2.zero;

        foreach (GameObject planet in gravityContributors)
        {
            Vector2 directionToPlanet = (Vector2)planet.transform.position - meteorRigidbody.position;
            float distance = directionToPlanet.magnitude;

            if (distance == 0f) continue;

            // float planetMass = Mathf.Pow(planet.transform.localScale.x,2) * Mathf.Pi;
            // float planetMass = (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(planet.transform.localScale.x / 2, 3); accurate mass
            float planetMass = 3f * Mathf.PI * Mathf.Pow(planet.transform.localScale.x/2, 1);

            Vector2 gravityForce = G * (planetMass * meteorRigidbody.mass) / (distance * distance) * directionToPlanet.normalized;
            totalGravity += gravityForce;
        }

        return totalGravity;
    }

    public void RecalculateGravityContributors()
    {
        // Initialize a new list for planets and moons that contribute to gravity
        List<GameObject> gravityContributorsList = new List<GameObject>();

        foreach (Transform planetTransform in planetsContainer)
        {
            if (!planetTransform.gameObject.activeInHierarchy)
            {
                continue;
            }
            gravityContributorsList.Add(planetTransform.gameObject);
            
            foreach (Transform moonTransform in planetTransform)
            {
                if (moonTransform.gameObject.activeInHierarchy)
                {
                    gravityContributorsList.Add(moonTransform.gameObject);
                }
            }
            
        }
        gravityContributors = gravityContributorsList.ToArray();
    }

    public GameObject[] GetGravityContributors()
    {
        return gravityContributors;
    }

    public void EnableGravity()
    {
        isGravityEnabled = true;
    }

    public void DisableGravity()
    {
        isGravityEnabled = false;
    }

}
