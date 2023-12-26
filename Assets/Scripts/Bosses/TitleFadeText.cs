using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class TitleFadeText : MonoBehaviour
{
    [SerializeField] private LocalizedString localBossVictory;
    [SerializeField] private LocalizedString localVersusVictory;
    [SerializeField] private AudioClip sound;

    public enum TitleTextType
    {
        Boss,
        Versus
    }

    private TMP_Text titleText;

    private Vector3 newScale = new Vector3(1.2f, 1.2f, 1);
    private float scalingSpeed = .5f;

    private void Awake()
    {
        titleText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0); // Keep this in "Start", so it could be possible to set color dynamically
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, scalingSpeed * Time.deltaTime);
    }

    public void SetColor(Color32 color)
    {
        titleText.color = color;
    }

    public void SetText(string name, TitleTextType type)
    {
        string textValue = "";
        if(type == TitleTextType.Boss)
        {
            localBossVictory.Arguments = new object[] { name };
            textValue = localBossVictory.GetLocalizedString();
        }
        if(type == TitleTextType.Versus)
        {
            localVersusVictory.Arguments = new object[] { name };
            textValue = localVersusVictory.GetLocalizedString();
        }
        titleText.text = textValue;
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
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
        while (titleText.color.a < 1.0f)
        {
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, titleText.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    private IEnumerator FadeOut(float t)
    {
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 1);
        while (titleText.color.a > 0.0f)
        {
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, titleText.color.a - (Time.deltaTime / t));
            yield return null;
        }

        Destroy(gameObject);
    }
}
