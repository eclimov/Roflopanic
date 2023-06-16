using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelListButtonHandler : MonoBehaviour
{
    public void OpenLevelsListScene()
    {
        FindObjectOfType<LevelLoader>().LoadLevels();
    }
}
