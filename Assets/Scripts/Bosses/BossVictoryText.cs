using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class BossVictoryText : MonoBehaviour
{
    [SerializeField] private LocalizedString localBossVictory;
    [SerializeField] private AudioClip sound;

    private TMP_Text victoryText;

    private Vector3 newScale = new Vector3(1.2f, 1.2f, 1);
    private float scalingSpeed = .5f;

    private void Awake()
    {
        victoryText = GetComponent<TMP_Text>();
        victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, 0);
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, scalingSpeed * Time.deltaTime);
    }

    public void SetBossName(string name)
    {
        localBossVictory.Arguments = new object[] { name };
        victoryText.text = localBossVictory.GetLocalizedString();
    }

    public void FadeIn()
    {
        AudioManager.instance.PlaySound(sound);
        StartCoroutine(FadeIn(1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOut(1f));
    }

    // https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/#post-2472835
    private IEnumerator FadeIn(float t)
    {
        victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, 0);
        while (victoryText.color.a < 1.0f)
        {
            victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, victoryText.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    private IEnumerator FadeOut(float t)
    {
        victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, 1);
        while (victoryText.color.a > 0.0f)
        {
            victoryText.color = new Color(victoryText.color.r, victoryText.color.g, victoryText.color.b, victoryText.color.a - (Time.deltaTime / t));
            yield return null;
        }

        Destroy(gameObject);
    }
}
