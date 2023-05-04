using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;

    public Animator dialogueBoxAnimator;
    public Animator dialogueCharacterAnimator;
    public GameObject characterAltMouthSprite;
    public GameObject buttonNext;

    private Queue<string> sentences;
    private bool isTextTyped;
    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    void Awake()
    {
        sentences = new Queue<string>();
        dialogueText.text = "";
        cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(.04f);
    }

    private void Update()
    {
        if(!isTextTyped)
        {
            if(dialogueBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("DialogueBox_Open"))
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

    public void StartDialogue(Dialogue dialogue)
    {
        characterNameText.text = dialogue.GetCharacterName();

        dialogueBoxAnimator.SetBool("IsOpen", true);

        sentences.Clear();
        foreach(string sentence in dialogue.GetSentences())
        {
            sentences.Enqueue(sentence);
        }

        buttonNext.SetActive(sentences.Count > 1); // Do not show the button if there is only one sentence
    }

    public void DisplayNextSentence()
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

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
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
}
