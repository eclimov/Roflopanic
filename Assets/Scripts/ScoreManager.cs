using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public enum ScoreUpdateTypesEnum {
        ZERO, // Update disabled
        SECOND, // Update per second (+= 1 * Time.deltaTime)
        FRAME // Update per frame
    };
    public static ScoreUpdateTypesEnum scoreUpdateType = ScoreUpdateTypesEnum.SECOND;

    public Text scoreText;
    
    private float score = 0f;

    // Update is called once per frame
    void Update()
    {
        if(!(GameOver.isGameOver || PauseMenu.GameIsPaused))
        {
            switch (scoreUpdateType)
            {
                case ScoreUpdateTypesEnum.FRAME:
                    score += 10 * Time.deltaTime;
                    break;
                case ScoreUpdateTypesEnum.SECOND:
                    score += 1 * Time.deltaTime;
                    break;
                default:
                    break;
            }
        }

        scoreText.text = ((int)score).ToString();
    }
}
