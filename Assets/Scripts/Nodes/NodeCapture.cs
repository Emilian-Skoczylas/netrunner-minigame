using System;
using UnityEngine;

public class NodeCapture : NodeTask
{
    public NodeHandler FromNode { get; private set; }
    public NodeHandler ToNode { get; private set; }
    public bool EnemyCapture { get; private set; }

    public NodeCapture(NodeHandler fromNode, NodeHandler toNode, float duration, bool enemyCapture = false)
    {
        Uid = Guid.NewGuid().ToString();

        this.FromNode = !enemyCapture ? fromNode : toNode;
        this.ToNode = !enemyCapture ? toNode : fromNode;

        this.RemainingTime = duration;
        Timer = 0f;
        EnemyCapture = enemyCapture;
    }

    public override void Update(float deltaTime)
    {
        Timer += deltaTime;
        float t = Mathf.Clamp01(Timer / RemainingTime);

        ToNode.UpdateUI(FromNode, t, EnemyCapture);
    }
    public override void Complete()
    {
        ToNode.UpdateUI(FromNode, 1f, EnemyCapture);

        if (!EnemyCapture)
            ToNode.OnCaptureCompleted();
        else
        {
            FromNode.OnEnemyCaptureCompleted();
        }
    }

    public override bool IsSameTask(NodeTask other)
    {
        if (other is not NodeCapture otherCapture)
            return false;

        return FromNode == otherCapture.FromNode &&
               ToNode == otherCapture.ToNode &&
               EnemyCapture == otherCapture.EnemyCapture;
    }
}
