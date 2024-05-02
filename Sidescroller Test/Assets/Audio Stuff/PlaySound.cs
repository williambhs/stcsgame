using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource soundPlayer; 
    public void playButtonSound()
    {
        //DontDestroyOnLoad(transform.gameObject);
        soundPlayer.Play();
    }

}
