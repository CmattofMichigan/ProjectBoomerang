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

    private CapsuleCollider capsuleCollider;

    private Vector3 velocity;
     
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void checkGround()
    {
        //todo: Implement ground check logic
    }
}
