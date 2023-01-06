using System.Collections.Generic;
using UnityEngine;
public class QuestNpc : Interactive
{
    public List<QuestData> quests = new List<QuestData>();
    private bool gaveQuest = false;
    private int current = 0;

    public override void OnInteraction()
    {
        if (gaveQuest) ThanksMessage();
        else GiveQuest();
        PlayerInteraction.Instance.StopInteractive();
    }


    void GiveQuest()
    {
        if (quests.Count > 0 && current < quests.Count)
        {
            QuestGivingUI.Instance.SetupQuest(quests[current]);
            
            gaveQuest = true;
            waitForObject = true;
            //Setting up requirements to finish quests
        }
    }

    void ThanksMessage()
    {
        Debug.Log("Thanks");
        FinishQuest();
    }

    void FinishQuest()
    {
        //Dialogue end quest

        current++;
        gaveQuest = false;
        if(current == quests.Count)
        {
            Destroy(this);
        }
    }
}
