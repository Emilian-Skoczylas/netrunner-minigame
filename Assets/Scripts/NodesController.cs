using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class NodesController : ITickable
{
    [Inject] private ISecurityService _securityService;

    private List<NodeTask> _activeTasks = new List<NodeTask>();

    private NodeHandler _diagnosisNode;
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
        float captureDuration = _securityService.GetCaptureDuration(toNode.SecurityLevel);

        var newTask = new NodeCapture(from, toNode, captureDuration, enemy);

        if (_activeTasks.Any(existing => existing.IsSameTask(newTask)))
            return;

        _activeTasks.Add(newTask);
    }

    public void InitDiagnosisNode(NodeHandler diagnosisNode)
    {
        this._diagnosisNode = diagnosisNode;
    }
    public void Detected()
    {
        if (IsDetected)
            return;

        IsDetected = true;

        if (_diagnosisNode != null)
        {
            foreach (var node in _diagnosisNode.PreviousNeighbors)
            {
                float captureDuration = _securityService.GetCaptureDuration(node.SecurityLevel);
                var task = new NodeCapture(_diagnosisNode, node, captureDuration, true);
                _activeTasks.Add(task);
            }
        }
    }

    public void TryFortify(NodeHandler node)
    {
        var newTask = new NodeFortify(node);

        if (_activeTasks.Any(existing => existing.IsSameTask(newTask)))
            return;

        _activeTasks.Add(newTask);
    }
}
