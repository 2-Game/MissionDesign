using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlate : Interactive
{
    public GameObject RepasNappe;
    public ItemData ItemData;
    public override void OnInteraction()
    {
        RepasNappe.SetActive(true);
        Inventory.Instance.AddToInventory(ItemData, 1);
        QuestManager.Instance.Notify();
        Inventory.Instance.RemoveFromInventory(requiredItems[0].item, 1);
    }
}