using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float maxFallSpeed = -60f;

    private CapsuleCollider capsuleCollider;

    private Vector3 velocity;
    
    private float groundCheckDistance = 0.1f;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckGround();
        ApplyGravity();
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private bool CheckCollision(Vector3 direction, float distance, out RaycastHit hitInfo)
    {
    
        Vector3 center = capsuleCollider.bounds.center;
        float halfHeight = capsuleCollider.bounds.extents.y;
        Vector3 point1 = center + Vector3.up * (halfHeight - capsuleCollider.radius);
        Vector3 point2 = center + Vector3.down * (halfHeight - capsuleCollider.radius);

        return Physics.CapsuleCast(point1, point2, capsuleCollider.radius, direction, out hitInfo, distance);
    }

    private bool CheckGround()
    {
        RaycastHit hitInfo;

        if (!CheckCollision(Vector3.down, groundCheckDistance, out hitInfo))
        {
            return false;
        }
        return hitInfo.normal == Vector3.up;
        
    }

    private void ApplyGravity()
    {

        if (isGrounded)
        {
            velocity.y = 0;
            Debug.Log("Grounded, velocity.y set to 0");
        } 

        else
        {

            if (velocity.y > maxFallSpeed)
            {
                velocity.y += gravity * Time.deltaTime;
                if (velocity.y < maxFallSpeed)
                {
                    velocity.y = maxFallSpeed;
                }
            }
            
        }
    }



}
