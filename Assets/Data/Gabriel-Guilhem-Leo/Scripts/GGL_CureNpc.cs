using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGL_CureNpc : Interactive
{
    public ItemData needItem;
    public override void OnInteraction()
    {
        if (Inventory.Instance.IsItemFound(needItem))
        {
            Inventory.Instance.RemoveFromInventory(needItem);
        }
    }
}
