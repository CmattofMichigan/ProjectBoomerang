using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    private HashSet<AbilityType> unlockedAbilities = new HashSet<AbilityType>();

    public void UnlockAbility(AbilityType ability)
    {
        if (unlockedAbilities.Contains(ability))
        {
            return;
        }

        unlockedAbilities.Add(ability);

    }

    public bool HasAbility(AbilityType ability)
    {
        return unlockedAbilities.Contains(ability);
    }
}
