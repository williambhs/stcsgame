using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Explosion : MonoBehaviour
{
    public Action<Explosion> Exploded;

    //[SerializeField] private GameObject player;
    [SerializeField] private float explosionPower = 30;
    [SerializeField] private float explosionRadius = 10;
    [SerializeField] private float maxDistance;
    private float timeRemaining = 3;
    private bool timerIsRunning = false;
    private Text countdownLabel;
    private float explosionAnimationDuration = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;
        GetComponent<SpriteRenderer>().color = Color.green;
        //countdownLabel = GameObject.Find("CountdownLabel").GetComponent<Text>();

        UpdateCountdownLabel(timeRemaining);
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

                UpdateCountdownLabel(timeRemaining);
            }
            else
            {
                UpdateCountdownLabel(0);

                timeRemaining = 0;
                timerIsRunning = false;

                GetComponent<SpriteRenderer>().color = Color.red;

                PlayExplosionAnimation();

                Exploded?.Invoke(this);
            }
        }
    }

    private void UpdateCountdownLabel(float seconds)
    {
        //countdownLabel.text = seconds.ToString("0.00") + "s";
    }

    private void PlayExplosionAnimation()
    {
        // Add Play animation logic here.
        //animator.Play("StateName");

        // Remove this object after the animation is complete.
        Invoke(nameof(RemoveExplodedObject), explosionAnimationDuration);
    }

    private void RemoveExplodedObject()
    {
        Destroy(this.gameObject);
    }

    // Return a value between 0 and explostion power, based on the other object's position
    // reltative to this bomb
    public Vector2 GetRelativePower(Vector2 otherObjectPosition)
    {
        //float distance = Vector2.Distance(transform.position, otherObjectPosition);
        // distance = Mathf.Abs(distance);
        // Make sure the distance isn't larger than the blast radius. Otherwise, the
        // calculations will be wrong.
        //distance = Mathf.Min(distance, explosionRadius);


        //calculate x and y distance 
        //float distanceX = otherObjectPosition.x - transform.position.x;
        //float distanceY = otherObjectPosition.y - transform.position.y;
        // The further away the other object is, the smaller the value will be. A distance of 0
        // will get the full explosion power. A distance >= the explosion radius will get none.

        float relativePowerX = GetRelativePowerForAxis(transform.position.x, otherObjectPosition.x);
        float relativePowerY = GetRelativePowerForAxis(transform.position.y, otherObjectPosition.y);

        //if (distanceX < maxDistanceX && distanceX > minDistanceX)
        //{
        //    relativePowerX = Mathf.Sign(distanceX) * explosionPower * (1 - Mathf.Abs(distanceX) / explosionRadius);
        //    relativePowerX = 3* explosionPower / distanceX;//* (1 - distanceX / explosionRadius);
        //}
        //if (distanceY < maxDistanceY && distanceY > minDistanceY)
        //{
        //    relativePowerY = (explosionPower / 3) / (distanceY + 1);//* (1 - distanceY / explosionRadius);
        //}

        return new Vector2(relativePowerX, relativePowerY);
    }

    private float GetRelativePowerForAxis(float explositonPosition, float otherObjectPosition)
    {
        float distance = otherObjectPosition - explositonPosition;

        float absDistance = Mathf.Abs(distance);
        float relativePower = 0;

        if (absDistance < maxDistance && absDistance > 0.1f)
        {
            relativePower = Mathf.Sign(distance) * explosionPower * (1 - absDistance / explosionRadius);
        }

        return relativePower;
    }


    // Return a value between 0 and explostion power, based on the other object's position
    // reltative to this bomb
    public Vector2 GetAppliedForce(Vector2 otherObjectPosition)
    {
        Vector2 direction = otherObjectPosition - new Vector2(transform.position.x, transform.position.y);


        float distance = direction.magnitude;

        direction.Normalize();

        float falloff = 1 - (distance / explosionRadius);

        float force = falloff * explosionPower;
        float forceY = falloff * (explosionPower / 2);

        return force * direction;
    }

}
 