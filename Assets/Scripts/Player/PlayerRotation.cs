using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private PlayerController playerController;
    
    [SerializeField] private float rotationSpeed = 10f;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer(playerController.targetDirection);
        
    }

    private void RotatePlayer(Vector3 targetDirection)
    {
    
        if (targetDirection.sqrMagnitude < 0.001f)
        {
            
            return;
            
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


}
