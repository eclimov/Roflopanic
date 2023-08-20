using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    public string itemId;

    private Sprite itemSprite;

    private string itemName;

    [SerializeField]
    public int itemPrice;

    private string itemPriceText;

    private Sprite itemCurrencySprite;

    public string GetItemName()
    {
        return itemName;
    }

    public void SetItemName(string name)
    {
        itemName = name;
    }

    public Sprite GetItemSprite()
    {
        return itemSprite;
    }

    public void SetItemSprite(Sprite sprite)
    {
        itemSprite = sprite;
    }

    public string GetItemPriceValueText()
    {
        return itemPriceText;
    }

    public void SetItemPriceValueText(string text)
    {
        itemPriceText = text;
    }

    public Sprite GetItemCurrencySprite()
    {
        return itemCurrencySprite;
    }

    public void SetItemCurrencySprite(Sprite sprite)
    {
        itemCurrencySprite = sprite;
    }
}
