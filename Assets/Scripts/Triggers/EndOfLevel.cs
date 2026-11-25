
using UnityEngine;

public class EndOfLevel : MonoBehaviour
{
    private bool levelCompleted = false;

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
                levelCompleted = true;

                Debug.Log("Level Complete!");

                return;
            }

            target = target.parent;
        }
    }
}
