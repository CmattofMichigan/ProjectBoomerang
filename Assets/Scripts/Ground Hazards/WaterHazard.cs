using UnityEngine;

public class WaterHazard : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player =
            other.GetComponent<CharacterController>();

        if (player != null)
        {
            player.enabled = false;
            player.transform.position = respawnPoint.position;
            player.enabled = true;
        }
    }
}