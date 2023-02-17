using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimbScript : MonoBehaviour
{
    public float open = 100f;
    public float range = 0.5f;
    public bool TouchingWall = false;
    public float UpwardSpeed = 3.3f;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();

        if(InputAction)
    }
}
