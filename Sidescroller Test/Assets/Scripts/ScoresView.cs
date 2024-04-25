using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var buttonObject = GameObject.Find("CloseButton");

        if (buttonObject != null)
        {
            closeButton = buttonObject.GetComponent<Button>();

            closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        scoresPanel = GameObject.Find("ScoresPanel");

        LoadScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCloseButtonClick()
    {
        Destroy(this.gameObject);
    }

    private void LoadScores()
    {
        HighScoreManager.ClearScores();

        HighScoreManager.AddScore("ANT", 5000);
        HighScoreManager.AddScore("DBOT", 1000);
        HighScoreManager.AddScore("TBIZ", 7000);

        var scoresList = HighScoreManager.GetScores();

        foreach (var playerScore in scoresList)
        {
            var scoreItemGameObject = Instantiate(scoreItemPrefab);

            scoreItemGameObject.transform.SetParent(scoresPanel.transform, false);

            var scoreItem = scoreItemGameObject.GetComponent<ScoreItem>();

            scoreItem.SetPlayerScore(playerScore);
        }
    }

    [SerializeField] private GameObject scoreItemPrefab;

    private Button closeButton;
    private GameObject scoresPanel;
}
