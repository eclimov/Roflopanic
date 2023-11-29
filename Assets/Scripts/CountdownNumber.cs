using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownNumber : MonoBehaviour
{
    private TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartCoroutine(DestroyDelayed(1.1f));
    }

    private IEnumerator DestroyDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void SetText(string text)
    {
        tmpText.text = text;
    }

    public void SetColor(Color32 color)
    {
        tmpText.color = color;
    }
}
