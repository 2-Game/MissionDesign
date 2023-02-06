using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class SpringArm : MonoBehaviour
{
    #region Rotation Settings

    [Space]
    [Header("Rotation Settings \n------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool useControlRotation = true;
    [SerializeField] private float mouseSensitivity = 500f;

    // For mouse inputs
    private float pitch;
    private float yaw;
    private bool invertAxe = true;


    [SerializeField] [Range(-90.0f, 0)] private float AngleClampX = -90f;
    [SerializeField] [Range(0, 90.0f)] private float AngleClampY = 90f;

    #endregion

    #region Follow Settings

    [Space]
    [Header("Follow Settings \n------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Transform target;
    [SerializeField] private float movementSmoothTime = 0.2f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);
    [SerializeField] private float targetArmLength = 3f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0.5f, 0, -0.3f);


    //refs for SmoothDanmping
    private Vector3 moveVelocity;
    private Vector3 endPoint;
    private Vector3 CameraPosition;

    #endregion

    #region Collisions

    [Space]
    [Header("Collision Settings\n ------------------------------------------------------------------------")]
    [Space]

    [SerializeField] private bool doCollisionsTest = true;
    [Range(2, 20)][SerializeField] private int collisionTestResolution = 4;
    [SerializeField] private float collisionProbeSize = 0.3f;
    [SerializeField] private float collisionSmoothtime = 0.05f;
    [SerializeField] private LayerMask collisionLayerMask = ~0;

    private RaycastHit[] hits;
    private Vector3[] raycastPositions;

    #endregion


    void Update()
    {
        // If target is null, get out
        if (!target)
        {
            return;
        }

        // Collision check
        if (doCollisionsTest)
        {
            CheckCollision();
        }
        SetCameraTransform();

        // Handle mouse inputs for rotations
        if (useControlRotation && Application.isPlaying)
        {
            Rotate();
        }

        // Follow the target applying targetOffset
        Vector3 targetPosition = target.position + targetOffset;
        transform.position = Vector3.SmoothDamp(transform.position,
            targetPosition, ref moveVelocity, movementSmoothTime);
    }

    /// <summary> 
    /// Handle rotations
    /// </summary>
    private void Rotate()
    {
        yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        if (!invertAxe)
        {

            pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }
        else
        {
            pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime * -1;
        }

        pitch += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, AngleClampX, AngleClampY);

        // Set the rotation to new rotation
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void SetCameraTransform()
    {
        //Cache transform as it is used quite often
        Transform trans = transform;

        // Offset a point in z direction of targetArmLength by camera offset and translating it into world space.
        Vector3 targetArmOffset = cameraOffset - new Vector3(0, 0, targetArmLength);
        endPoint = trans.position + (trans.rotation * targetArmOffset);

        // Set cameraPosition value as endpoint
        cameraOffset = endPoint;

        // If collisionTest is enabled
        if (doCollisionsTest)
        {
            //Finds the minDistance
            float minDistance = targetArmLength;
            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider)
                    continue;

                float distance = Vector3.Distance(hit.point, transform.position);
                if (minDistance > distance)
                {
                    minDistance = distance;
                }
            }

            // Calculate the direction of children movement
            Vector3 dir = (endPoint - transform.position).normalized;
            // Get vector for movement
            Vector3 armOffset = dir * (targetArmLength - minDistance);
            // Offset it by endPoint and set the cameraPositionValue
            CameraPosition = endPoint - armOffset;
        }
        // If collision is disabled
        else
        {
            // Set cameraPosition value as endPoint
            CameraPosition = endPoint;
        }

        // Iterate through all children and set their position as cameraPosition, using SmoothDamp to smoothly translate the vectors.
        Vector3 cameraVelocity = Vector3.zero;
        foreach (Transform child in trans)
        {
            child.position = Vector3.SmoothDamp(child.position,
                CameraPosition, ref cameraVelocity, collisionSmoothtime);
        }
    }
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
    /// Checks for collisions and fill the raycastPositions and hits array
    /// </summary> 
    
    private void CheckCollision()
    {
        //Cache transform as it is used quite often
        Transform trans = transform;

        //iterate through raycastpositions and hits and set the corresponding data
        for (int i = 0, angle = 0; i < collisionTestResolution; i++, angle += 360 / collisionTestResolution)
        {
            // Calculate the local position of a point with relation to angle
            Vector3 raycastLocalEndPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * collisionProbeSize;
            // Covert it to world space by offsetting it by origin: endPoint, and push in the array
            raycastPositions[i] = endPoint + (trans.rotation * raycastLocalEndPoint);
            // Sets the hit struct if collision is detected between this gameobject's position and calculated raycastPosition
            Physics.Linecast(trans.position, raycastPositions[i], out hits[i], collisionLayerMask);
        }
        
    }
}
