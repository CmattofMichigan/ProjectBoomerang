using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;
    public WaterHazard waterHazard;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            waterHazard.respawnPoint = respawnPoint;
            Debug.Log("Checkpoint activated!");
        }
    }
}