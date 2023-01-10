using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableChange : Interactive
{
    public GameObject TableWithout, TableWith;

    public override void OnInteraction()
    {
        TableWithout.SetActive(false);
        TableWith.SetActive(true);
    }
}