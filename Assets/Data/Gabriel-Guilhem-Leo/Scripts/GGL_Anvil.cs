using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGL_Anvil : Interactive
{
    [SerializeField] private ItemData _blade;
    [SerializeField] private Collider _anvilCollider;

    public override void OnInteraction()
    {
        if (onlyOnce)
        {
            Inventory.Instance.RemoveFromInventory(requiredItems[0].item, 1);
            Inventory.Instance.AddToInventory(_blade);
            _anvilCollider.enabled = false;
        }
    }
}
