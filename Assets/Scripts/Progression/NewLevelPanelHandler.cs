using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;

public class NewLevelPanelHandler : MonoBehaviour
{
    public Animator NewLevelBoxAnimator;
    public Animator GlintAnimator;

    public LocalizedString levelTitle;
    public TMP_Text levelTitleText;

    public TMP_Text levelNameText;
    public TMP_Text levelSloganText;
    public TMP_Text levelPerksText;
    public Image levelImage;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        AudioManager.instance.PauseMusic();

        StartCoroutine(OpenNewLevelBoxWithADelay());
    }

    public void InitializeLevel(Level level)
    {
        levelTitle.Arguments = new object[] { level.GetNumber() };
        levelTitleText.text = levelTitle.GetLocalizedString();

        levelNameText.text = level.GetName();
        levelSloganText.text = level.GetSlogan();
        levelPerksText.text = level.GetPerks();
        levelImage.sprite = level.GetSprite();
    }

    protected IEnumerator OpenNewLevelBoxWithADelay()
    {
        yield return new WaitForSecondsRealtime(.3f);
        NewLevelBoxAnimator.SetBool("IsOpen", true); // On created, start the animation

        yield return new WaitForSecondsRealtime(2f);
        GlintAnimator.SetBool("IsMoving", true);
    }

    public void ClosePanel()
    {
        Time.timeScale = 1f;
        AudioManager.instance.UnpauseMusic();
        Destroy(gameObject);
    }
}
