using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    private void Start()
    {
   
    }




    // collision with obstacles and stuff?
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene("End Screen");
        }
    }

}
