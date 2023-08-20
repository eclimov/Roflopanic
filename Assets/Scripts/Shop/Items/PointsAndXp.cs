using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsAndXp : AbstractShopItem
{
    protected override bool CanBeEquipped()
    {
        return false;
    }

    protected override bool CanBePurchased()
    {
        return true;
    }

    protected override void Equip()
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsEquipped()
    {
        return false;
    }

    protected override bool IsPurchased()
    {
        return false;
    }

    protected override void PurchaseItem()
    {
        DestroyConfirmPurchasePanel();

        if (true)
        {
            SettingsManager.instance.AddScore(10_000);
            SettingsManager.instance.AddExperience(10_000);
            AudioManager.instance.PlayCashSound();

            InitializeStyles();
        }
    }
}
