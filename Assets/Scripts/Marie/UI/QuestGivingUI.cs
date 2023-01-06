using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGivingUI : MonoBehaviour
{
    public static QuestGivingUI Instance;
    
    [SerializeField] private GameObject questPanel; 
    [SerializeField] private TextMeshProUGUI title, description, reward;
    [SerializeField] private Button accept, later;

    private QuestData currentQuest;
    
    void Start()
    {
        
    }

    void SetupQuest(QuestData quest)
    {
        currentQuest = quest;
        //Setting up the text components of the UI
    }

    void AcceptQuest()
    {
        //Add to quest list
        //Check to NPC that quest is activated
        questPanel.SetActive(false);
    }

    void RefuseQuest()
    {
        questPanel.SetActive(false);
    }
}
