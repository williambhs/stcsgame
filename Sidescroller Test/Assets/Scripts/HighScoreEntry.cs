using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreEntry : MonoBehaviour
{
    public void SetScore(uint score)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        score = HighScoreManager.GetPendingHighScore();

        okButton = okButtonGameObject.GetComponent<Button>();
        okButton.onClick.AddListener(OnOKButtonClick);

        nameInput = nameInputGameObject.GetComponent<TMP_InputField>();
        scoreLabel = scoreLabelGameObject.GetComponent<TextMeshProUGUI>();

        scoreLabel.text = HighScoreManager.GetScoreFormattedString(score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(IGameObjectOwner owner)
    {
        this.owner = owner;
    }

    private void OnOKButtonClick()
    {
        HighScoreManager.AddScore(nameInput.text, score);

        HighScoreManager.ClearPendingHighScore();

        //Destroy(this.gameObject); 
        

        if (owner != null)
        {
            owner.GameObjectDestroyed(this.gameObject);
            owner = null;
        }
    }

    [SerializeField] GameObject okButtonGameObject;
    [SerializeField] GameObject nameInputGameObject;
    [SerializeField] GameObject scoreLabelGameObject;

    private Button okButton;
    private TMP_InputField nameInput;
    private TextMeshProUGUI scoreLabel;
    private uint score;
    private IGameObjectOwner owner;
}
