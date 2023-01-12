using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnAxe : MonoBehaviour
{
    public GameObject womanHand, manHand, axe;
    public QuestItem axeItem;
    public bool axeInInventory = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void CheckAxe()
    {
        if (Inventory.Instance.HasEvery(axeItem)
        {
            Instantiate(axe, womanHand.transform);
            Instantiate(axe, manHand.transform);
            Destroy(this);
        }
        else
        {
            Invoke("CheckAxe", 1.0f);
        }
    }
}
