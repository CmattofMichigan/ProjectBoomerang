using System;
using UnityEngine;

public class AirKick : MonoBehaviour
{
    [Header("Air Kick Settings")]
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private float kickLift = 1f;
    [SerializeField] private float kickDistance = 2f;
    [SerializeField] private float kickDuration = 0.5f;
    [SerializeField] private float kickDelayTimer = 0.15f;
    [SerializeField] private float kickInputBufferTimer = 0.2f;
    private float kickDelayRemaining = 0f;
    private bool kickInputBuffered = false;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpYForce = 5f;
    [SerializeField] private float wallJumpControlBuffer = 0.2f;
    private float wallKickVelocityLockTimer = 0f;
    [SerializeField] private float wallKickVelocityTimerLength = 2f;


    private float kickActiveTimer = 0f;
    private bool kickIsActive = false;
    private bool kickAvailable = false;


    private PlayerController playerController;
    private CharacterController characterController;
    private PlayerRotation playerRotation;

    


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log(kickAvailable);
        checkAirKickAvailability();
        KickReset();
        airKick();
        castKick();
        KickTimer();
        WallKickControlLock();
        
    }

    private void checkAirKickAvailability()
    {
        if (!characterController.isGrounded && !kickAvailable && !kickIsActive)
        {
            Debug.Log("checking kick availability");
            kickDelayRemaining += Time.deltaTime;

            if (kickDelayRemaining >= kickDelayTimer)
            {
                
                kickAvailable = true;
                kickDelayRemaining = 0f;
            }
        }
    }

    private void airKick()
    {
        
        //if jump pressed while airborne
        if (playerController.jumpRequested && kickAvailable)
        {
            kickIsActive = true;
            kickAvailable = false;
            
            kickActiveTimer = 0f;
            playerController.jumpRequested = false;
            Vector3 kickDirection = playerController.targetDirection * kickForce;

            kickDirection.y = kickLift;

            //playerController.velocity.x += kickDirection.x;
            //playerController.velocity.z += kickDirection.z;
            playerController.velocity.y = kickDirection.y;
        }
    }

    private void castKick()
    {
        if (kickIsActive)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, kickDistance))
            {
                if (Mathf.Abs(hitInfo.normal.y) < 0.2f)
                {
                    kickIsActive = false;
                    TriggerWallKick(hitInfo);
                }
            }
        }
    }

    private void KickTimer() 
    {
        if (kickIsActive)
        {
            kickActiveTimer += Time.deltaTime;
            
            if (kickActiveTimer >= kickDuration)
            {
                kickIsActive = false;
            }

        }
        
    }

    private void TriggerWallKick(RaycastHit hitInfo)
    {
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
        //playerRotation.StartWallKickRotationLock();
        playerController.isWallKickVelocityLocked = true;
        wallKickVelocityLockTimer = wallKickVelocityTimerLength;
        

        kickAvailable = true;

    }

    private void KickReset()
    {
        if (characterController.isGrounded || kickIsActive)
        {
            kickAvailable = false;
        }
    }

    private void WallKickControlLock()
    {
        if (wallKickVelocityLockTimer > 0f)
        {
            wallKickVelocityLockTimer -= Time.deltaTime;
        }
        else
        {
            playerController.isWallKickVelocityLocked = false;
            
        }
        
    }

    

    
   
}
