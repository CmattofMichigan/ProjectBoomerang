
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider healthBar;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar == null)
        {
            Debug.LogError("HealthBar is not assigned!");
            return;
        }

        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (healthBar != null)
            healthBar.value = currentHealth;

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth == 0)
            Debug.Log("Player died!");
    }

    [ContextMenu("Test - Take 10 Damage")]
    private void TestDamage()
    {
        if (Application.isPlaying)
            TakeDamage(10);
    }
}
