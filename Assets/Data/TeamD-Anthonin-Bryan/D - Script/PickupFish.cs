using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFish : Interactive
{
    public GameObject Fish1, Fish2, PopUpFish;
    public ItemData ItemData;
    public override void OnInteraction()
    {
        Fish1.SetActive(false);
        Fish2.SetActive(false);
        Inventory.Instance.AddToInventory(ItemData, 4);
        QuestManager.Instance.Notify();
        PopUpFish.SetActive(true);
    }
}
