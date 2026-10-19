using UnityEngine;

public class AirKick : MonoBehaviour
{
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private float kickLift = 1f;
    [SerializeField] private float kickDistance = 2f;
    [SerializeField] private float kickDuration = 0.5f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpYForce = 5f;
    private float kickTimer = 0f;
    private bool kickIsActive = false;
    private bool kickAvailable = true;


    private PlayerController playerController;
    private CharacterController characterController;


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
        airKick();
        castKick();
        KickTimer();
        KickReset();
    }

    private void airKick()
    {
        
        //if jump pressed while airborne
        if (playerController.jumpRequested && !characterController.isGrounded && kickAvailable)
        {
            kickAvailable = false;
            kickIsActive = true;
            kickTimer = 0f;
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
            kickTimer += Time.deltaTime;
            
            if (kickTimer >= kickDuration)
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

        kickAvailable = true;

    }

    private void KickReset()
    {
        if (characterController.isGrounded)
        {
            kickAvailable = true;
        }
    }
}
