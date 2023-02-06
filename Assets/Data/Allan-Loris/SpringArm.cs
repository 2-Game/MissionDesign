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
    [SerializeField][Range(-90.0f, 90)] public float angleClampY = -90f;
    [SerializeField][Range(90.0f, 0)] public float angleClampZ = 90f;
    #endregion

    #region Follow Settings
    [Space]
    [Header("Player Follow Settings \n")]
    [Space]

    [SerializeField] private Transform target;
    [SerializeField] private float movementSmothTime = 0.2f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);

    //refs for Smooth Damping
    private Vector3 moveVelocity;

    [SerializeField] private float targetArmLength = 3f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0.5f, 0, -0.3f);

    private Vector3 endPoint;
    private Vector3 cameraPosition;
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

    void Start()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }

    private void OnValidate()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }

    void Update()
    {
        //if target is null get out / nullref check
        if(!target)
            return;

        //collision check
        if (doCollisionTest)
            CheckCollisions();
        
        SetCameraTransform();

        if (useControlRotation && Application.isPlaying)
        {
            Rotate();
        }

        //follow target with offSet
        Vector3 targetPosition = target.position + targetOffset;
        transform.position = Vector3.SmoothDamp(targetPosition, targetPosition, ref moveVelocity, movementSmothTime);
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
                if(!hit.collider)
                    continue;

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
