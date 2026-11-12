using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundAcceleration = 10f;
    [SerializeField] private float groundDeceleration = 10f;
    public bool isVelocityLocked = false;
    public MovingPlatform currentPlatform;
    
    


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

    [Header("Air Kick")]
    [SerializeField] private float kickAirAcceleration = 0.2f;
    public Vector3 airKickDirection;
    public bool isKickActive = false;
    public bool isWallJumpActive = false;
    private float kickDelayRemaining = 0f;
    [SerializeField] private float kickDelayTime = 0.15f;
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private float kickLift = 1f;
    [SerializeField] private float kickDistance = 2f;
    [SerializeField] private float airKickDuration = 2f;
    private float airKickLockTimer = 0f;
    private float kickCounter = 0;

    [Header("WallJump")]
    private Vector3 wallNormal;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpYForce = 5f;
    [SerializeField] private float wallJumpDuration = 2f;
    
    
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
    private PlayerRotation playerRotation;
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
    private Vector3 HorizontalVelocity => new Vector3(velocity.x, 0, velocity.z);
    public Vector3 targetDirection { get; set; }

    [Header("State")]
    [SerializeField] private PlayerState currentState = PlayerState.Idle;




    void Awake()
    {
        inputActions = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        abilityManager = GetComponent<AbilityManager>();
        playerRotation = GetComponent<PlayerRotation>();

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
        KickReset();
        GetMovementInput();
        UpdateTargetDirection();

        //run timers
        jumpBufferCounter -= Time.deltaTime;
        if (jumpBufferCounter < 0)
        {
            jumpRequested = false;
        }


        switch (currentState)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Walk:
                UpdateWalk();
                break;
            case PlayerState.Jump:
                UpdateJump();
                break;
            case PlayerState.Fall:
                UpdateFall();
                break;
            case PlayerState.Airkick:
                UpdateAirKick();
                break;
            case PlayerState.WallJump:
                UpdateWallJump();
                break;
        }

        UpdateVelocity();
        CheckLanding();
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
        {
            return;
        }
        Debug.Log($"State changed from {currentState} to {newState}");
        currentState = newState;
    }

    private void UpdateIdle()
    {
        float groundVelocity = -2f;
        velocity.y = groundVelocity;

        coyoteTimeCounter = coyoteTime;

        if (CheckFall())
        {
            ChangeState(PlayerState.Fall);
            return;
        }

        if (jumpRequested && CanStartJump())
        {
            StartJump();
            ChangeState(PlayerState.Jump);
            return;
        }

        GroundMovement();

        if (targetDirection.sqrMagnitude > 0.01f)
        {
            ChangeState(PlayerState.Walk);
        }
    }

    private void UpdateWalk()
    {
        float groundVelocity = -2f;
        velocity.y = groundVelocity;

        coyoteTimeCounter = coyoteTime;

        if (CheckFall())
        {
            ChangeState(PlayerState.Fall);
            return;
        }

        //change to jump state
        if (jumpRequested && CanStartJump())
        {
            StartJump();
            ChangeState(PlayerState.Jump);
            return;
        }

        GroundMovement();

        if (targetDirection.sqrMagnitude < 0.01f && HorizontalVelocity.sqrMagnitude < 0.01f)
        {
            ChangeState(PlayerState.Idle);
        }


    }

    private void UpdateJump()
    {

        AirMovement();

        coyoteTimeCounter -= Time.deltaTime;
        kickDelayRemaining += Time.deltaTime;

        // if the player releases the jump button while jumping, cut the jump short
        if (!isJumpPressed && velocity.y > 0)
        {
            if (!jumpWasCut)
            {
                jumpWasCut = true;
                velocity.y *= jumpCutMultiplier;

            }
        }


        if (CheckFall())
        {
            ChangeState(PlayerState.Fall);
            return;
        }

        if (jumpRequested && CanAirKick())
        {
            PerformAirKick();
            ChangeState(PlayerState.Airkick);
        }

    }

    private void UpdateFall()
    {
        AirMovement();

        coyoteTimeCounter -= Time.deltaTime;
        kickDelayRemaining += Time.deltaTime;

        if (jumpRequested && CanStartJump())
        {
            StartJump();
            ChangeState(PlayerState.Jump);
            return;
        }

        //switch State Airkick
        if (jumpRequested && CanAirKick())
        {
            PerformAirKick();
            ChangeState(PlayerState.Airkick);
            return;
        }

        if (CheckGround())
        {
            ChangeState(PlayerState.Idle);
        }

        
    }

    private void UpdateAirKick()
    {
        AirKickMovement();
        kickDelayRemaining = 0f;
        airKickLockTimer -= Time.deltaTime;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, kickDistance))
        {
            if (Mathf.Abs(hitInfo.normal.y) < 0.2f)
            {
                wallNormal = new Vector3(hitInfo.normal.x, 0, hitInfo.normal.z).normalized;
                PerformWallJump();
                ChangeState(PlayerState.WallJump);
                return;
            }
        }

        if (CheckGround())
            {
                ChangeState(PlayerState.Idle);
                return;
            }

        if (airKickLockTimer < 0)
        {
            jumpRequested = false;
            ChangeState(PlayerState.Fall);
        }

    }

    private void UpdateWallJump()
    {
        ApplyGravity();
        kickDelayRemaining += Time.deltaTime;

        if (jumpRequested && CanAirKick())
        {

            PerformAirKick();
            playerRotation.isRotationLocked = false;
            ChangeState(PlayerState.Airkick);

            return;
        }

        if (velocity.y < 0)
        {
            
            playerRotation.isRotationLocked = false;
            
            if (CheckGround())
            {
                ChangeState(PlayerState.Idle);
            }
            else
            {
                ChangeState(PlayerState.Fall);
            }
            
        }

    }
