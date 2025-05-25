using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class NodesController : ITickable
{
    private List<NodeCaptureTask> _activeTasks = new List<NodeCaptureTask>();

    private NodeHandler _registryNode;
    public bool IsDetected { get; private set; }

    public void Tick()
    {
        for (int i = _activeTasks.Count - 1; i >= 0; i--)
        {
            var task = _activeTasks[i];

            task.Update(Time.deltaTime);

            if (task.IsComplete)
            {
                task.Complete();
                _activeTasks.RemoveAt(i);
            }
        }
    }
    
    public void TryCapture(NodeHandler from, NodeHandler toNode, bool enemy = false)
    {
        if (_activeTasks.Any(t => t.FromNode == from && t.ToNode == toNode && t.EnemyCapture == enemy))
            return;

        var task = new NodeCaptureTask(from, toNode, 2f, enemy);
        _activeTasks.Add(task);
    }

    public void InitRegistryNode(NodeHandler registryNode)
    {
        this._registryNode = registryNode;
    }
    public void Detected()
    {
        if (IsDetected)
            return;

        IsDetected = true;

        if (_registryNode != null)
        {
            foreach (var node in _registryNode.PreviousNeighbors)
            {
                var task = new NodeCaptureTask(_registryNode, node, 2f, true);
                _activeTasks.Add(task);
            }
        }
    }
}
