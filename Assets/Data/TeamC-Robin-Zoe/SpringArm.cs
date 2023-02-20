using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

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
    public static SpringArm Instance;
    #region Rotation Settings
    [Space]
    [Header("Rotation Settings \n-------------------------")]
    [Space]
    [SerializeField] private bool useControlRotation = true;
    [SerializeField] private float mouseSensitivity = 500f;
    [SerializeField] private bool invertYAxis;
    [SerializeField] [Range(-90.0f, 0)] private float minAngle = -90f;
    [SerializeField] [Range(0, 90.0f)] private float maxAngle = 90f;

    private float pitch;
    private float yaw;
    private int axeInt;


    #endregion

    #region Follow Settings

    [Space]
    [Header("Follow Settings \n-------------------------")]
    [Space]
    [SerializeField] private Transform target;
    [SerializeField] private float movementSmoothTime = 0.5f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);
    [SerializeField] private float targetArmLength = 13f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0.5f, 0, -0.3f);

    private Vector3 moveVelocity;
    private Vector3 endPoint;
    private Vector3 cameraPosition;

    [SerializeField] private float deadZoneSize;
    [SerializeField] private float targetZoneSize = 0.1f;
    private DeadZoneStatus deadZoneStatus = DeadZoneStatus.In;


    #endregion

    #region Camera Deadzone
    [Space]
    [Header("Camera DeadZone Settings \n")]
    [Space]

    [SerializeField] private bool cameraCanMove = true;
    [SerializeField] private float circleSize = 2.0f;

    #endregion

    #region Collisions

    [Space]
    [Header("Collision Settings \n-------------------------")]
    [Space]
    [SerializeField] private bool doCollisionTest = true;
    [Range(2, 20)] [SerializeField] private int collisiontestResolution = 4;
    [SerializeField] private float collisionProbeSize = 0.3f;
    [SerializeField] private float collisionSmoothTime = 0.05f;
    [SerializeField] private LayerMask collisionLayerMask = ~0;

    private RaycastHit[] hits;
    private Vector3[] raycastPositions;


    #endregion

    #region Debug

    [Space]
    [Header("Debugging \n-------------------------")]
    [Space]

    [SerializeField] private bool visualDebugging = true;
    [SerializeField] private Color springArmColor = new Color(0.75f, 0.2f, 0.2f, 0.75f);
    [Range(1f, 10f)] [SerializeField] private float springArmLineWidth = 6f;
    [SerializeField] private bool showRayCasts;
    [SerializeField] private bool showCollisionProbe;

    private readonly Color collisionProbeColor = new Color(0.2f, 0.75f, 0.2f, 0.15f);

    #endregion

    #region Camera Transition

    [Space]
    [Header("Camera Transition \n-------------------------")]
    [Space]

    [SerializeField] private Transform camera1;
    [SerializeField] private Transform fpsView;
    private CameraStatus cameraStatus = CameraStatus.ThirdPerson;

    public bool isInPlace, isInStreet, isInside, isOnStair;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (Instance) Destroy(this);
        else Instance = this;

        raycastPositions = new Vector3[collisiontestResolution];
        hits = new RaycastHit[collisiontestResolution];
    }

    private void OnValidate()
    {
        raycastPositions = new Vector3[collisiontestResolution];
        hits = new RaycastHit[collisiontestResolution];
    }

    // Update is called once per frame
    void Update()
    {
        //if target is null get out : nullref check
        if (!target)
        {
            return;
        }

        Vector3 targetPosition = Vector3.zero;

        if (Input.GetKey(KeyCode.Alpha1) || isInside)
        {
            cameraStatus = CameraStatus.FirstPerson;
        }
        else
        if (Input.GetKey(KeyCode.Alpha2) || isInPlace)
        {
            cameraStatus = CameraStatus.Camera1;
        }
        else
        if (Input.GetKey(KeyCode.Alpha3) || isInStreet || isOnStair)
        {
            cameraStatus = CameraStatus.ThirdPerson;
        }

        switch (cameraStatus)
        {
            case CameraStatus.Camera1:
                {
                    targetPosition = camera1.position;
                    transform.LookAt(target);
                    break;
                }

            case CameraStatus.FirstPerson:
                {
                    //targetArmLength = 0f;
                    /*angleClampZ = 20f;
                    cameraOffset = new Vector3(0f, 0, 0f);
                    //targetPosition = target.position + targetOffset;*/
                    targetPosition = fpsView.position;

                    transform.position = new Vector3(target.position.x, target.position.y + 1.8f, 0);
                    transform.GetChild(0).position = targetPosition;

                    transform.forward = target.forward;
                    movementSmoothTime = 10;
                    break;
                }

            case CameraStatus.ThirdPerson:
                {
                    targetArmLength = 3f;
                    cameraOffset = new Vector3(0.5f, 0, -0.3f);
                    maxAngle = 50f;

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

    //Ca c'est de la documentation qu'on peut rajouter
    /// <summary>
    /// Handle Rotation
    /// </summary>
    private void Rotate()
    {
        //Notation ternaire = faire ce if juste au dessu en une ligne pour avoir un retour de valeur, gauche si c'est true et droite si c'est false
        axeInt = invertYAxis ? -1 : 1;
        pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime * axeInt;
        yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        //Bloque la caméra verticalement pour pas qu'elle passe derrière le player
        pitch = Mathf.Clamp(pitch, minAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void SetCameraTransform()
    {
        Transform trans = transform;

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

            Vector3 dir = (endPoint - trans.position).normalized;
            Vector3 armOffset = dir * (targetArmLength - minDistance);
            cameraPosition = endPoint - armOffset;
        }
        else
        {
            cameraPosition = endPoint;
        }

        Vector3 cameraVelocity = Vector3.zero;
        foreach (Transform child in trans)
        {
            child.position = Vector3.SmoothDamp(child.position, cameraPosition, ref cameraVelocity, collisionSmoothTime);
        }
    }

    ///<summary>
    ///Checks for collisions and fill the raycastPositions and hits array
    /// </summary>
    private void CheckCollisions()
    {
        Transform trans = transform;

        for (int i = 0, angle = 0; i < collisiontestResolution; i++, angle += 360 / collisiontestResolution)
        {
            Vector3 raycastLocalEndPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * collisionProbeSize;
            raycastPositions[i] = endPoint + (trans.rotation * raycastLocalEndPoint);
            Physics.Linecast(trans.position, raycastPositions[i], out hits[i], collisionLayerMask);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualDebugging)
            return;

        Handles.color = springArmColor;
        if (showRayCasts)
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

        Handles.color = collisionProbeColor;
        if (showCollisionProbe)
        {
            Handles.SphereHandleCap(0, cameraPosition, Quaternion.identity, 2 * collisionProbeSize, EventType.Repaint);
        }
    }
    private void CheckCamPlayerDistance()
    {
        if (Vector3.Distance(transform.position, target.position + targetOffset) <= circleSize)
        {
            Debug.Log("Cam doesnt't move");
            cameraCanMove = false;
        }
        else
        {
            Debug.Log("Cam moves");
            cameraCanMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Place")
        {
            isInPlace = true;
        }
        else if(other.gameObject.tag == "Street")
        {
            isInStreet = true;
        }
        else if(other.gameObject.tag == "Inside")
        {
            isInside = true;
        }
        else if(other.gameObject.tag == "Echelle")
        {
            isOnStair = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Place")
        {
            isInPlace = false;
        }
        else if (other.gameObject.tag == "Street")
        {
            isInStreet = false;
        }
        else if (other.gameObject.tag == "Inside")
        {
            isInside = false;
        }
        else if (other.gameObject.tag == "Echelle")
        {
            isOnStair = false;
        }
    }
}
