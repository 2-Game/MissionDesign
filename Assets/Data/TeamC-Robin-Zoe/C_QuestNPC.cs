using System.Collections.Generic;
using UnityEngine;
public class C_QuestNPC : QuestNpc
{
    public int necessaryGold;

    public override void OnInteraction()
    {
        if (Wallet.Instance.CanSpend(necessaryGold))
        {
            base.OnInteraction();
        }
        else
        {
            PlayerInteraction.Instance.SetInteraction(InteractionType.FailedAction);
        }
    }


    public override void FinishQuest()
    {
        //Dialogue end quest
        Wallet.Instance.SpendMoney(necessaryGold);
        base.FinishQuest();
    }
}
