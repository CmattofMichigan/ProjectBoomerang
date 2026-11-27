using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private string mainMenuScene = "Main Menu";

    private bool levelCompleted = false;

    private void Start()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected: " + other.gameObject.name);

        if (levelCompleted)
            return;

        Transform target = other.transform;

        while (target != null)
        {
            if (target.CompareTag("Player"))
            {
                CompleteLevel();
                return;
            }

            target = target.parent;
        }
    }

    private void CompleteLevel()
    {
        levelCompleted = true;

        Debug.Log("Level Complete!");

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Level Complete Panel is not assigned!");
            return;
        }

        // Pause gameplay
        Time.timeScale = 0f;

        // Unlock and display the mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }
}