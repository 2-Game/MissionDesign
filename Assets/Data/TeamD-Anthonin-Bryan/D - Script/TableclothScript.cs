using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableclothScript : Interactive
{
    public GameObject TableCloth;
    public ItemData ItemData;

    public override void OnInteraction()
    {
        TableCloth.SetActive(false);
        Inventory.Instance.AddToInventory(ItemData, 2);
        QuestManager.Instance.Notify();
    }
}
