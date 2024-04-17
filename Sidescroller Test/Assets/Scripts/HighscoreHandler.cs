using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreHandler : MonoBehaviour
{
    int highscore;
    private void Start()
    {
        SetLatestHighscore();
    }

    private void SetLatestHighscore()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    private void SaveHighscore (int score)
    {
        PlayerPrefs.SetInt("Highscore", score);
    }

    public void SetHighscoreifGreater (int score)
    {
        if (score > highscore)
        {
            highscore = score;
            SaveHighscore(score);
        }
    }

}
