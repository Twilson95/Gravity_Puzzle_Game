using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;
    private float heightLimit;
    private float widthLimit;
    public float speed = 5f;

    void Start()
    {
        mainCamera = Camera.main;
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isOutOfBounds = transform.localPosition.x < -heightLimit || 
                             transform.localPosition.x > heightLimit || 
                             transform.localPosition.y < -widthLimit || 
                             transform.localPosition.y > widthLimit;

        if (isOutOfBounds)
        {
            ResetAsteroid();
        }
    }

    void ResetAsteroid()
    {
        Vector3 newPosition = transform.localPosition;

        float newXPos = rb.linearVelocity.y > 0 ? -heightLimit : heightLimit;

        Transform parentTransform = transform.parent;
        transform.localPosition = new Vector3(newXPos, newPosition.y, newPosition.z);

        Vector2 newDirection = rb.linearVelocity.y > 0 ? parentTransform.right : -parentTransform.right;
        rb.linearVelocity = newDirection.normalized * speed;
    }

    public void SetStats(float newSpeed, float newHeight, float newWidth)
    {
        speed = newSpeed;
        heightLimit = newHeight;
        widthLimit = newWidth;
    }
}
