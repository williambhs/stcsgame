using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameFinishCollider : MonoBehaviour
{
    [SerializeField] private GameObject gameTimer;
    [SerializeField] private GameObject player;
    private Rigidbody2D playerBody;
    private GameTimerScript gameTimerScript;
    public string scene;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = player.GetComponent<Rigidbody2D>();
        gameTimerScript = gameTimer.GetComponent<GameTimerScript>();

    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == playerBody)
        {
            //go to different scene
            SceneManager.LoadSceneAsync("EndScreen_Finished", LoadSceneMode.Single);

            gameTimerScript.gameEnded = true;
        }

                
    }
}
