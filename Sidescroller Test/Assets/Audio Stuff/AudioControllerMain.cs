using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerMain : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameMusic").GetComponent<Music>().PlayMusic();
        GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<Music>().StopMusic();
    }
}
