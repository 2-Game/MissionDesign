using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder1 : MonoBehaviour

{

    public Transform chController;
    bool inside = false;
    public float speedUpDown;
    public PlayerController FPSInput;

    void Start()
    {
        FPSInput = GetComponent<PlayerController>();
        inside = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            FPSInput.enabled = false;
            inside = !inside;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            FPSInput.enabled = true;
            inside = !inside;
        }
    }

    void Update()
    {
        if (inside == true && Input.GetKey("w"))
        {
            chController.transform.position += Vector3.up / speedUpDown;
        }

        if (inside == true && Input.GetKey("s"))
        {
            chController.transform.position += Vector3.down / speedUpDown;
        }
    }

}