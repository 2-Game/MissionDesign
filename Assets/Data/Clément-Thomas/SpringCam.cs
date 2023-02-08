using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


enum deadZoneStatus
{
    In, Out, CatchingUp
}
enum cameraStatus
{
    ThirdPerson, FirstPerson, Camera1
}

//pour le first personne il faut faire un set des valeur dans l'inspector 

public class SpringCam : MonoBehaviour
{
    #region Rotation Settings

    [Space]
    [Header("Rotation Settings \n----------------")]
    [Space]
    [SerializeField] private bool useControlRotation = true;
    [SerializeField] private float mouseSensitivity = 500f;
    [SerializeField] private float minPitchValue = -90;
    [SerializeField] private float maxPitchValue = 90;
    [SerializeField] private bool invertYaxis = false;


    // For mouse inputs
    private float pitch;
    private float yaw;
    private int axeInt;

    #endregion

    #region Follow Settings
    [Space]
    [Header("Follow Settings \n--------------------")]
    [Space]
    [SerializeField] private Transform target;
    [SerializeField] private float movementSmoothTime = 0.2f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);
    [SerializeField] private float targetArmLenght = 3f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0.5f, 0, -0.3f);
    [SerializeField] private bool sphereOffset;
    [SerializeField] private float deadZoneSize = 2.0f;
    [SerializeField] private float targetZoneSize = 0.1f;
    private DeadZoneStatus deadZoneStatus = DeadZoneStatus.In;


    private Vector3 endPoint;
    private Vector3 cameraPosition;

    //refs for SmoothDamping
    private Vector3 moveVelocity;

   

    #endregion

    #region Debug
    [Space]
    [Header("Debugging \n----------------")]
    [Space]

    [SerializeField] private bool visualDebugging = true;
    [SerializeField] private Color springArmColor = new Color(0.75f, 0.2f, 0.2f, 0.75f);
    [Range(1f, 10f)][SerializeField] private float springArmLineWidth = 6f;
    [SerializeField] private bool showRaycasts;
    [SerializeField] private bool showCollisionProbe;

    private readonly Color collisionProbeColor = new Color(0.2f, 0.75f, 0.2f, 0.15f);

    #endregion

    #region Camera Transition

    [Space]
    [Header("Camera Transition \n------------")]
    [Space]
    [SerializeField] private Transform camera1;
    private CameraStatus cameraStatus = CameraStatus.ThirdPerson;

    #endregion

    #region Collisions

    [Space]
    [Header("Collision Settings \n----------------")]
    [Space]

    [SerializeField] private bool doCollisionTest = true;
    [Range(2, 20)][SerializeField] private int collisionTestResolution = 4;
    [SerializeField] private float collisionProbeSize = 0.3f;
    [SerializeField] private float collisionSmoothTime = 0.05f;
    [SerializeField] private LayerMask collisionLayerMask = ~0;


    private RaycastHit[] hits;
    private Vector3[] raycastPositions;




    #endregion

    private void Start()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }

    private void OnValidate()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }
    /// <summary>
    /// CheckCollisions
    /// </summary>>
    private void CheckCollisions()
    {
        //Cache transforme as it is used quite often 
        Transform trans = transform;

        //iterate through raycast position and hits and set the corresponding data
        for (int i = 0, angle = 0; i < collisionTestResolution; i++, angle += 360 / collisionTestResolution)
        {
            //Calculate the local position of a point w.r.t angle
            Vector3 raycastLocalEndPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * collisionProbeSize;

            raycastPositions[i] = endPoint + (trans.rotation * raycastLocalEndPoint);

            Physics.Linecast(trans.position, raycastPositions[i], out hits[i], collisionLayerMask);
        }
    }


    void Update()
    {

        DistanceSphere();

        // if target is null, get out
        if (!target)
        {
            return;
        }

        Vector3 targetPosition = Vector3.zero;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            cameraStatus = CameraStatus.ThirdPerson;
        }
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            cameraStatus = CameraStatus.Camera1;

        }

        //switch(
        if(cameraStatus == CameraStatus.Camera1)
        {
            targetPosition = camera1.position;
            transform.LookAt(target);
        }
        else if(cameraStatus == CameraStatus.ThirdPerson)
        {

            //Collision check
            if (doCollisionTest)
            {
                CheckCollisions();
            }
            SetCameraTransform();
        }


        

        // Handle mouse inputs for rotations
        if (useControlRotation && Application.isPlaying)
        {
            Rotate();
        }

        

            

        



        // Follow the target applying targetOffset
        if (sphereOffset)
        {
            //Vector3 targetosition = target.position + targetOffset;
            transform.position = Vector3.SmoothDamp(transform.position,
                targetPosition, ref moveVelocity, movementSmoothTime);
        }
        

    }

    

    /// <summary>
    /// Handle rotations
    /// </summary>>

    private void Rotate()
    {   //Incremement yaw by Mouse X input
        yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        //Decrement pitch by Mouse Y input (ex : invertion de l'axe, avec un bool 1 ou - 1 )
        if (!invertYaxis)
        {
            axeInt = 1;
            pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        }
        else
        {
            axeInt = -1;
            pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime * axeInt;

        }


        //Clamp pitch so that we can't invert the gameobject by mistake
        pitch = Mathf.Clamp(pitch, minPitchValue, maxPitchValue);


        //Set the rotation to new rotation
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }


    private void SetCameraTransform()
    {
        //Cache transform as it is used quite often
        Transform trans = transform;

        //Offset a point in z direction of targetArmLengh by camera offset and translating int into world space
        Vector3 targetArmOffset = cameraOffset - new Vector3(0, 0, targetArmLenght);
        endPoint = transform.position + (trans.rotation * targetArmOffset);

        //If collisionTest is enable
        if (doCollisionTest)
        {
            //Finds the minDistance
            float minDistance = targetArmLenght;
            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider)
                {
                    continue;

                    float distance = Vector3.Distance(hit.point, trans.position);
                    if (minDistance < distance)
                    {
                        minDistance = distance;
                    }
                }

            }
            //Calculate the direction of children movement 
            Vector3 dir = (endPoint - trans.position).normalized;
            // Get vector for movement 
            Vector3 armOffset = dir * (targetArmLenght - minDistance);
            // Offset is by endPoint and set the cameraPositiveValue
            cameraPosition = endPoint - armOffset;
        }
        //If collision is disable
        else
        {
            //set cameraPosition value as endPoint
            cameraPosition = endPoint;
        }



        //Iterate through all children and set their position as cmaeraPosition, using SmoothDamp to smoothly translat the vector
        Vector3 cameraVelocity = Vector3.zero;
        foreach (Transform child in trans)
        {
            child.position = Vector3.SmoothDamp(child.position,
                cameraPosition, ref cameraVelocity, collisionSmoothTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualDebugging)
            return;

        //Draw main LineTrace or LineTraces of raycastPosition, useful for debugging
        Handles.color = springArmColor;
        if (showRaycasts)
        {
            foreach (Vector3 raycastPosition in raycastPositions)
            {
                Handles.DrawAAPolyLine(springArmLineWidth, 2, transform.position, raycastPosition);
            }

        }
        else
        {
            Handles.DrawAAPolyLine(springArmLineWidth, 2, transform.position, endPoint);
        }

        //Draw collisionProbe, useful for debugging
        Handles.color = collisionProbeColor;
        if (showCollisionProbe)
        {
            Handles.SphereHandleCap(0, cameraPosition, Quaternion.identity, 2 * collisionProbeSize, EventType.Repaint);
        }
    }

    //chaque fram calucler distance spring arme et la cible, cree le bool pour dire si en dehors ou pas et activer le mouvement.
    //chaque fram mesurer la distance, difference vecteur joueur et cam
   

    private void DistanceSphere()
    {
        Vector3 targetPosition = Vector3.zero;
        float distanceToTarget = Vector3.Distance(transform.position, target.position + targetOffset);

        if (distanceToTarget > deadZoneSize)
        {
            sphereOffset = true;
            deadZoneStatus = DeadZoneStatus.Out;
        }
        else
        {
            sphereOffset= false;

            switch (deadZoneStatus)
            {
                case DeadZoneStatus.In:
                    targetPosition = transform.position;
                    break;
                case DeadZoneStatus.Out:
                    targetPosition = target.position + targetOffset;
                    deadZoneStatus= DeadZoneStatus.CatchingUp;
                    break;
                case DeadZoneStatus.CatchingUp:
                    targetPosition = target.position + targetOffset;
                    if(distanceToTarget < targetZoneSize)
                    {
                        deadZoneStatus= DeadZoneStatus.In;
                    }
                    break;

            }


        }
    }
} 




