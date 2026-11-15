using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText; // Drag your UI text here

    private float elapsedTime = 0f;
    private bool timerIsRunning = true;

    void Start()
    {
        elapsedTime = 0f;
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            // Add time passing every frame
            elapsedTime += Time.deltaTime;
            DisplayTime(elapsedTime);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Calculate minutes and seconds
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Optional: Include milliseconds if you want precision (e.g., 01:23.45)
        float milliseconds = Mathf.FloorToInt((timeToDisplay % 1) * 100);

        // Format the text to show minutes, seconds, and milliseconds
        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    // Call this method if you want to stop the timer (e.g., when reaching the goal)
    public void StopTimer()
    {
        timerIsRunning = false;
    }
}