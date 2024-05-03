using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Instructions");
    }
}
