using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClownichGate : MonoBehaviour
{
    public enum typeEnum // your custom enumeration
    {
        Ally,
        Enemy,
        Cleaning
    };
    public typeEnum type = typeEnum.Ally;

    public bool IsAlly()
    {
        return type == typeEnum.Ally;
    }

    public bool IsCleaning()
    {
        return type == typeEnum.Cleaning;
    }

    public bool IsEnemy()
    {
        return type == typeEnum.Enemy;
    }

    private void Update()
    {
        if(IsEnemy())
        {
            GetComponent<Image>().color = Color.Lerp(new Color32(0, 255, 255, 90), new Color32(0, 255, 255, 255), Mathf.PingPong(Time.time / .5f, 1));
        }
    }
}
