using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeleteDataHandler : MonoBehaviour
{
    public GameObject confirmPanelPrefab;
    public Dialogue dialogue;

    private GameObject confirmPanel;
    private ConfirmPanelManager confirmPanelManager;

    public void SpawnDialogue()
    {
        confirmPanel = Instantiate(confirmPanelPrefab, GameObject.Find("Canvas").transform, false);
        confirmPanelManager = confirmPanel.GetComponent<ConfirmPanelManager>();
        confirmPanelManager.StartDialogue(dialogue);

        confirmPanelManager.OnCancel += Cancel;
        confirmPanelManager.OnOk += DeleteData;
    }

    private void OnDisable()
    {
        if(confirmPanelManager != null)
        {
            confirmPanelManager.OnCancel -= Cancel;
            confirmPanelManager.OnOk -= DeleteData;
        }
    }

    private void Cancel()
    {
        Destroy(confirmPanel);
    }

    private void DeleteData()
    {
        Destroy(confirmPanel);
        SettingsManager.instance.DeleteData();
    }
}
