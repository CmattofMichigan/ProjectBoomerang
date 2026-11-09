using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private GameObject abilityPickupPrefab;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health);

        if (health <= 0)
        {
            if (abilityPickupPrefab != null)
            {
                Instantiate(abilityPickupPrefab, transform.position + Vector3.down * 1.5f, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject otherThing = collision.gameObject;
        if (otherThing.CompareTag("Boomerang"))
        {
            TakeDamage(1);
        }
    }
}