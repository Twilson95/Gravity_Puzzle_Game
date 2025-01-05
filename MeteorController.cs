using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeteorController : MonoBehaviour
{
    public Rigidbody2D meteorRigidbody;
    public MeteorGravity meteorGravity;
    private Vector2 initialTouchPosition;
    public bool isLaunched = false;
    public Vector2 initialPosition = new Vector2(-8.5f, 1.5f);
    public float maxInitialSpeed = 10f;
    public LaunchOverlay launchOverlay;

    public LineRenderer launchLineRenderer;
    public float maxLineLength = 5f;
    public Color minSpeedColor = Color.green;
    public Color maxSpeedColor = Color.red;

    public float adjSpeed = 1f;
    public float adjDirection = 1f;
    public float initialSpeed = 1f;
    public bool showTrajectory = false;
    public bool showOverlay = false;

    public GameController gameController;
    public LevelManager levelManager;
    public TrajectorySimulator trajectorySimulator;

    public float trajectoryUpdateCooldown = 2.0f; // Cooldown time in seconds
    public float timeSinceLastUpdate = 1.95f; // Time elapsed since last update

    public bool pullLaunch = true;
    public bool startOnUI = false;

    private bool isDragging = false;


    void Start()
    {
        initialPosition = transform.position;
        launchLineRenderer.positionCount = 3;
        launchLineRenderer.enabled = false;
    }
    

    void Update()
    {
        if (isLaunched)
        {
            CheckOffScreen();
            return;
        }

        // Detect touch start
        if (Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                startOnUI = true;
                return;
            }
            startOnUI = false;
            // int id = touch.fingerId;
            initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
            launchLineRenderer.enabled = true;
        }

        if(startOnUI)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 launchVelocity = GetInitialVelocity();
            UpdateLaunchLine(launchVelocity);

            if(showOverlay)
            {
                launchOverlay.UpdateOverlay(launchVelocity, maxInitialSpeed);
            }

            if(showTrajectory)
            {
                timeSinceLastUpdate += Time.deltaTime;
                if(timeSinceLastUpdate < trajectoryUpdateCooldown)
                {
                    return;
                }
                trajectorySimulator.UpdateTrajectory(launchVelocity);
                timeSinceLastUpdate = 0f;
            }
        }

        // Detect touch end
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LaunchMeteor(finalTouchPosition);
            isDragging = false;
            isLaunched = true;
            meteorGravity.EnableGravity();
            launchLineRenderer.enabled = false;
        }
    }


    void LaunchMeteor(Vector2 finalTouchPosition)
    {
        launchOverlay.ClearOverlay();
        Vector2 initialVelocity = GetInitialVelocity();

        // Apply force to the meteor
        meteorRigidbody.AddForce(initialVelocity * meteorRigidbody.mass, ForceMode2D.Impulse);

        TrailRenderer trailRenderer = this.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }
        trajectorySimulator.ClearTrajectory();
    }

    void CheckOffScreen()
    {
        // Convert the meteor's position to viewport space
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(meteorRigidbody.position);

        // Check if the position is outside the viewport (with a small threshold)
        if (viewportPosition.x < -0.05 || viewportPosition.x > 1.05 || viewportPosition.y < -0.1 || viewportPosition.y > 1.1)
        {
            levelManager.ResetLevel();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the meteor collides with a planet
        if (collision.gameObject.CompareTag("planet"))
        {
            Debug.Log("hit planet");
            levelManager.ResetLevel();
        }
        if (collision.gameObject.CompareTag("asteroid"))
        {
            Debug.Log("hit asteroid");
            levelManager.ResetLevel();
        }
        if (collision.gameObject.CompareTag("goal"))
        {
            Debug.Log("hit goal");
            levelManager.ResetLevel();
            gameController.GoalReached();
            levelManager.NextLevel();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("star"))
        {
            Debug.Log("collect star");
            CollectStar(collider.gameObject);
        }
    }

    void CollectStar(GameObject star)
    {
        // Example: Deactivate the collected star
        star.SetActive(false);

        // Update score or perform other actions
    }

    public void ResetMeteor()
    {
        PlaceMeteor();
        TrailRenderer trailRenderer = gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = false;
            // Debug.Log("trail cleared");
        }
        // PlaceMeteor();
        isLaunched = false;
        meteorGravity.DisableGravity();
    }

    public void PlaceMeteor()
    {
        meteorRigidbody.position = initialPosition;
        meteorRigidbody.linearVelocity = Vector2.zero;
        meteorRigidbody.angularVelocity = 0f;
        meteorRigidbody.rotation = 0f;
    }

    public void SetPosition(Vector2 position)
    {
        this.transform.position = position;
        initialPosition = position;
    }

    private Vector2 GetInitialVelocity()
    {
        adjDirection = 1f;
        adjSpeed = 1f;
        if(pullLaunch)
        {
            adjDirection = -1f;
            adjSpeed = 4f;
        }
        Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)meteorRigidbody.position) * adjDirection;
        float speed = Mathf.Min(direction.magnitude * adjSpeed, maxInitialSpeed);
        Vector2 launchVelocity = direction.normalized * speed;
        initialSpeed = launchVelocity.magnitude;

        return launchVelocity;
    }

    public void ToggleTrajectory()
    {
        showTrajectory = !showTrajectory;
    }

    public void ToggleOverlay()
    {
        showOverlay = !showOverlay;
    }

    void UpdateLaunchLine(Vector2 launchVelocity)
    {
        // Scale the distance to match the max line length
        float lineLength = (launchVelocity.magnitude / maxInitialSpeed) * maxLineLength;

        // Set the start and end positions of the line
        launchLineRenderer.SetPosition(0, meteorRigidbody.position);
        launchLineRenderer.SetPosition(1, meteorRigidbody.position + launchVelocity.normalized * lineLength);
        launchLineRenderer.SetPosition(2, meteorRigidbody.position + launchVelocity.normalized * lineLength * 1.1f);

        // Update the color of the line based on the speed
        Color currentColor = Color.Lerp(minSpeedColor, maxSpeedColor, launchVelocity.magnitude / maxInitialSpeed);
        launchLineRenderer.startColor = currentColor;
        launchLineRenderer.endColor = currentColor;
    }

}