using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    
    private List<ItemData> items = new List<ItemData>();
    
    [SerializeField] private List<QuestItem> usableItems;
    [SerializeField] private Transform hand;

    private int objectInHand = -1;

    private void Start()
    {
        Instance = this;
    }

    public void RemoveFromInventory(ItemData keyItem)
    {
        if (items.Contains(keyItem))
        {
            items.Remove(keyItem);
            int place = FindItemInList(keyItem.UID);
            if (place != -1)
            {
                QuestItem item = usableItems[place];
                usableItems.RemoveAt(place);
                Destroy(item.gameObject);
            }
        }
    }
    public void PickupKeyItem(ItemData questItem)
    {
        if (!items.Contains(questItem))
        {
            GameObject keyInstance = Instantiate(questItem.prefab, hand);
            items.Add(questItem);
            usableItems.Add(keyInstance.GetComponent<QuestItem>());
            //Utilise le dernier trouvé
            HoldItem(usableItems.Count-1);
        }
    }

    private int FindItemInList(Guid id)
    {
        for (int item = 0; item < usableItems.Count; item++)
        {
            if (usableItems[item].GetComponent<QuestItem>().data.UID == id)
            {
                return item;
            }
        }
        
        //Easier method would be
        //return usableItems.FindIndex(item => item.GetComponent<KeyItem>()?.ID == id);

        return -1;
    }

    private void HoldItem(int number)
    {
        if (number < -1 || number >= usableItems.Count)
        {
            //Index out of bounds, nothing can be done
            return;
        }
        if (objectInHand != -1)
        {
            usableItems[objectInHand].gameObject.SetActive(false);
        }

        objectInHand = number;
        usableItems[objectInHand].gameObject.SetActive(true);
    }

    public void NextItem()
    {
        if (usableItems.Count != 0)
        {
            int next = ((objectInHand+1)%usableItems.Count)-1;
            HoldItem(next);
        }
    }

    public bool IsItemFound(ItemData key)
    {
        //Empty ID means no necessary key item
        //true if the id is found in the list, false otherwise
        return key==null|| items.Contains(key);
    }

    public bool HasEveryItem(List<ItemData> keys)
    {
        foreach (ItemData key in keys)
        {
            if (!IsItemFound(key))
            {
                return false;
            }
        }

        return true;
    }
}
