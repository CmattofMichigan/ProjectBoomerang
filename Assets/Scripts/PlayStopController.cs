using UnityEngine;

public class PlayStopController : MonoBehaviour
{
    public GameObject objectToControl;

    public void PlayObject()
    {
        objectToControl.SetActive(true);
    }

    public void StopObject()
    {
        objectToControl.SetActive(false);
    }
}