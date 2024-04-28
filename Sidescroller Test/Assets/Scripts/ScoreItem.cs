using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour
{
    public void SetPlayerScore(PlayerScore playerScore)
    {
        // Store the playerScore here. We will set the text on the Start method later.
        this.playerScore = playerScore;
    }

    // Start is called before the first frame update
    void Start()
    {
        var nameLabel = nameGameObject.GetComponent<TextMeshProUGUI>();
        var scoreLabel = scoreGameObject.GetComponent<TextMeshProUGUI>();

        nameLabel.text = playerScore.name;
        scoreLabel.text = HighScoreManager.GetScoreFormattedString(playerScore.score);
    }

    // Update is called once per frame
    void Update()
    {
    }

    [SerializeField] private GameObject nameGameObject;
    [SerializeField] private GameObject scoreGameObject;

    private PlayerScore playerScore;
}
