using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    [SerializeField] private AbilityType abilityToUnlock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AbilityManager abilityManager = other.GetComponentInParent<AbilityManager>();

        if (abilityManager == null)
        {
            return;
        }

        abilityManager.UnlockAbility(abilityToUnlock);

        Destroy(gameObject);


    }
}
