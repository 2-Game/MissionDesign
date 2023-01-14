using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_LootTree : Interactive
{
    public QuestItem axeItem;
    public ItemData log;
    public override void OnInteraction()
    {
        if (Inventory.Instance.HasEvery(axeItem))
        {
            Inventory.Instance.AddToInventory(log, 1);
        }
    }

}
