
using UnityEngine;

public class WaterHazard : MonoBehaviour
{
    public Transform respawnPoint;

    [SerializeField] private int waterDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player =
            other.GetComponent<CharacterController>();

        if (player != null)
        {
            // NEW: Apply water damage to the player.
            PlayerHealth health =
                player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(waterDamage);
            }

            // EXISTING: Preserve original respawn behavior.
            player.enabled = false;
            player.transform.position = respawnPoint.position;
            player.enabled = true;
        }
    }
}
