using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestGivingUI : MonoBehaviour
{
    public static QuestGivingUI Instance;
    
    [SerializeField] private GameObject questPanel, thankYouPanel; 
    [SerializeField] private TextMeshProUGUI title, description, reward;
    [SerializeField] private Button accept, later, welcome;
    private QuestNpc npc;

    private QuestData currentQuest;
    
    void Start()
    {
        if (Instance) Destroy(this);
        else Instance = this;

        accept.onClick.AddListener(AcceptQuest);
        later.onClick.AddListener(RefuseQuest);

        welcome.onClick.AddListener(delegate
        {
            thankYouPanel.SetActive(false);
            Time.timeScale = 1;
        });
    }

    public void SetupQuest(QuestData quest, QuestNpc giver)
    {
        Time.timeScale = 0;
        currentQuest = quest;
        npc = giver;
        questPanel.SetActive(true);
        title.text = quest.title;
        description.text = quest.description;
        accept.Select();
        //Setting up the text components of the UI
    }

    void AcceptQuest()
    {
        //Check to NPC that quest is activated
        questPanel.SetActive(false);
        if(npc != null)
        {
            npc.GiveQuest();
        }
        Time.timeScale = 1;
    }

    void RefuseQuest()
    {
        questPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ThankYou()
    {
        thankYouPanel.SetActive(true);
        Time.timeScale = 0;
        welcome.Select();
    }
}
