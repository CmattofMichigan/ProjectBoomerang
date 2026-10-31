using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

    
    public bool isRotationLocked = false;
    private Vector3 lockedRotationDirection;

    private Quaternion targetRotation;

    


    private PlayerController playerController;
    private PlayerRotation playerRotation;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float airKickRoation = 5f;

    public Vector3 airKickDirection;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerRotation = GetComponent<PlayerRotation>();
    }

    // Update is called once per frame
    void Update()
    {
    
        if (!isRotationLocked)
        {
           
            RotatePlayer(playerController.targetDirection); 
            
            
        }
        
        

    }

    private void RotatePlayer(Vector3 targetDirection)
    {
        

        if (targetDirection.sqrMagnitude < 0.001f)
        {

            return;

        }

        float currentRotationSpeed = playerController.isKickActive ? airKickRoation : rotationSpeed;
        Debug.Log(currentRotationSpeed);

        targetRotation = Quaternion.LookRotation(targetDirection);


        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
    }



}
