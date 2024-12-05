using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NonRewardObject : ETObject
{
    [SerializeField] TextMeshProUGUI _testDuration;
    private float focusTime;
    private bool isTimerRunning;
    private List<float> focusDurations = new List<float>();
    private float timeThreshold = 1.0f; // 1 second threshold

    //error handling of pupil out of focus but then focus again
    private float offsetTracking = 0.5f;  // Offset duration to avoid misTracking
    private float unfocusTimer = 0f;
    private bool isUnfocusTimerRunning = false;
    private void Update()
    {
        if (isFocused && isTimerRunning)
        {
            focusTime += Time.deltaTime;
            _testDuration.text = focusTime.ToString();
        }
        if (isUnfocusTimerRunning)
        {
            unfocusTimer += Time.deltaTime;

            if (unfocusTimer >= offsetTracking)
            {
                isUnfocusTimerRunning = false;  // Offset timer reached, consider it truly unfocused
                AddFocusDuration();
            }
        }
    }

    public override void IsFocused()
    {
        if (isUnfocusTimerRunning)
        {
            // If we refocus before offsetTracking, cancel the unfocus event
            isUnfocusTimerRunning = false;
        }
        else
        {
            base.IsFocused();
            isTimerRunning = true;
        }
        if (gameObject.GetComponent<GhostBehavior>() != null)
            gameObject.GetComponent<GhostBustBehavior>().GlowColor(); 

    }

    public override void UnFocused()
    {
        base.UnFocused();
        isTimerRunning = false;
        focusTime = 0f;
        unfocusTimer = 0f;
        isUnfocusTimerRunning = true;  // Start the offset timer
        if (gameObject.GetComponent<GhostBehavior>() != null)
            gameObject.GetComponent<GhostBustBehavior>().ResetColor();
    }
    private void AddFocusDuration()
    {
        if (focusTime >= timeThreshold)
        {
            focusDurations.Add(focusTime);
        }

        _testDuration.text = focusDurations.Count.ToString();
    }
    public void UpdateTotalFixationTime()
    {
        float totalFocusTime = 0f;
        foreach (float duration in focusDurations)
        {
            totalFocusTime += duration;
        }

        PUWStats.AddFixationTimeNonAlcoholicDisplays(totalFocusTime);
    }
}
