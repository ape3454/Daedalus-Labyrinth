using System.Collections.Generic;
using UnityEngine;

public class AssembledCoinController : MonoBehaviour
{
    private GameManager gameManager;

    private List<Vector2Int> pathSpawnToBoss;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void ShowShortestPath()
    {
        pathSpawnToBoss = gameManager.spawnToBoss;
        HashSet<Vector2Int> corridorFloor = gameManager.GetPathFloor(pathSpawnToBoss);
        gameManager.PaintNewFloor(corridorFloor);
    }
}