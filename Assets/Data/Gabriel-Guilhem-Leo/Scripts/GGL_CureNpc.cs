using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GGL_CureNpc : Interactive
{
    public Collider interactionCollider;
    [SerializeField] private GameObject thankYouPanel;
    [SerializeField] private TextMeshProUGUI thankYou;
    [SerializeField] private Button welcome;
    [SerializeField] private string thankYouMessage;

    private void Start()
    {
        welcome.onClick.AddListener(delegate
        {
            thankYouPanel.SetActive(false);
            Time.timeScale = 1;
        });
    }
    public override void OnInteraction()
    {
        if (Inventory.Instance.IsItemFound(requiredItems[0].item))
        {
            Inventory.Instance.RemoveFromInventory(requiredItems[0].item);
            if (onlyOnce) interactionCollider.enabled = false;
        }
    }

    public void ThankYou()
    {
        thankYou.text = thankYouMessage;
        thankYouPanel.SetActive(true);
        Time.timeScale = 0;
        welcome.Select();
    }
}
