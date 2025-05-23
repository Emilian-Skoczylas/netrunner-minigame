public interface ISecurityService
{
    float GetDetectionChance(SecurityLevel level);
    bool IsDetected(SecurityLevel level);
}
