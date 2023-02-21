using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5, _rotationSpeed = 100;
    private Vector2 _movement = Vector2.zero;
    private Animator _animator;
    private Rigidbody _rigidbody;

    public Animator ladderAnim;
    public SpringArm camArm;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.Rotate(0, _movement.x * Time.deltaTime * _rotationSpeed, 0);
        _rigidbody.MovePosition(transform.position + transform.forward * (_movement.y * _movementSpeed * Time.deltaTime));
        //transform.position += transform.forward * (_movement.y * _movementSpeed * Time.deltaTime);
    }

    //Values are already normalized through the new Input System
    public void Move(InputAction.CallbackContext ctx)
    {
        if (PlayerInteractionAnim.AnimationInProgress)
        {
            _movement = Vector2.zero;
            return;
        }
        _movement = ctx.ReadValue<Vector2>();
        _animator.SetFloat("Speed", _movement.sqrMagnitude == 0 ? 0 : 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            ladderAnim.SetBool("Up", true);
        }

        if (other.CompareTag("1stperson"))
        {
            //1st person
            camArm.cameraStatus = CameraStatus.FirstPerson;
        }
        else if (other.CompareTag("Cinematic"))
        {
            //cinematic view 1
            camArm.cameraStatus = CameraStatus.Camera1;
        }
        else if (other.CompareTag("Cinematic2"))
        {
            //cinematic view 2
            camArm.cameraStatus = CameraStatus.Camera2;
        }
    }

    private void OnTriggerExit(Collider lad)
    {
        ladderAnim.SetBool("Up", false);
        
        //third person
        camArm.cameraStatus = CameraStatus.ThirdPerson;
    }
}
