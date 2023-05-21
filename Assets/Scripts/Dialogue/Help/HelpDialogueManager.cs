using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpDialogueManager : AbstractDialogueManager
{
    public GameObject characterAltMouthSprite;
    public GameObject buttonNext;

    public override void StartDialogue(Dialogue dialogue)
    {
        characterNameText.text = dialogue.GetCharacterName();

        sentences.Clear();
        foreach(string sentence in dialogue.GetSentences())
        {
            sentences.Enqueue(sentence);
        }

        buttonNext.SetActive(sentences.Count > 1); // Do not show the button if there is only one sentence
    }

    protected override void DisplayNextSentence()
    {
        if(sentences.Count == 1)
        {
            buttonNext.SetActive(false);
            characterAltMouthSprite.SetActive(true);
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // Make sure to stop typing previous sentence (if any), in case user proceeds to the next one
        StartCoroutine(TypeSentence(sentence));
    }
}
