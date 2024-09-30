using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject timerPanel;
    public TextMeshProUGUI timerText;

    public float currentTime;
    public bool running;

    private void Update()
    {
        if (running)
        {
            currentTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ShowTimer()
    {
        timerPanel.SetActive(true);
    }

    public void StartTimer()
    {
        currentTime = 0;
        running = true;
    }

    public void StopTimer()
    {
        //currentTime = 0;
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        GameManager.GetInstance().finalTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        running = false;
    }

    public void PauseTimer()
    {
        running = false;
    }

    public void ResumeTimer()
    {
        running = true;
    }

    public void RestartTimer()
    {
        currentTime = 0;
    }
}
