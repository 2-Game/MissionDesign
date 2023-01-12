using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGL_Bofor : QuestNpc
{
    [SerializeField] private ItemData _ingot;
    [SerializeField] private Collider _anvilCollider;

    public override void GiveQuest()
    {
        if (quests.Count > 0 && current < quests.Count)
        {
            gaveQuest = true;
            waitForObject = true;
            //Setting up requirements to finish quests
            foreach (QuestItem item in quests[current].requirements)
            {
                requiredItems.Add(item);
            }
            if (current == 0)
            {
                Inventory.Instance.AddToInventory(_ingot, 1);
                _anvilCollider.enabled = true;
            }
            QuestManager.Instance.TakeQuest(quests[current]);
        }
    }
}
