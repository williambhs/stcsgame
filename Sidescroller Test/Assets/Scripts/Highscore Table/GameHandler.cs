using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] PointCounter pointCounter;
    [SerializeField] HighscoreHandler highscoreHandler;
    //[SerializeField] PointHUD pointHUD;

    public void StartGame()
    {
        pointCounter.StartGame();
    }

    public void StopGame()
    {
        highscoreHandler.SetHighscoreIfGreater (pointHUD.Points);
        pointCounter.StopGame();
    }
}
