using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonHandler : MonoBehaviour
{
    public GameObject helpPanelPrefab;
    public Dialogue dialogue;

    // NOTE: do not remove this intermediate method, because this handler MUST return void
    public void OnClick()
    {
        SpawnDialogue();
    }

    // NOTE: do not modify signature of this method, because it is referenced in GuideManager gameobject GUI
    public GameObject SpawnDialogue()
    {
        GameObject obj = Instantiate(helpPanelPrefab, GameObject.Find("Canvas UI").transform, false);

        TriggerDialogue();

        return obj;
    }

    public void TriggerDialogue()
    {
        FindAnyObjectByType<HelpDialogueManager>().StartDialogue(dialogue);
    }
}
