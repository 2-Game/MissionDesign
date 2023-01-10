using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateScript : Interactive
{
    public GameObject Fish1, Fish2, PickupBOX;
    public ItemData ItemData;
    public SphereCollider colider;
    public override void OnInteraction()
    {
        Fish1.SetActive(true);
        Fish2.SetActive(true);
        PickupBOX.SetActive(true);
        Inventory.Instance.RemoveFromInventory(requiredItems[0].item, 2);
        Inventory.Instance.AddToInventory(ItemData, 1);
    }
}
