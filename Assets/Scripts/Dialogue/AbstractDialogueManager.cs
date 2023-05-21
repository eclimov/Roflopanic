using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class AbstractDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;

    public Animator dialogueBoxAnimator;
    public Animator dialogueCharacterAnimator;

    protected Queue<string> sentences = new Queue<string>();

    private bool isTextTyped;
    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    protected void Awake()
    {
        dialogueText.text = "";
        cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(.04f);

        dialogueBoxAnimator.SetBool("IsOpen", true); // On created, start the animation
    }

    protected void Update()
    {
        if (!isTextTyped)
        {
            if (dialogueBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("DialogueBox_Open"))
            {
                dialogueCharacterAnimator.SetBool("IsOpen", true);

                if (dialogueCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("DialogueCharacter_Open"))
                {
                    DisplayNextSentence();
                    isTextTyped = true;
                }
            }
        }
    }

    protected IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            // play Undertale-like sound( https://www.youtube.com/watch?v=-HEhl8kq1rA ). PLAY IT FOR LETTERS ONLY, skip for spaces and punctuation marks.
            if (char.IsLetter(letter))
            {
                AudioManager.instance.PlayVoiceLetterSound();
            }
            yield return cachedWaitForSecondsRealtime;
            // yield return null; // Wait 1 frame

            dialogueText.text += letter;
        }
    }

    public abstract void StartDialogue(Dialogue dialogue);

    protected abstract void DisplayNextSentence();
}
