using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // testing
    public int minutes;
    public int seconds;
    //
    [SerializeField] TextMeshProUGUI timerText;
    float elapsedTime;
    
    void Update()
    {
        elapsedTime += Time.deltaTime;
        minutes = Mathf.FloorToInt(elapsedTime / 60);
        seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
