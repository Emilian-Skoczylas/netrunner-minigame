using UnityEngine;

public class SecurityService : ISecurityService
{
    public float GetDetectionChance(SecurityLevel level)
    {
        return level switch
        {
            SecurityLevel.Level_1 => 0.15f,
            SecurityLevel.Level_2 => 0.25f,
            SecurityLevel.Level_3 => 0.30f,
            SecurityLevel.Level_4 => 0.40f,
            SecurityLevel.Level_5 => 0.50f,
            _ => 0f
        };
    }

    public bool IsDetected(SecurityLevel level)
    {
        bool result = false;

        float detectionChange = GetDetectionChance(level);

        result = Random.value < detectionChange;
        return result;
    }
}
