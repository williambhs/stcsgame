using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameFinishCollider : MonoBehaviour
{
    [SerializeField] private GameTimerScript gameTimerScript;
    [SerializeField] private Collider2D playerCollider;
    public string scene;

    // Start is called before the first frame update
    void Start()
    {
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == playerCollider)
        {
            gameTimerScript.gameEnded = true;

            HighScoreManager.TrySetPendingHighScore((uint)(gameTimerScript.getCurrentTime() * 1000));

            //go to different scene
            SceneManager.LoadSceneAsync("EndScreen_Finished", LoadSceneMode.Single);
        }

                
    }
}
