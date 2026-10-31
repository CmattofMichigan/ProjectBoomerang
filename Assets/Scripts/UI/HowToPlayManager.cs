
using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject howToPlayPanel;

    public void OpenHowToPlay()
    {
        Debug.Log("How to Play button clicked!");
        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }
}
