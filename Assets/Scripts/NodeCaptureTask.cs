using System;
using UnityEngine;

[Serializable]
public class NodeCaptureTask
{
    public string Uid { get; private set; }
    public float CaptureTime { get; private set; }
    public float Timer { get; private set; }

    public bool IsComplete => Timer >= CaptureTime;

    public NodeHandler FromNode { get; private set; }
    public NodeHandler ToNode { get; private set; }

    public NodeCaptureTask(NodeHandler fromNode, NodeHandler toNode, float duration = 2f)
    {
        Uid = new Guid().ToString();

        this.FromNode = fromNode;
        this.ToNode = toNode;

        this.CaptureTime = duration;
        Timer = 0f;
    }

    public void Update(float deltaTime)
    {
        Timer += deltaTime;
        float t = Mathf.Clamp01(Timer / CaptureTime);
        ToNode.UpdateUI(FromNode, t);
    }

    public void Complete()
    {
        ToNode.UpdateUI(FromNode, 1f);
        ToNode.OnCaptureCompleted();
    }
}
