using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{

    public void ChangeToScene(string sceneToChangeTo)
    {
        StartCoroutine(ChangeScene(sceneToChangeTo,0.1f));
    }

    public void LoadMenu()
    {
        ChangeToScene("StartScreen_Finished");
    }

    public void LoadInstructions()
    {
        ChangeToScene("Instructions");
    }

    public void LoadGame()
    {
        ChangeToScene("Game_Finished");
    }

    public void LoadLeaderboard()
    {
        ChangeToScene("EndScreen_Finished");
    }
    
    // delay buffer cuz unity sucks and switches the scene too fast
    IEnumerator ChangeScene(string sceneToChangeTo,float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToChangeTo);
    }
}
