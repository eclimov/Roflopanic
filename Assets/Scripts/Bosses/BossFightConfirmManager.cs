using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossFightConfirmManager : MonoBehaviour
{
    public TMP_Text bossNameText;

    public TMP_Text dialogueText;
    public Animator dialogueBoxAnimator;
    public Animator bossCharacterAnimator;
    public Image bossImage;
    public NumberController bossRewardNumberController;
    public AudioClip bossVoiceLetterSound;

    private bool isTextTyped;
    private WaitForSecondsRealtime cachedWaitForSecondsRealtime;

    private string sentence;
    private int rewardValue;
    private AbstractBossGameManager bossGameManagerPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        cachedWaitForSecondsRealtime = new WaitForSecondsRealtime(.04f);

        dialogueText.text = "";
        dialogueBoxAnimator.SetBool("IsOpen", true); // On created, start the animation

        Time.timeScale = 0f;
        AudioManager.instance.PauseMusic();
    }

    public void SetRewardValue(int reward)
    {
        rewardValue = reward;

        SetRewardValueText(rewardValue);
    }

    private void SetRewardValueText(int reward)
    {
        bossRewardNumberController.SetNumberValue(reward);
    }

    public void SetBossImage(Sprite sprite)
    {
        bossImage.sprite = sprite;
    }

    public void SetBossGameManager(AbstractBossGameManager gm)
    {
        bossGameManagerPrefab = gm;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        bossNameText.text = dialogue.GetCharacterName();
        sentence = dialogue.GetSentences()[0]; // There should be only one sentence for boss dialogue
    }

    protected void Update()
    {
        if (!isTextTyped)
        {
            if (dialogueBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("DialogueBox_Open"))
            {
                bossCharacterAnimator.SetBool("IsOpen", true);

                if (bossCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("BossCharacter_Open"))
                {
                    StartCoroutine(TypeSentence(sentence));
                    isTextTyped = true;
                }
            }
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            // play Undertale-like sound( https://www.youtube.com/watch?v=-HEhl8kq1rA ). PLAY IT FOR LETTERS ONLY, skip for spaces and punctuation marks.
            if (char.IsLetter(letter))
            {
                AudioManager.instance.PlaySound(bossVoiceLetterSound);
            }
            yield return cachedWaitForSecondsRealtime;
            // yield return null; // Wait 1 frame

            dialogueText.text += letter;
        }
    }

    public void ConfirmFight()
    {
        StartCoroutine(ClosePanel(true));
    }

    public void Escape()
    {
        StartCoroutine(ClosePanel(false));
    }

    public IEnumerator DoubleDown()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SetRewardValue(rewardValue * 2);
        AudioManager.instance.PlayCoinSound();
    }

    public IEnumerator ClosePanel(bool isFightConfirmed)
    {
        bossCharacterAnimator.SetBool("IsOpen", false);
        dialogueBoxAnimator.SetBool("IsOpen", false);

        yield return new WaitForSecondsRealtime(.5f);

        if(isFightConfirmed)
        {
            FindObjectOfType<GameManager>().StartBossFight(bossGameManagerPrefab, bossNameText.text, rewardValue);
        }

        Time.timeScale = 1f;
        AudioManager.instance.UnpauseMusic();
        Destroy(gameObject);
    }
}
