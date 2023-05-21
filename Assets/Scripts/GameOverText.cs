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

    private bool isHightscore;

    private void OnEnable()
    {
        isHightscore = FindObjectOfType<ScoreManager>().IsHighscore();

        if (isHightscore)
        {
            textComp.text = gameOverHighscoreText.GetLocalizedString();
        } else
        {
            textComp.text = gameOverText.GetLocalizedString();
        }
    }

    private void Update()
    {
        if(isHightscore)
        {
            GetComponent<TMP_Text>().color = Color.Lerp(Color.black, Color.yellow, Mathf.PingPong(Time.time / .5f, 1));
        }
    }
}
