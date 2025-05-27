using UnityEngine;

public class NodeFortify : NodeTask
{
    public NodeHandler Target { get; private set; }

    public NodeFortify(NodeHandler target)
    {
        Target = target;
        RemainingTime = 2f;
    }

    public override bool IsSameTask(NodeTask other)
    {
        if (other is not NodeFortify otherCapture)
            return false;

        return Target == otherCapture.Target;
    }

    public override void Update(float deltaTime)
    {
        Timer += deltaTime;
        float t = Mathf.Clamp01(Timer / RemainingTime);

        Target.UpdateFortifyProgress(t);
    }

    public override void Complete()
    {
        Target.UpdateFortifyProgress(1f);

        Target.OnFortifyCompleted();
    }
}

