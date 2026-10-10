using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    bool isJumpPressed = false;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float maxFallSpeed = -60f;



    private CharacterController characterController;
    private Vector2 moveInput;
    private PlayerInput inputActions;

    private Vector3 velocity;

    void Awake()
    {
        inputActions = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        inputActions.Player.Jump.started += onJump;
        inputActions.Player.Jump.canceled += onJump;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
        UpdateVelocity();
        
    }

    // takes movement input and applies it to the player's velocity
    private void UpdateVelocity()
    {
        ApplyGravity();
        //if on ground
        if (CheckGround())
        {
            // apply horizontal movement when on the ground
            ApplyHorizontalMovement();
            // apply jump when on the ground
            Jump();
        }
        // else in air
        {
            HandleJumpHeight();
        }
        
        characterController.Move(velocity * Time.deltaTime);
    }

    private void ApplyHorizontalMovement()
    {
        Vector2 input = Vector2.ClampMagnitude(moveInput, 1f);

        velocity.x = input.x * moveSpeed;
        velocity.z = input.y * moveSpeed;
    }

    // retrieves the movement input from the player input system
    private void GetMovementInput()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    /*private bool CheckCollision(Vector3 direction, float distance, out RaycastHit hitInfo)
    {
    
        Vector3 center = capsuleCollider.bounds.center;
        float halfHeight = capsuleCollider.bounds.extents.y;
        Vector3 point1 = center + Vector3.up * (halfHeight - capsuleCollider.radius);
        Vector3 point2 = center + Vector3.down * (halfHeight - capsuleCollider.radius);

        bool hit = Physics.CapsuleCast(point1, point2, capsuleCollider.radius, direction, out hitInfo, distance);
        
        return hit;
    }*/
    
    // checks if the player is on the ground
    private bool CheckGround()
    {
        return characterController.isGrounded;
    }





    private void ApplyGravity()
    {
        if (CheckGround())
        {
            float groundVelocity = -2f;
            velocity.y = groundVelocity;
        }

        else
        {
            velocity.y += gravity * Time.deltaTime;
            if (velocity.y < maxFallSpeed)
            {
                velocity.y = maxFallSpeed;
            }
        }

    }

    private void Jump()
    {
        if (isJumpPressed)
        {
            velocity.y = jumpForce;
        }
    }

    private void HandleJumpHeight()
    {
        if (!isJumpPressed)
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }
    }

}
