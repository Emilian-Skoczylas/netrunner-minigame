using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Scriptable Objects/LevelData")]
public class LevelSO : ScriptableObject
{
    public LevelData Data;
}


[Serializable] public class LevelData
{
    public string Name;
    public int GridWidth;
    public int GridHeight;
    public List<GridData> Grid;
}

[Serializable] public class GridData
{
    public int X;
    public int Y;
    public NodeType NodeType;
}