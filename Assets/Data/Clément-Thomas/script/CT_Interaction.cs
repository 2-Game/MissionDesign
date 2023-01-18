using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CT_Interaction : QuestNpc
{

    public SphereCollider npc1;
    public ItemData item;


   
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
                npc1.enabled = true;
                
            }
            else
            {
                npc1.enabled = false;
            }
            QuestManager.Instance.TakeQuest(quests[current]);
        }
    }


}
