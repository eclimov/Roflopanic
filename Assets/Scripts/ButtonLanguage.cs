using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLanguage : MonoBehaviour
{
    public ushort localeId;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            ChangeLocaleProxy(localeId);
        });
    }

    // Update is called once per frame
    void Update()
    {
        var tempColor = GetComponent<Image>().color;

        if (localeId == SettingsManager.localeId)
        {
            tempColor.a = 1f;
        }
        else
        {
            tempColor.a = .6f; // Set semi-transparency to a button
        }

        GetComponent<Image>().color = tempColor;
    }

    public void ChangeLocaleProxy(ushort localeId)
    {
        FindAnyObjectByType<SettingsManager>().ChangeLocale(localeId);
    }
}
