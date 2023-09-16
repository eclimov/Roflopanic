using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : AbstractShopItem
{
    protected override bool CanBeEquipped()
    {
        return IsPurchased();
    }

    protected override void Equip()
    {
        SettingsManager.instance.SetPlayerSkin(shopItemContainerData.itemData.itemId);
    }

    protected override bool IsEquipped()
    {
        return SettingsManager.GetPlayerSkin() == shopItemContainerData.itemData.itemId;
    }

    protected override bool IsPurchased()
    {
        return SettingsManager.IsSkinPurchased(shopItemContainerData.itemData.itemId);
    }

    protected override void PurchaseItem()
    {
        DestroyConfirmPurchasePanel();

        if (SettingsManager.instance.SubtractScore(shopItemContainerData.itemData.itemPrice))
        {
            SettingsManager.PurchaseSkin(shopItemContainerData.itemData.itemId);
            AudioManager.instance.PlayCashSound();

            CheckIfCanBePurchased();
        }
    }
}
