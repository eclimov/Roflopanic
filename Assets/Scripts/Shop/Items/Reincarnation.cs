using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reincarnation : AbstractShopItem
{
    protected override bool CanBeEquipped()
    {
        return IsPurchased();
    }

    protected override void Equip()
    {
        SettingsManager.instance.ToggleEquipAbility(shopItemContainerData.itemData.itemId);
    }

    protected override bool IsEquipped()
    {
        return SettingsManager.IsAbilityEquipped(shopItemContainerData.itemData.itemId);
    }

    protected override bool IsPurchased()
    {
        return SettingsManager.IsAbilityPurchased(shopItemContainerData.itemData.itemId);
    }

    protected override void PurchaseItem()
    {
        DestroyConfirmPurchasePanel();

        if (SettingsManager.instance.SubtractScore(shopItemContainerData.itemData.itemPrice))
        {
            SettingsManager.PurchaseAbility(shopItemContainerData.itemData.itemId);
            AudioManager.instance.PlayCashSound();

            CheckIfCanBePurchased();
        }
    }
}
