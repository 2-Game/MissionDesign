using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpringArm : MonoBehaviour
{
    #region Rotation Settings
    [Space]
    [Header("Rotation Settings \n")]
    [Space]

    [SerializeField] private bool useControlRotation = true;
    [SerializeField] private float mouseSensitivity = 500f;

    private float pitch;
    private float yaw;
    public bool invertAxe = true;
    public float angleClampY = -90f;
    public float angleClampZ = 90f;
    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        //if target is null get out
        /*if(!target)
            return;*/

        if (useControlRotation && Application.isPlaying)
        {
            Rotate();
        }
    }

    ///<summary>
    ///Handle Rotation
    ///</summary>
    private void Rotate()
    {
        yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        if (!invertAxe)
        {
            pitch += Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }
        else 
        { 
            pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime; 
        }

        pitch = Mathf.Clamp(pitch, -angleClampY, angleClampZ);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }



}
