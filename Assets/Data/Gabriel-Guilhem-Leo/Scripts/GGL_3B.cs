using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGL_3B : QuestNpc
{
    public ItemData _data;

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
            if (current == 3)
            {
                Inventory.Instance.AddToInventory(_data,3);
            }
            QuestManager.Instance.TakeQuest(quests[current]);
        }
    }
}