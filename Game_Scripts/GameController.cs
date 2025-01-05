using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Assign this in the Unity Editor
    private int score = 0;

    public void GoalReached()
    {
        score++; // Increment score
        UpdateScoreText(); // Update the score display
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
