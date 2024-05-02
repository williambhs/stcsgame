using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerMenu : MonoBehaviour
{


    void Start()
    {
        GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<Music>().PlayMusic();
        GameObject.FindGameObjectWithTag("GameMusic").GetComponent<Music>().StopMusic();
    }

}
