using UnityEngine;

// This script should be placed on the HITBOX of the lever

public class LeverInteract : MonoBehaviour
{
    public Transform pivot; // The pivot point around which the lever rotates
    public ActivationTarget target; // The thing that will be activated by the lever
    [SerializeField] private bool singleTrigger = false; // Whether the lever can only be triggered once
    private bool isTriggered;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boomerang") && (!isTriggered)){
            isTriggered = true;
            pivot.rotation = Quaternion.Euler(30, 0, 0);
            target.activate();
        }
        else if (other.gameObject.CompareTag("Boomerang") && isTriggered && !singleTrigger){
            isTriggered = false;
            pivot.rotation = Quaternion.Euler(-30, 0, 0);
            target.activate();
        }
    }
}
