using System.Collections.Generic;
using UnityEngine;
public class C_SellerMan : Interactive
{
    public int necessaryGold;
    public QuestItem soldItem;

    public override void OnInteraction()
    {
        if (Wallet.Instance.CanSpend(necessaryGold))
        {
            Wallet.Instance.SpendMoney(necessaryGold);
            Inventory.Instance.AddToInventory(soldItem.item, soldItem.quantity);
            QuestManager.Instance.Notify();
        }
        else
        {
            PlayerInteraction.Instance.SetInteraction(InteractionType.FailedAction);
        }
    }


}
