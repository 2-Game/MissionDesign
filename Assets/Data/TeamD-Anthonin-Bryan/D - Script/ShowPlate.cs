using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlate : Interactive
{
    public GameObject RepasNappe, PNJSpawn;
    public ItemData ItemData;
    public override void OnInteraction()
    {
        RepasNappe.SetActive(true);
        Invoke("SpawnPNJ2", 2f);
        Inventory.Instance.AddToInventory(ItemData, 1);
        QuestManager.Instance.Notify();
        Inventory.Instance.RemoveFromInventory(requiredItems[0].item, 1);
    }

    private void SpawnPNJ2()
    {
        PNJSpawn.SetActive(true);
    }
}