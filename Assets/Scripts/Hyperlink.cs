using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hyperlink : MonoBehaviour, IPointerClickHandler
{
    public string url;

    // https://www.spongehammer.com/link-on-the-ui-using-textmesh-pro-in-unity/
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(url);
    }
}
