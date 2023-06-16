using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[System.Serializable]
public class Level
{
    [SerializeField] private int number;
    [SerializeField] private LocalizedString name;
    [SerializeField] private LocalizedString slogan;
    [SerializeField] private LocalizedString perks;
    [SerializeField] private Sprite sprite;

    public int GetNumber()
    {
        return number;
    }

    public string GetName()
    {
        return name.GetLocalizedString();
    }

    public string GetSlogan()
    {
        return slogan.GetLocalizedString();
    }

    public string GetPerks()
    {
        if(perks.IsEmpty)
        {
            return "";
        }

        return perks.GetLocalizedString();
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}
