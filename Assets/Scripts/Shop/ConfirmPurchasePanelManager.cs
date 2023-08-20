using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ConfirmPurchasePanelManager : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;

    public Image itemCurrencyImage;
    public TextMeshProUGUI itemPriceValueText;

    public Animator dialogueBoxAnimator;
    public Animator dialogueCharacterAnimator;

    public delegate void OnCancelDelegate();
    public event OnCancelDelegate OnCancel;

    public delegate void OnOkDelegate();
    public event OnOkDelegate OnOk;

    protected void Awake()
    {
        itemNameText.text = "";

        dialogueBoxAnimator.SetBool("IsOpen", true); // On created, start the animation
        dialogueCharacterAnimator.SetBool("IsOpen", true);
    }

    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    public void SetData(ShopItemData itemData)
    {
        itemImage.sprite = itemData.GetItemSprite();
        itemNameText.text = itemData.GetItemName();

        itemCurrencyImage.sprite = itemData.GetItemCurrencySprite();
        itemPriceValueText.text = itemData.GetItemPriceValueText();
    }

    public void Cancel()
    {
        if (OnCancel != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnCancel();
        }
    }

    public void Ok()
    {
        if (OnOk != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnOk();
        }
    }
}
