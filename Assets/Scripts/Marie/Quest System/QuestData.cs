using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Quest System/Quest")]
public class QuestData : ScriptableObject
{
    public string title, description, shortDescription;
    public List<QuestItem> requirements = new List<QuestItem>();

    [Header("Rewards")]
    public int moneyReward;
    public bool hasItemReward;
    public QuestItem itemReward;
}


