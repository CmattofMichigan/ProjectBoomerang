using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform startingRespawn;

    void Start()
    {
        CharacterController controller = GetComponent<CharacterController>();

        controller.enabled = false;
        transform.position = startingRespawn.position;
        controller.enabled = true;
    }
}