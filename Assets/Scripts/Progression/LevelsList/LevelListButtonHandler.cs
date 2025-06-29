using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelListButtonHandler : MonoBehaviour
{
    public void OpenLevelsListScene()
    {
        FindAnyObjectByType<LevelLoader>().LoadLevels();
    }
}
