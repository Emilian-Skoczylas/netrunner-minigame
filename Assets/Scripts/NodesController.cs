using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NodesController : IInitializable, ITickable
{
    [SerializeField] private List<NodeCaptureTask> activeTasks = new List<NodeCaptureTask>();
    
    public void Initialize()
    {
        Debug.Log("NodesController init");
    }

    public void Tick()
    {
        for (int i = activeTasks.Count - 1; i >= 0; i--)
        {
            var task = activeTasks[i];

            task.Update(Time.deltaTime);

            if (task.IsComplete)
            {
                task.Complete();
                activeTasks.RemoveAt(i);
            }
        }
    }

    public void TryCapture(NodeHandler from, NodeHandler toNode)
    {
        if (activeTasks.Exists(node => node.FromNode == from && node.ToNode == toNode))
            return;

        var task = new NodeCaptureTask(from, toNode);
        activeTasks.Add(task);
    }
}
