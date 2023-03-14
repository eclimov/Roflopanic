using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

public class GameOverText : MonoBehaviour
{
    [SerializeField] private LocalizedString gameOverHighscoreText;
    [SerializeField] private LocalizedString gameOverText;
    [SerializeField] private TextMeshProUGUI textComp;

    private void OnEnable()
    {
        if(GameOver.isHighscore)
        {
            textComp.text = gameOverHighscoreText.GetLocalizedString();
        } else
        {
            textComp.text = gameOverText.GetLocalizedString();
        }
    }

    private void Update()
    {
        if(GameOver.isHighscore)
        {
            GetComponent<TMP_Text>().color = Color.Lerp(Color.black, Color.yellow, Mathf.PingPong(Time.time / .5f, 1));
        }
    }
}
