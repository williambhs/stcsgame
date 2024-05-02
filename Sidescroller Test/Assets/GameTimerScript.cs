using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimerScript : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float currentTime;
    public bool countUp;
    public bool gameEnded;
    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    private void Awake()
    {
        gameEnded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded == false)
        {
            currentTime += Time.deltaTime;
            SetTimerText();
        }



    }

    private void SetTimerText()
    {
        timerText.text = currentTime.ToString("0.00");
    }
    public float getCurrentTime()
    {
        return currentTime;
    }
}
