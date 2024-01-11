using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI difficultyNameText;
    [SerializeField] private LocalizedString[] difficulties; // Must have the same length as number of available difficulties

    [SerializeField] private GameObject gameObjectPrevious;
    [SerializeField] private GameObject gameObjectNext;

    private Button buttonPrevious;
    private Button buttonNext;

    private Image buttonPreviousSprite;
    private Image buttonNextSprite;

    // Start is called before the first frame update
    void Start()
    {
        buttonPrevious = gameObjectPrevious.GetComponent<Button>();
        buttonPreviousSprite = gameObjectPrevious.GetComponentInChildren<Image>();

        buttonNext = gameObjectNext.GetComponent<Button>();
        buttonNextSprite = gameObjectNext.GetComponentInChildren<Image>();

        SetDifficulty(SettingsManager.difficultyId);
    }

    public void SetPreviousDifficulty()
    {
        if(SettingsManager.difficultyId > 0)
        {
            SetDifficulty(SettingsManager.difficultyId - 1);
        }
    }

    public void SetNextDifficulty()
    {
        if (SettingsManager.difficultyId < (difficulties.Length - 1))
        {
            SetDifficulty(SettingsManager.difficultyId + 1);
        }
    }

    private void SetDifficulty(int index)
    {
        var tempColorPrev = buttonPreviousSprite.color;
        var tempColorNext = buttonNextSprite.color;

        switch (index)
        {
            case 0:
                buttonPrevious.interactable = false;
                tempColorPrev.a = .6f;
                buttonPreviousSprite.color = tempColorPrev;

                difficultyNameText.color = new Color32(154, 255, 97, 255);
                break;
            case 1: // Reset opacity for both buttons
                buttonPrevious.interactable = true;
                tempColorPrev.a = 1f;
                buttonPreviousSprite.color = tempColorPrev;

                buttonNext.interactable = true;
                tempColorNext.a = 1f;
                buttonNextSprite.color = tempColorNext;

                difficultyNameText.color = new Color32(255, 252, 97, 255);
                break;
            case 2:
                buttonNext.interactable = false;
                tempColorNext.a = .6f;
                buttonNextSprite.color = tempColorNext;

                difficultyNameText.color = new Color32(255, 97, 111, 255);
                break;
            default:
                break;
        }

        SettingsManager.instance.SetDifficultyId(index);
        difficultyNameText.text = difficulties[index].GetLocalizedString();
    }
}
