using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : AbstractShopItem
{
    protected override bool CanBeEquipped()
    {
        return IsPurchased();
    }

    protected override void Equip()
    {
        SettingsManager.instance.SetBackground(shopItemContainerData.itemData.itemId);
    }

    protected override bool IsEquipped()
    {
        return SettingsManager.GetBackground() == shopItemContainerData.itemData.itemId;
    }

    protected override bool IsPurchased()
    {
        return SettingsManager.IsBackgroundPurchased(shopItemContainerData.itemData.itemId);
    }

    protected override void PurchaseItem()
    {
        DestroyConfirmPurchasePanel();

        if (SettingsManager.instance.SubtractScore(shopItemContainerData.itemData.itemPrice))
        {
            SettingsManager.PurchaseBackground(shopItemContainerData.itemData.itemId);
            AudioManager.instance.PlayCashSound();

            CheckIfCanBePurchased();
        }
    }
}
