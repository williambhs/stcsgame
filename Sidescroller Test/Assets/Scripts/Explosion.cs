using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Action<Explosion> Exploded;

    //[SerializeField] private GameObject player;
    [SerializeField] private float explosionPower = 30;
    [SerializeField] private float explosionRadius = 10;
    private float timeRemaining = 4;
    private bool timerIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        //acting as a timer to count down bomb towards explosion
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;

                GetComponent<SpriteRenderer>().color = Color.red;

                Exploded?.Invoke(this);
            }
        }
    }


    // Return a value between 0 and explostion power, based on the other object's position
    // reltative to this bomb
    public float GetRelativePower(Vector2 otherObjectPosition)
    {
        float distance = Vector2.Distance(transform.position, otherObjectPosition);

        distance = Mathf.Abs(distance);

        // Make sure the distance isn't larger than the blast radius. Otherwise, the
        // calculations will be wrong.
        distance = Mathf.Min(distance, explosionRadius);

        // The further away the other object is, the smaller the value will be. A distance of 0
        // will get the full explosion power. A distance >= the explosion radius will get none.
        float relativePower = explosionPower * (1 - distance / explosionRadius);
        
        return relativePower;
    }

    
}
