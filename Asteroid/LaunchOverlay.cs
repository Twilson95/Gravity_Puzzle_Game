using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to include this if you're using TextMeshPro

public class LaunchOverlay : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI angleText;
    public Transform meteorTransform; // Assign the meteor's transform in the inspector

    // Optional: Variables to control the display logic
    // private bool showOverlay = true;

    void Start()
    {
        meteorTransform = this.transform.parent;
    }

    void Update()
    {
        // if (showOverlay)
        // {
        //     UpdateOverlay();
        // }
    }

    public void UpdateOverlay(Vector2 launchVelocity, float maxInitialSpeed)
    {
        // Example calculation for speed percentage and angle
        float speedPercent = CalculateSpeedPercent(launchVelocity, maxInitialSpeed);
        float angle = CalculateLaunchAngle(launchVelocity);

        // Update the UI elements
        // speedText.text = $"Speed: {speedPercent}%";
        // angleText.text = $"Angle: {angle}°";

        speedText.text = $"{speedPercent}%";
        angleText.text = $"{angle}°";
    }

    public void ClearOverlay()
    {
        speedText.text = "";
        angleText.text = "";
    }

    float CalculateSpeedPercent(Vector2 velocity, float maxSpeed)
    {
        float speed = velocity.magnitude;
        float speedPercentage = Mathf.Round(100 * (speed / maxSpeed) * 10) / 10f;
        return speedPercentage;
    }

    float CalculateLaunchAngle(Vector2 velocity)
    {
        float angleRadians = Mathf.Atan2(velocity.y, velocity.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        return Mathf.Round(angleDegrees * 10) / 10f;
    }
}
