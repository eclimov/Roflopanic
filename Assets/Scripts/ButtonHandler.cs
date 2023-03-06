using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Clicked()
    {
        AudioManager.instance.PlayButtonSound();
    }
}
