using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform startingRespawn;
    private static bool spawnFromMenu = false;

    public static void SetSpawnFromMenu()
    {
        spawnFromMenu = true;
    }

    void Start()
    {
        if (!spawnFromMenu) return;

        spawnFromMenu = false;

        CharacterController controller = GetComponent<CharacterController>();

        controller.enabled = false;
        transform.position = startingRespawn.position;
        controller.enabled = true;
    }
}