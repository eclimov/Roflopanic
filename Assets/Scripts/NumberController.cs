using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberController : MonoBehaviour
{
    private TMP_Text textComp;

    protected int currentValue = 0;
    protected int targetValue = 0;

    protected float increaseValue;
    protected bool isSmoothIncrement;

    private float maxDeltaMultiplier;

    private void Start()
    {
        textComp = GetComponent<TMP_Text>();
    }

    protected void UpdateValueText(int value)
    {
        textComp.text = value.ToString();
    }

    private void Update()
    {
        if (isSmoothIncrement)
        {
            if (targetValue != currentValue)
            {
                increaseValue = Mathf.MoveTowards(increaseValue, targetValue, Time.unscaledDeltaTime * maxDeltaMultiplier);
                currentValue = (int)increaseValue;
                UpdateValueText(currentValue);
            }
            else
            {
                isSmoothIncrement = false;
            }
        }
    }

    public void SetNumberValue(int newValue)
    {
        targetValue = newValue;
        isSmoothIncrement = true;

        maxDeltaMultiplier = Mathf.Abs(targetValue - currentValue);
    }
}
