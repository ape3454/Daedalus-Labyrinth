using System;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualiser tilemapVisualiser = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        tilemapVisualiser.Clear();
        RunProceduralGeneration();
    }

    public void RegenerateTiles()
    {
        tilemapVisualiser.Clear();
        ResolveTiles();
    }

    protected abstract void RunProceduralGeneration();
    protected abstract void ResolveTiles();
}
