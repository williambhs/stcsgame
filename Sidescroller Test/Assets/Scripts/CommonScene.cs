using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonScene : MonoBehaviour, IGameObjectOwner
{
    [SerializeField] private GameObject addScorePrefab;
    [SerializeField] private GameObject highScoresPrefab;
    [SerializeField] private GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        // Set ourself as the instance that will be used when static 
        // functions are called.
        instance = this;
        debugLabel = GameObject.Find("DebugLabel").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Debug_AddHighScore();
        }
        else if (Input.GetKey(KeyCode.H))
        {
            ShowHighScores();
        }
    }

    private void ShowHighScores()
    {
        if (!overlayIsVisible)
        {
            overlayIsVisible = true;

            var highScores = Instantiate(highScoresPrefab);

            highScores.transform.SetParent(canvas.transform, false);

            var scoresView = highScores.GetComponent<ScoresView>();

            scoresView.SetOwner(this);
        }
    }

    private void Debug_AddHighScore()
    {
        if (!overlayIsVisible)
        {
            overlayIsVisible = true;
            HighScoreManager.SetPendingHighScore(9547);

            var addScore = Instantiate(addScorePrefab);

            addScore.transform.SetParent(canvas.transform, false);

            var scoreEntry = addScore.GetComponent<HighScoreEntry>();

            scoreEntry.SetOwner(this);
        }
    }

    public void GameObjectDestroyed(GameObject gameObject)
    {
        overlayIsVisible = false;
    }

    public static void PrintDebugText(string text)
    {
        instance.debugLabel.text = text;
    }

    private bool overlayIsVisible = false;
    private Text debugLabel;

    private static CommonScene instance;
}
