using UnityEngine;

public class SecurityService : ISecurityService
{
    private const float BaseCaptureTime = 1f;
    private const float GrowthCaptureFactor = 1.5f;
    public float GetCaptureDetectionChance(SecurityLevel level)
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

    public float GetCaptureDuration(SecurityLevel securityLevel)
    {
        int levelInt = (int)securityLevel;

        var result = BaseCaptureTime * Mathf.Pow(GrowthCaptureFactor, levelInt);
        return result;
    }

    public float GetFortifyDetectionChance(SecurityLevel level)
    {
        return level switch
        {
            SecurityLevel.Level_1 => 0.30f,
            SecurityLevel.Level_2 => 0.45f,
            SecurityLevel.Level_3 => 0.60f,
            SecurityLevel.Level_4 => 0.75f,
            SecurityLevel.Level_5 => 0.90f,
            _ => 0f
        };
    }

    public bool IsDetected(SecurityLevel level)
    {
        bool result = false;

        float detectionChange = GetCaptureDetectionChance(level);

        result = Random.value < detectionChange;
        return result;
    }
}
