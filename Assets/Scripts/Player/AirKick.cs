using System;
using UnityEngine;

public class AirKick : MonoBehaviour
{
    [Header("Air Kick Settings")]
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private float kickLift = 1f;
    [SerializeField] private float kickDistance = 2f;
    [SerializeField] private float kickDelayTimer = 0.15f;
    //[SerializeField] private float kickInputBufferTimer = 0.2f;
    private float kickDelayRemaining = 0f;
    //private bool kickInputBuffered = false;
    private float airKickLockTimer = 0f;
    [SerializeField] private float airKickDuration = 2f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpYForce = 5f;
    //[SerializeField] private float wallJumpControlBuffer = 0.2f;
    private float wallKickLockTimer = 0f;
    [SerializeField] private float wallKickDuration = 2f;
    private float kickCounter = 0;
    


    
    
    private bool kickAvailable = false;
    private bool kickUsed = false;


    private PlayerController playerController;
    private CharacterController characterController;
    private PlayerRotation playerRotation;

    


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        playerRotation = GetComponent<PlayerRotation>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkAirKickAvailability();
        KickReset();
        PerformAirKick();
        castKick();
        AirKickControlLock();
        WallKickControlLock();
    }

    private void checkAirKickAvailability()
    {
        if (!characterController.isGrounded && !kickAvailable)
        {

    
            kickDelayRemaining += Time.deltaTime;

            if (kickDelayRemaining >= kickDelayTimer && !kickUsed && kickCounter < 3)
            {
                kickAvailable = true;
                kickDelayRemaining = 0f;
            }
        }
    }

    private void PerformAirKick()
    {
        
        //if jump pressed while airborne
        if (playerController.jumpRequested && kickAvailable)
        {
            playerRotation.airKickDirection = playerController.targetDirection;

            playerController.isKickActive = true;
            kickUsed = true;
            airKickLockTimer = airKickDuration;
            
            
            playerController.jumpRequested = false;
            Vector3 kickDirection = playerController.targetDirection * kickForce;

            kickDirection.y = kickLift;

            playerController.velocity.x += kickDirection.x;
            playerController.velocity.z += kickDirection.z;
            playerController.velocity.y = kickDirection.y;
            kickAvailable = false;
            kickCounter += 1;
        }
    }

    private void castKick()
    {
        if (playerController.isKickActive && !characterController.isGrounded)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, kickDistance))
            {
                if (Mathf.Abs(hitInfo.normal.y) < 0.2f)
                {
                    playerController.isKickActive = false;
                    TriggerWallKick(hitInfo);
                }
            }
        }
        
    }

   

    private void TriggerWallKick(RaycastHit hitInfo)
    {
        kickUsed = false;
        playerController.isWallJumpActive = true;
        // Apply a force in the opposite direction of the wall normal
        Vector3 incomingVelocity = playerController.velocity;
        // incoming horizontal velocity
        Vector3 incomingHorizontalVelocity = new Vector3(incomingVelocity.x, 0, incomingVelocity.z);
        // wall normal
        Vector3 wallNormal = new Vector3(hitInfo.normal.x, 0, hitInfo.normal.z).normalized;

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
        playerController.velocity.x = wallBounceDirection.x * wallJumpForce; // Apply a horizontal force to the player from Wall Jump
        playerController.velocity.z = wallBounceDirection.z * wallJumpForce;
        playerController.velocity.y = wallJumpYForce;// Apply a vertical force to the player

        transform.rotation = Quaternion.LookRotation(wallBounceDirection);
        
        playerController.isVelocityLocked = true;
        playerRotation.isRotationLocked = true;
        wallKickLockTimer = wallKickDuration;
        
        

    }

    private void KickReset()
    {
        if (characterController.isGrounded)
        {
            kickAvailable = false;
            playerController.isKickActive = false;
            playerController.isWallJumpActive = false;
            playerController.isVelocityLocked = false;
            playerRotation.isRotationLocked = false;
            kickUsed = false;
            airKickLockTimer = airKickDuration;
            wallKickLockTimer = wallKickDuration;
            kickCounter = 0;

        }
    }

    private void WallKickControlLock()
    {
        if (playerController.isWallJumpActive)
        {
            if (wallKickLockTimer > 0f)
            {
                wallKickLockTimer -= Time.deltaTime;
            }
            else
            {
                playerController.isVelocityLocked = false;
                playerRotation.isRotationLocked = false;
                playerController.isWallJumpActive = false;

            }

        }

    }

    private void AirKickControlLock()
    {
        if (playerController.isKickActive)
        {
            if (airKickLockTimer > 0f)
            {
                airKickLockTimer -= Time.deltaTime;
            }
            else
            {
                //playerController.isVelocityLocked = false;
                //playerRotation.isRotationLocked = false;
                playerController.isKickActive = false;
            }
        }
    }





}
