using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    private CharacterController _characterController;

    [Header("Gravity")]
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float gravity;
    private Vector3 playerVelocity;

    [Header("Rotation")] 
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSmoothTime;
    private float currentVelocity;
    private Vector2 mouseCamRotation;
    [SerializeField] private float mouseCamRotationSpeed;

    private PlayerManager _playerManager;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerManager.playerHealth.isDead) return;

        Movement();
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.position, groundCheckRadius, groundLayer);  
    }

    void Movement()
    {
        if (IsGrounded()) playerVelocity.y = -2f;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0, z).normalized;

        //Set player animation movement
        float moveAnim = new Vector2 (x, z).magnitude;
        _playerManager.playerAnimation.animator.SetFloat(
            _playerManager.playerAnimation.MOVE_ANIM_PARAM, moveAnim, 0.1f, Time.deltaTime);

        bool hasPlayerInputMoving = move.magnitude >= 0.1f;

        if (_playerManager.playerShoot.isAiming)
        {
            if (hasPlayerInputMoving)
            {
                //Logic movement baru
                MoveCharacter(AimMovement(move));

            }
            MouseCamRotation();
        }
            else
            {
                if (hasPlayerInputMoving)
                {
                    MoveCharacter(FreeMovement(move));
                }   
            }

        playerVelocity.y += gravity * Time.deltaTime;
        _characterController.Move(playerVelocity * Time.deltaTime);
    }

    Vector3 AimMovement(Vector3 move)
    {
        move = transform.TransformDirection(move);
        return move;
    }

    Vector3 FreeMovement(Vector3 move)
    {
        float targetAngle = Mathf.Atan2 (move.x, move.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        //Membuat biar putaran playernya lebih smooth
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, rotationSmoothTime);
        //Ini baru bener muter playernya
        transform.rotation = Quaternion.Euler(0, angle, 0);

        Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        MoveCharacter(moveDirection);        
        return moveDirection;
    }

    void MoveCharacter(Vector3 move)
    {
        _characterController.Move(move.normalized * movementSpeed * Time.deltaTime);
    }

    void MouseCamRotation()
    {
        mouseCamRotation.x += Input.GetAxis("Mouse X") * mouseCamRotationSpeed;
        mouseCamRotation.y += Input.GetAxis("Mouse Y") * mouseCamRotationSpeed;

        mouseCamRotation.y = Mathf.Clamp(mouseCamRotation.y, -10, 10);

        transform.rotation = Quaternion.Euler(-mouseCamRotation.y, mouseCamRotation.x, 0);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawSphere(groundChecker.position, groundCheckRadius);    
    }
}
