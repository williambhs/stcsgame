using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerMenu : MonoBehaviour
{
    public PauseMenu pauseMenu;
   void Start()
    {

        GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<Music>().PlayMusic();
        GameObject.FindGameObjectWithTag("GameMusic").GetComponent<Music>().StopMusic();
        if (GameObject.FindGameObjectWithTag("EndMusic").GetComponent<Music>())
        { 
            GameObject.FindGameObjectWithTag("EndMusic").GetComponent<Music>().StopMusic();
        }
    }

    public void PlayMenu() {
        GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<Music>().PlayMusic();
    }
}
