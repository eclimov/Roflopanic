using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersusPlayerChoiceText : MonoBehaviour
{
    public VersusController playerChoiceController;
    public string playerName;
    public Color32 panelPlayerTextColor;

    private Color32 panelAiTextColor = new(176, 176, 176, 255); // Gray
    private TMP_Text panelText;

    // Start is called before the first frame update
    void Start()
    {
        panelText = GetComponent<TMP_Text>();
        UpdateText(VersusGameManager.ActorTypesEnum.AI);

        playerChoiceController.OnPlayerTypeChanged += UpdateText;
    }

    private void OnDestroy()
    {
        playerChoiceController.OnPlayerTypeChanged -= UpdateText;
    }

    private void UpdateText(VersusGameManager.ActorTypesEnum actorType)
    {
        if (actorType == VersusGameManager.ActorTypesEnum.AI)
        {
            panelText.text = "AI";
            panelText.color = panelAiTextColor;
        }
        else
        {
            panelText.text = playerName;
            panelText.color = panelPlayerTextColor;
        }
    }
}
