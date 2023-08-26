using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinChanceUpgrade : AbstractShopItem
{
    protected override bool IsPurchased()
    {
        return SettingsManager.GetCoinChance() >= 100;
    }

    protected override void PurchaseItem()
    {
        DestroyConfirmPurchasePanel();

        if(SettingsManager.instance.SubtractScore(shopItemContainerData.itemData.itemPrice))
        {
            SettingsManager.instance.AddCoinChance(1);
            AudioManager.instance.PlayCoinSound();

            CheckIfCanBePurchased();
        }
    }

    protected override bool CanBeEquipped()
    {
        return false;
    }

    protected override bool IsEquipped()
    {
        return false;
    }

    protected override void Equip()
    {
        throw new System.NotImplementedException();
    }
}
