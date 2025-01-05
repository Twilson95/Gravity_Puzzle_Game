using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectorySimulator : MonoBehaviour
{
    public LineRenderer trajectoryLineRenderer;
    public Rigidbody2D meteorRigidbody;
    public float simulationTime = 2f;
    public float timeStep = 0.1f;
    public float G = 6.674f; // Gravitational constant
    public MeteorGravity meteorGravity;
    public bool collisionDetected;

    private GameObject[] planets;

    void Start()
    {
        planets = meteorGravity.GetGravityContributors();

        trajectoryLineRenderer.positionCount = (int)(simulationTime / timeStep);
    }

    public void UpdateTrajectory(Vector2 initialVelocity)
    {
        planets = meteorGravity.GetGravityContributors();
        List<Vector3> trajectoryPoints = SimulateTrajectory(meteorRigidbody.position, initialVelocity, simulationTime, timeStep);
        trajectoryLineRenderer.positionCount = (int)(simulationTime / timeStep);
        trajectoryLineRenderer.SetPositions(trajectoryPoints.ToArray());
    }

    List<Vector3> SimulateTrajectory(Vector2 initialPosition, Vector2 initialVelocity, float duration, float step)
    {
        List<Vector3> trajectoryPoints = new List<Vector3>();
        Vector2 currentPosition = initialPosition;
        Vector2 currentVelocity = initialVelocity;

        collisionDetected = false;

        for (float t = 0; t < duration; t += step)
        {
            Vector2 totalGravityForce = Vector2.zero;
            foreach (GameObject planet in planets)
            {
                Rigidbody2D planetRigidbody = planet.GetComponent<Rigidbody2D>();
                if (planetRigidbody == null) continue;

                Vector2 directionToPlanet = (Vector2)planet.transform.position - currentPosition;
                float distance = directionToPlanet.magnitude;

                float planetRadius = planetRigidbody.GetComponent<CircleCollider2D>().radius * planet.transform.localScale.x;

                if (distance < planetRadius)
                {
                    collisionDetected = true;
                    break;
                }

                if (distance == 0f) continue;

                // float planetMass = Mathf.Pow(planet.transform.localScale.x,2) * Mathf.Pi;
                float planetMass = 3f * Mathf.PI * Mathf.Pow(planet.transform.localScale.x/2, 1);
                // float planetMass = planetRigidbody.mass;

                totalGravityForce += G * (planetMass * meteorRigidbody.mass) / (distance * distance) * directionToPlanet.normalized;
            }

            if (collisionDetected)
            {
                trajectoryPoints.Add(currentPosition);
                continue; // Stop simulating further steps on collision
            }

            // Assuming mass of meteor is 1 for simplicity
            Vector2 gravityAcceleration = totalGravityForce / meteorRigidbody.mass;
            currentVelocity += gravityAcceleration * step;
            currentPosition += currentVelocity * step;
            trajectoryPoints.Add(currentPosition);
        }

        return trajectoryPoints;
    }

    public void ClearTrajectory()
    {
        trajectoryLineRenderer.positionCount = 0;
    }
}