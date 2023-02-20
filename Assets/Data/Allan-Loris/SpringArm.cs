using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

enum DeadZoneStatus
{
    In, Out, CatchingUp
}

enum CameraStatus
{
    ThirdPerson, FirstPerson, Camera1
}

public class SpringArm : MonoBehaviour
{
    [SerializeField] public GameObject playerSprite;

    #region Rotation Settings
    [Space]
    [Header("Rotation Settings \n")]
    [Space]

    [SerializeField] private bool useControlRotation = true;
    [SerializeField] private float mouseSensitivity = 500f;

    private float pitch;
    private float yaw;
    public bool invertAxe = false;
    [SerializeField][Range(-90.0f, 90)] public float angleClampY = -90f;
    [SerializeField][Range(90.0f, 0)] public float angleClampZ = 90f;
    #endregion

    #region Follow Settings
    [Space]
    [Header("Player Follow Settings \n")]
    [Space]

    [SerializeField] private Transform target;
    [SerializeField] private float movementSmoothTime = 0.4f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);

    //refs for Smooth Damping
    private Vector3 moveVelocity;

    [SerializeField] private float targetArmLength = 3f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0.5f, 0, -0.3f);
    [SerializeField] private Vector3 zeroPos = new Vector3(0, 0, 0);

    private Vector3 endPoint;
    private Vector3 cameraPosition;
    #endregion

    #region Camera Dead Zone
    [Space]
    [Header("Camera DeadZone Settings \n")]
    [Space]

    [SerializeField] private bool cameraCanMove = true;
    [SerializeField] private float circleSize = 2.0f;

    private DeadZoneStatus deadZoneStatus = DeadZoneStatus.In;
    #endregion

    #region Collision
    [Space]
    [Header("Collision Settings \n")]
    [Space]

    [SerializeField] private bool doCollisionTest = true;
    [Range(2, 20)][SerializeField] private int collisionTestResolution = 4;
    [SerializeField] private float collisionProbeSize = 0.5f;
    [SerializeField] private float collisionSmoothTime = 0.05f;
    [SerializeField] private LayerMask collisionLayerMask = ~0;

    private RaycastHit[] hits;
    private Vector3[] raycastPositions;
    #endregion

    #region Debug
    [Space]
    [Header("Debugging \n")]
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
    [Header("Camera Transition \n")]
    [Space]

    [SerializeField] private Transform Camera1;
    [SerializeField] private Transform FPSView;
    private CameraStatus cameraStatus = CameraStatus.ThirdPerson;
    #endregion

    void Start()
    {
        /*Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;*/

        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];

        transform.rotation = Quaternion.Euler(zeroPos);
    }

    private void OnValidate()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }

    void Update()
    {
        //if target is null get out : null ref check
        if (!target)
        {
            return;
        }

        Vector3 targetPosition = Vector3.zero;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            cameraStatus = CameraStatus.FirstPerson;
        } 
        else 
        if (Input.GetKey(KeyCode.Alpha2))
        {
            cameraStatus = CameraStatus.Camera1;
        }
        else
        if(Input.GetKey(KeyCode.Alpha3))
        {
            cameraStatus = CameraStatus.ThirdPerson;
        }

        switch (cameraStatus)
        {
            case CameraStatus.Camera1:
                {
                    playerSprite.SetActive(true);

                    target.rotation = Quaternion.Euler(zeroPos);

                    targetPosition = Camera1.position;
                    transform.LookAt(target);
                    break;
                }

            //you are working on this, it's down here, look

            case CameraStatus.FirstPerson:
                {
                    playerSprite.SetActive(false);

                    targetPosition = FPSView.position;

                    transform.position = new Vector3(target.position.x, target.position.y + targetOffset.y, 0);
                    transform.GetChild(0).position = targetPosition;

                    if (useControlRotation && Application.isPlaying)
                    {
                        Rotate();
                    }

                    target.forward = transform.forward;

                    break;
                }

            case CameraStatus.ThirdPerson:
                {
                    playerSprite.SetActive(true);

                    target.rotation = Quaternion.Euler(0, target.transform.rotation.y, 0);

                    //collision check
                    if (doCollisionTest)
                    {
                        CheckCollisions();
                    }

                    CheckCamPlayerDistance();

                    if (cameraCanMove)
                    {
                        SetCameraTransform();
                    }

                    if (useControlRotation && Application.isPlaying)
                    {
                        Rotate();
                    }

                    //follow target with offSet
                    float distancetotarget = Vector3.Distance(transform.position, targetPosition + targetOffset);
                    if (distancetotarget > circleSize)
                    {
                        deadZoneStatus = DeadZoneStatus.Out;
                        targetPosition = target.position + targetOffset;
                    }

                    else
                    {
                        switch (deadZoneStatus)
                        {
                            case DeadZoneStatus.In:
                                targetPosition = transform.position;
                                break;
                            case DeadZoneStatus.Out:
                                targetPosition = transform.position + targetOffset;
                                deadZoneStatus = DeadZoneStatus.CatchingUp;
                                break;
                            case DeadZoneStatus.CatchingUp:
                                targetPosition = transform.position + targetOffset;
                                if (distancetotarget > circleSize)
                                {
                                    deadZoneStatus = DeadZoneStatus.In;
                                }
                                break;
                        }
                    }
                }
                break;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref moveVelocity, movementSmoothTime);
    }

    private void CheckCamPlayerDistance()
    {
        if (Vector3.Distance(transform.position, target.position + targetOffset) <= circleSize)
        {
            Debug.Log("Cam doesn't move");
            cameraCanMove = false;
        }
        else
        {
            Debug.Log("Cam moves");
            cameraCanMove = true;
        }
    }

    private void CheckCollisions()
    {
        Transform trans = transform;

        for (int i = 0, angle = 0; i < collisionTestResolution; i++, angle += 360 / collisionTestResolution)
        {
            Vector3 rayvastLocalEndPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * collisionProbeSize;
            raycastPositions[i] = endPoint + (trans.rotation * rayvastLocalEndPoint);
            Physics.Linecast(trans.position, raycastPositions[i], out hits[i], collisionLayerMask);
        }
    }


    private void SetCameraTransform()
    {
        Transform trans = transform;

        //offset a point in Z direction of targetArmLength by cam offset and translating it into world space
        Vector3 targetArmOffset = cameraOffset - new Vector3(0, 0, targetArmLength);
        endPoint = trans.position + (trans.rotation * targetArmOffset);

        if (doCollisionTest)
        {
            float minDistance = targetArmLength;

            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider)
                { 
                    continue; 
                }

                float distance = Vector3.Distance(hit.point, trans.position);

                if (minDistance > distance)
                { 
                    minDistance = distance; 
                }
            }

            //calculate the direction of children movement
            Vector3 dir = (endPoint - trans.position).normalized;
            //get vector for movement
            Vector3 armOffset = dir * (targetArmLength - minDistance);
            //offset it by endPoint and set the cameraPositionValue
            cameraPosition = endPoint - armOffset;
        }
        //if collision is disabled
        else
        {
            cameraPosition = endPoint;
        }

        //iterate through all children and set their position as cameraPosition, using smoothDamp to smoothly translate the vectors
        Vector3 cameraVelocity = Vector3.zero;
        foreach (Transform child in trans)
        {
            child.position = Vector3.SmoothDamp(child.position, cameraPosition, ref cameraVelocity, collisionSmoothTime);
        }
    }

    //Handles Rotation
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

    private void OnDrawGizmosSelected()
    {
        if (!visualDebugging)
        {
            return;
        }

        //Draws main linesTrace or LineTrace of RaycastPositions, useful for debugging
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

        //Draws collisionProbe, useful for debugging
        Handles.color = collisionProbeColor;
        if (showCollisionProbe)
        {
            Handles.SphereHandleCap(0, cameraPosition, Quaternion.identity, 2 * collisionProbeSize, EventType.Repaint);
        }
    }
}
