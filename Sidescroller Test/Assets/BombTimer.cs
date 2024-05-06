using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class BombTimer : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timertext;

    [Header("Timer Settings")]
    public float currentTime;
    private float secondCurrentTime;

    [Header("Limit Settings")]
    public bool hasLimit;
    public float timerLimit;

    // Start is called before the first frame update
    void Start()
    {
        timerLimit = 0;
        currentTime = 0;
        secondCurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X) && secondCurrentTime == 0)
        {
            setTimer(3);

        }
        
        if (currentTime > timerLimit)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0;
        }

        if (secondCurrentTime > timerLimit)
        {
            secondCurrentTime -= Time.deltaTime;
        }
        else
        {
            secondCurrentTime = 0;
        }
        timertext.text = "Bomb Time: " + currentTime.ToString();
        
    }

    public void setTimer(float time)
    {
        currentTime = time;
        secondCurrentTime = 5;
    }

  
}
