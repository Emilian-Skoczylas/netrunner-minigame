public interface ISecurityService
{
    float GetCaptureDetectionChance(SecurityLevel level);
    float GetFortifyDetectionChance(SecurityLevel level);
    bool IsDetected(SecurityLevel level);
    float GetCaptureDuration(SecurityLevel level);
}
