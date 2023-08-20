using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ShopItemContainerData : MonoBehaviour
{
    public ShopItemData itemData;

    public Image itemContainerImage;
    public Button itemContainerButton;
    public Animator glintAnimator;

    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public Image itemCurrencyImage;
    public TextMeshProUGUI itemPriceValueText;

    // The following represent state of the object: can be purchased, purchased, equipped
    public GameObject itemPriceContainer;
    public GameObject itemEquippedSprite;
}
