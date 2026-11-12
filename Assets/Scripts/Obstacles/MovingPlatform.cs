
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 movementOffset = new Vector3(5f, 0f, 0f);
    [SerializeField] private float moveSpeed = 1f;

    private Vector3 startPosition;
    private Vector3 previousPosition;
    private Vector3 platformMovement;

    void Start()
    {
        startPosition = transform.position;
        previousPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.PingPong(Time.time * moveSpeed, 1f);

        transform.position = Vector3.Lerp(
            startPosition,
            startPosition + movementOffset,
            movement
        );

        platformMovement = transform.position - previousPosition;
        previousPosition = transform.position;
    }

    public Vector3 GetMovement()
    {
        return platformMovement;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.currentPlatform = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null && player.currentPlatform == this)
        {
            player.currentPlatform = null;
        }
    }
}
