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
        QuestManager.Instance.Notify();
        Invoke("PopUpSpawn", 0.1f);
        Invoke("PopUpDisapear", 2f);
    }

    private void PopUpDisapear()
    {
        PopUpRustyKnife.SetActive(false);
    }
    private void PopUpSpawn()
    {
        PopUpRustyKnife.SetActive(true);
    }
}
