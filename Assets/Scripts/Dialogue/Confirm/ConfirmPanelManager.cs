using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanelManager : AbstractDialogueManager
{
    public delegate void OnCancelDelegate();
    public event OnCancelDelegate OnCancel;

    public delegate void OnOkDelegate();
    public event OnOkDelegate OnOk;

    public override void StartDialogue(Dialogue dialogue)
    {
        characterNameText.text = dialogue.GetCharacterName();

        sentences.Clear();
        foreach (string sentence in dialogue.GetSentences())
        {
            sentences.Enqueue(sentence);
        }
    }

    protected override void DisplayNextSentence()
    {
        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // Make sure to stop typing previous sentence (if any)
        StartCoroutine(TypeSentence(sentence));
    }

    public void Cancel()
    {
        if (OnCancel != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnCancel();
        }
    }

    public void Ok()
    {
        if (OnOk != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnOk();
        }
    }
}
