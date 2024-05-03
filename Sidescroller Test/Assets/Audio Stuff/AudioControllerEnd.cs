using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerEnd : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("EndMusic").GetComponent<Music>().PlayMusic();
        GameObject.FindGameObjectWithTag("GameMusic").GetComponent<Music>().StopMusic();
        GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<Music>().StopMusic();
    }
}
