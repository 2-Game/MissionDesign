using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGL_CureNpc : Interactive
{
    public Collider interactionCollider;
    public override void OnInteraction()
    {
        if (Inventory.Instance.IsItemFound(requiredItems[0].item))
        {
            Inventory.Instance.RemoveFromInventory(requiredItems[0].item);
            if (onlyOnce) interactionCollider.enabled = false;
        }
    }
}
