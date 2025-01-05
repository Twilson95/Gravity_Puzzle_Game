using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitScript : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform centerPoint; // The center point of rotation
    public float radius = 5f;     // Radius of the circle
    public float speed = 1f;      // Speed of rotation

    private float angle;          // Current angle

    private int currentWaypointIndex = 0;

    void OnEnable()
    {
        if (!centerPoint)
        {
            centerPoint = transform.parent;
        }
    }

    void Update()
    {
        // FollowPath();
        CircularPath();
    }

    void FollowerPath()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (transform.position == targetWaypoint.position)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }


    void CircularPath()
    {
        // Increment the angle based on speed and time
        angle += speed * Time.deltaTime;

        // Calculate the new position
        float x = centerPoint.position.x + radius * Mathf.Cos(angle);
        float y = centerPoint.position.y + radius * Mathf.Sin(angle);

        // Update the position of the planet
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
