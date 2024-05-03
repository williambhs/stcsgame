using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;
 
public class MainManager : MonoBehaviour

{

    public static MainManager Instance;
 
    private void Awake()

    {

        // start of new code
        
        if (Instance != null)

        {

            Destroy(gameObject);

            if(!SceneManager.GetActiveScene().name=="StartScreen_FinishedFinished")return;

        }

        // end of new code
 
        Instance = this;

        DontDestroyOnLoad(gameObject);

    }

}