using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeModification : Interactive
{
    public GameObject Knife2;


    public override void OnInteraction()
    {
        Knife2.SetActive(true);
        Inventory.Instance.RemoveFromInventory(requiredItems[0].item);
    }
}