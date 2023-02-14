using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    [SerializeField] private SpringArm SpringArm;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("tps1"))
        {
            
            SpringArm.cameraStatus = CameraStatus.ThirdPersonClose;
        }
        if (other.CompareTag("fpsCollider"))
        {
            SpringArm.cameraStatus = CameraStatus.FirstPerson;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("tps1"))
        {
            SpringArm.cameraStatus = CameraStatus.ThirdPersonVeryClose;
        }
        if (other.CompareTag("fpsCollider"))
        {
            SpringArm.cameraStatus = CameraStatus.Camera1;
        }
    }





    
    


}
