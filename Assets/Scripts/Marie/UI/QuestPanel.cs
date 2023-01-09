using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    public TextMeshProUGUI title, progress;
    public Sprite completeQuest;
    private QuestData trackedQuest;
    private int max = 0;

    public void SetupQuest(QuestData quest)
    {
        trackedQuest = quest;
        title.text = quest.title;
        SetTotalRequirements();
        progress.text = " 0/" + quest.requirements;
    }

    public void Notify()
    {
        foreach (QuestItem item in trackedQuest.requirements)
        {
            if(Inventory.Instance.items.Contains(item)) max += item.quantity;
        }
    }

    public void SetTotalRequirements()
    {
        foreach (QuestItem item in trackedQuest.requirements)
        {
            max += item.quantity;
        }
    }
}
