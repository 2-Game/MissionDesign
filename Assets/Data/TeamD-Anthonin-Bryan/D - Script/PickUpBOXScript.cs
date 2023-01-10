using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBOXScript : Interactive
{
    public GameObject Repas;

    public override void OnInteraction()
    {
        Repas.SetActive(false);
    }
}