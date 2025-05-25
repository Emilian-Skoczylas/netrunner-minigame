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
    public bool EnemyCapture { get; private set; }

    public NodeCaptureTask(NodeHandler fromNode, NodeHandler toNode, float duration = 2f, bool enemyCapture = false)
    {
        Uid = Guid.NewGuid().ToString();

        this.FromNode = !enemyCapture ? fromNode : toNode;
        this.ToNode = !enemyCapture ? toNode : fromNode;

        this.CaptureTime = !enemyCapture ? duration : duration * 1.5f;
        Timer = 0f;
        EnemyCapture = enemyCapture;
    }

    public void Update(float deltaTime)
    {
        Timer += deltaTime;
        float t = Mathf.Clamp01(Timer / CaptureTime);

        ToNode.UpdateUI(FromNode, t, EnemyCapture);
    }

    public void Complete()
    {
        ToNode.UpdateUI(FromNode, 1f, EnemyCapture);

        if (!EnemyCapture)
            ToNode.OnCaptureCompleted();
        else
        {
            FromNode.OnEnemyCaptureCompleted();
        }
    }
}