/*

        Vector3 platformMovement = Vector3.zero;

        if (currentPlatform != null && velocity.y <= 0f)
        {
            platformMovement = currentPlatform.GetMovement();
        }

        characterController.Move(velocity * Time.deltaTime + platformMovement);

        CheckLanding();

        */
    // takes movement input and applies it to the player's velocity
    private void UpdateVelocity()
    {
        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateTargetDirection()
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

    private void GroundMovement()
    {
        ApplyAcceleration(groundAcceleration, groundDeceleration);
    }

    private void AirMovement()
    {
        ApplyAcceleration(airAcceleration, airDeceleration);
        ApplyGravity();
    }

    private void AirKickMovement()
    {
        ApplyAcceleration(kickAirAcceleration, airDeceleration);
        ApplyGravity();
    }

    private bool CheckFall()
    {
        return (!CheckGround() && velocity.y < 0);
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

    private void ApplyAcceleration(float acceleration, float deceleration)
    {
        // If velocity is locked, do not apply acceleration
       

        // If input is too small, set deceleration flag
        bool isDeceleration = targetDirection.sqrMagnitude < 0.001f;

        float currentAcceleration = isDeceleration ? deceleration : acceleration;

        Vector3 currentHorizontal = HorizontalVelocity;
        Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetDirection * moveSpeed, currentAcceleration * Time.deltaTime);
        velocity.x = newHorizontal.x;
        velocity.z = newHorizontal.z;
    }


    // checks if the player is on the ground
    private bool CheckGround()
    {
        return characterController.isGrounded;
    }

    private void ApplyGravity()
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

    private void StartJump()
    {
        coyoteTimeCounter = 0f;

        jumpRequested = false;
        velocity.y = jumpForce;

        jumpWasCut = false;
        //play jump sound
        SFXManager.instance.PlaySFXClip(jump, transform, 1f);
    }

    private bool CanStartJump()
    {
        return (CheckGround() || coyoteTimeCounter > 0f) && jumpBufferCounter >= 0f;
    }

    #region Air Kick

    private bool CanAirKick()
    {
        return kickDelayRemaining >= kickDelayTime && kickCounter < 3 && abilityManager.HasAbility(AbilityType.AirKick);
    }
    

     private void PerformAirKick()
    {
            airKickLockTimer = airKickDuration;
            jumpRequested = false;
            Vector3 kickDirection = targetDirection * kickForce;

            //check this
            kickDirection.y = kickLift;

            velocity.x += kickDirection.x;
            velocity.z += kickDirection.z;
            
            velocity.y = kickDirection.y;
            kickCounter += 1;
    }

    private void KickReset()
    {
        if (CheckGround())
        {
            
            playerRotation.isRotationLocked = false;
            kickCounter = 0;

        }
    }

    private void PerformWallJump()
    {
        // Apply a force in the opposite direction of the wall normal
        Vector3 incomingVelocity = velocity;
        // incoming horizontal velocity
        Vector3 incomingHorizontalVelocity = new Vector3(incomingVelocity.x, 0, incomingVelocity.z);
        // wall normal
        Vector3 horizontalWallNormal = new Vector3(wallNormal.x, 0, wallNormal.z).normalized;

        // reflect incoming trajectory off wall
        Vector3 wallBounceDirection;

        
        if (incomingHorizontalVelocity.sqrMagnitude > 0.01f)
        {
            wallBounceDirection = Vector3.Reflect(incomingHorizontalVelocity.normalized, wallNormal).normalized;
        }
        else
        {
            wallBounceDirection = wallNormal;
        }
        velocity.x = wallBounceDirection.x * wallJumpForce; // Apply a horizontal force to the player from Wall Jump
        velocity.z = wallBounceDirection.z * wallJumpForce;
        velocity.y = wallJumpYForce;// Apply a vertical force to the player

        transform.rotation = Quaternion.LookRotation(wallBounceDirection);
        
        
        playerRotation.isRotationLocked = true;
        
    }

    #endregion



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
