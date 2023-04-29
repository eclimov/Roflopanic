using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class Dialogue
{
    [SerializeField] private LocalizedString characterName;
    [SerializeField] private LocalizedString[] sentences;

    public string[] GetSentences()
    {
        return Array.ConvertAll(sentences, sentence => sentence.GetLocalizedString());
    }

    public string GetCharacterName()
    {
        return characterName.GetLocalizedString();
    }
}
