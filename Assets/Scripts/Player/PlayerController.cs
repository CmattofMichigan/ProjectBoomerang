using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundAcceleration = 10f;
    [SerializeField] private float groundDeceleration = 10f;
    public bool isVelocityLocked = false;
    

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float jumpApexThreshold = 1.5f;
    bool isJumpPressed = false;
    public bool isJumping = false;
    public bool jumpRequested = false;
    private bool jumpWasCut = false;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter = 0f;
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter = 0f;

    [Header("Wall Kick")]
    [SerializeField] private float kickAirAcceleration = 0.2f;
    public Vector3 airKickDirection;
    public bool isKickActive = false;
    public bool isWallJumpActive = false;
    
    
    

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float maxFallSpeed = -60f;
    [SerializeField] private float airAcceleration = 5f;
    [SerializeField] private float airDeceleration = 5f;
    [SerializeField] private float apexGravityMultiplier = 0.3f;
    private bool wasGrounded;
    

    [Header("Components")]
    private CharacterController characterController;
    private AbilityManager abilityManager;
    [SerializeField] private Transform cameraTransform;

    [Header("Player Input")]
    public Vector2 moveInput { get; set; }
    private PlayerInput inputActions;

    [Header("Audio")]
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;

    [Header("Throw")]
    [SerializeField] public Rigidbody Boomerang;
    private bool isThrown = false;

    public Vector3 velocity;
    public Vector3 targetDirection { get; set; }


    void Awake()
    {
        inputActions = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        abilityManager = GetComponent<AbilityManager>();

        inputActions.Player.Jump.started += onJump;
        inputActions.Player.Jump.canceled += onJump;

        inputActions.Player.BoomerangThrow.started += OnBoomerangThrow;
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
        ApplyHorizontalMovement();
        Jump();
        ApplyAcceleration();
        
        //if on ground

        if (CheckGround())
        {
            //reset coyote time counter when on ground
            if (velocity.y <= 0)
            {
                coyoteTimeCounter = coyoteTime;
            }
            

        }
        // else in air
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }



        characterController.Move(velocity * Time.deltaTime);

        CheckLanding();
    }

    private void ApplyHorizontalMovement()
    {
        
        Vector2 input = Vector2.ClampMagnitude(moveInput, 1f);

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        targetDirection = cameraForward * input.y + cameraRight * input.x;

        targetDirection = Vector3.ClampMagnitude(targetDirection, 1f);

    }

    // retrieves the movement input from the player input system
    private void GetMovementInput()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();

        if (context.started)
        {
            jumpRequested = true;
            jumpBufferCounter = jumpBufferTime;
        }
    }

    void OnBoomerangThrow(InputAction.CallbackContext context)
    {
        
        if (context.ReadValueAsButton() && !isThrown && abilityManager.HasAbility(AbilityType.Boomerang))
        {
            Rigidbody boom = Instantiate(Boomerang, transform.position + transform.forward, transform.rotation) as Rigidbody;
            isThrown = true;
        }
    }

    public void ReturnBoomerang()
    {
        isThrown = false;
        //Debug.Log("Boomerang returned");
    }

    private void ApplyAcceleration()
    {
        // If velocity is locked, do not apply acceleration
        if (isVelocityLocked)
        {
            return;
        }

        // If input is too small, set deceleration flag
        bool isDeceleration = false;
        if (targetDirection.sqrMagnitude < 0.001f)
        {
            isDeceleration = true;
        }

        float currentAcceleration;
        if (CheckGround())
        {
            currentAcceleration = isDeceleration ?  groundDeceleration : groundAcceleration;
        }
        else
        {
            currentAcceleration = isDeceleration ? airDeceleration : isKickActive ? kickAirAcceleration : airAcceleration;
        }

        Vector3 horizontalMovement = new Vector3(velocity.x, 0, velocity.z);
        

        horizontalMovement = Vector3.MoveTowards(horizontalMovement, targetDirection * moveSpeed, currentAcceleration * Time.deltaTime);
        velocity.x = horizontalMovement.x;
        velocity.z = horizontalMovement.z;
    }

    
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
            float calculatedGravity = gravity;
           
            //if near apex of jump reduce gravity for short hangtime
            if (Mathf.Abs(velocity.y) < jumpApexThreshold)
            {
                calculatedGravity *= apexGravityMultiplier;
            }
            if (velocity.y < maxFallSpeed)
            {
                velocity.y = maxFallSpeed;
            }

            
            velocity.y += calculatedGravity * Time.deltaTime;
        }

    }

    private void Jump()
    {
        jumpBufferCounter -= Time.deltaTime;

        if (jumpRequested && (CheckGround() || coyoteTimeCounter > 0f) && jumpBufferCounter >= 0f)
        {
            coyoteTimeCounter = 0f;
        
            jumpRequested = false;
            velocity.y = jumpForce;
            isJumping = true;
            jumpWasCut = false;
            //play jump sound
            SFXManager.instance.PlaySFXClip(jump, transform, 1f);
        }

        if (velocity.y <= 0)
        {
            isJumping = false;
        }

        // if the player releases the jump button while jumping, cut the jump short
        if (!isJumpPressed && isJumping && velocity.y > 0)
        {
            if (!jumpWasCut)
            {
                jumpWasCut = true;
                velocity.y *= jumpCutMultiplier;

            }
        }
    }

    private void CheckLanding()
    {
        bool isGrounded = CheckGround(); 

        if (isGrounded && !wasGrounded)
        {
            // Player has just landed
            SFXManager.instance.PlaySFXClip(land, transform, 1f);
        }

        wasGrounded = isGrounded;
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherThing = collision.gameObject;
        if (otherThing.CompareTag("Boomerang"))
        {
            Destroy(otherThing);
            isThrown = false;
        }
    }

}
