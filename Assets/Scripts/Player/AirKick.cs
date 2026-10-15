using UnityEngine;

public class AirKick : MonoBehaviour
{
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private float kickLift = 1f;
    [SerializeField] private float kickDistance = 2f;
    [SerializeField] private float kickDuration = 0.5f;
    private float kickTimer = 0f;
    private bool kickIsActive = false;


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
    }

    private void airKick()
    {
        
        //if jump pressed while airborne
        if (playerController.jumpRequested && !characterController.isGrounded)
        {
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
                    TriggerWallKick();
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

    private void TriggerWallKick()
    {
        // Apply a force in the opposite direction of the wall normal
        Debug.Log("Wall Kick Triggered");
    }
}
