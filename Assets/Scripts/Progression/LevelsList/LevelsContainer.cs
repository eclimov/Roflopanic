using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsContainer : MonoBehaviour
{
	public GameObject levelPrefab;

	private GameObject levelGameObject;

	// Start is called before the first frame update
	void Start()
	{
		for(int i = 0; i < SettingsManager.instance.levels.Length; i++)
        {
			levelGameObject = Instantiate(levelPrefab, GameObject.Find("LevelsContainer").transform, false);
			levelGameObject.GetComponent<LevelHandler>().InitializeLevel(SettingsManager.instance.levels[i]);
		}
	}
}
