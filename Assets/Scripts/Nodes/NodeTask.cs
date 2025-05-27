public abstract class NodeTask
{
    public string Uid { get; set; }
    public float RemainingTime { get; set; }
    public float Timer { get; set; }
    public bool IsComplete => Timer >= RemainingTime;

    public abstract void Update(float deltaTime);
    public abstract void Complete();
    public abstract bool IsSameTask(NodeTask other);
}
