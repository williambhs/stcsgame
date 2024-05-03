using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public void SetOwner(IGameObjectOwner owner)
    {
        this.owner = owner;
    }

    private void OnCloseButtonClick()
    {

        //Destroy(this.gameObject);
        SceneManager.LoadScene("StartScreen_Finished");
        if (owner != null)
        {
            owner.GameObjectDestroyed(this.gameObject);
            owner = null;
        }
    }

    private void LoadScores()
    {
        //HighScoreManager.ClearScores();

        //HighScoreManager.AddScore("ANT", 5000);
        //HighScoreManager.AddScore("DBOT", 1000);
        //HighScoreManager.AddScore("TBIZ", 7000);

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
    private IGameObjectOwner owner;
}
