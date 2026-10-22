using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

    [Header("Wall Kick")]
    public bool isRotationLocked = false;
    private Vector3 lockedRotationDirection;

    private Quaternion targetRotation;

    


    private PlayerController playerController;
    private PlayerRotation playerRotation;

    [SerializeField] private float rotationSpeed = 10f;

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

        targetRotation = Quaternion.LookRotation(targetDirection);


        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }



}
