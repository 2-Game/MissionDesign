using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateform : MonoBehaviour
{
    public static Plateform instance;
    public bool isUp, isDown;
    // Start is called before the first frame update
    void Start()
    {
        if (instance) Destroy(this);
        else instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Up")
        {
            Debug.Log("IsUp");
            isUp = true;
        }
        else if(other.gameObject.tag == "Down")
        {
            Debug.Log("IsDown");
            isDown = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Up")
        {
            isUp = false;
        }
        else if(other.gameObject.tag == "Donw")
        {
            isDown = false;
        }
    }

    public void StopAnimation()
    {
        isDown=false;
        isUp=false;
    }
}
