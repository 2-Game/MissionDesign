using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGL_3B : QuestNpc
{
    private bool gaveQuest = false;
    private int current = 0;

    public void GiveQuest2()
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
            QuestManager.Instance.TakeQuest(quests[current]);
        }
    }
}
