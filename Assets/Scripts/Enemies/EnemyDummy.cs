using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health);

        if (health <= 0)
        {
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