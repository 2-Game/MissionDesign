using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustKnifePopUp : Interactive
{
    public GameObject RustyKnife, PopUpRustyKnife;
    public ItemData ItemData;

    public override void OnInteraction()
    {
        RustyKnife.SetActive(false);
        Inventory.Instance.AddToInventory(ItemData, 1);
        PopUpRustyKnife.SetActive(true);
        
    }
}
