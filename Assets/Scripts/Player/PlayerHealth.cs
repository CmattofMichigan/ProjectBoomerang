
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject gameOverPanel;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (healthBar == null)
        {
            Debug.LogError("HealthBar is not assigned!");
            return;
        }

        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        if (healthBar.fillRect != null)
        {
            healthBar.fillRect.gameObject.SetActive(currentHealth > 0);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;

            if (healthBar.fillRect != null)
            {
                healthBar.fillRect.gameObject.SetActive(currentHealth > 0);
            }
        }

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth == 0)
        {
            Debug.Log("Player died!");

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
            else
                Debug.LogError("GameOverPanel is not assigned!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    [ContextMenu("Test - Take 10 Damage")]
    private void TestDamage()
    {
        if (Application.isPlaying)
            TakeDamage(10);
    }
}
