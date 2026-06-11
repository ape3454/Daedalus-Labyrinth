using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GridFirstDungeonGenerator dungeonGenerator;
    private CoinController coinController;

    public PlayerController player;

    public List<Vector2Int> spawnToBoss;
    public HashSet<Vector2Int> roomsList;
    private List<Vector2Int[]> connections;

    public Vector2Int spawnCoord, bossCoord;

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restarting Scene...");
    }

    private void Awake()
    {
        dungeonGenerator = this.GetComponent<GridFirstDungeonGenerator>();
        coinController = GameObject.Find("CoinFragments").GetComponent<CoinController>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Stage1();
        Stage2();
        Stage3();
    }

    private void Stage1()
    {
        dungeonGenerator.GenerateDungeon();
        return;
    }

    private void Stage2()
    {
        spawnCoord = dungeonGenerator.spawnPosition;
        bossCoord = dungeonGenerator.bossPosition;
        connections = dungeonGenerator.connections;
        roomsList = dungeonGenerator.roomCoords.ToHashSet();
        return;
    }

    private void Stage3()
    {
        coinController.GetStarted();
        spawnToBoss = GetShortestPath(spawnCoord, bossCoord);
        return;
    }

    public List<Vector2Int> GetShortestPath(Vector2Int firstRoom, Vector2Int secondRoom, bool formatted = false)
    {
        Vector2Int room1 = (formatted) ? firstRoom : dungeonGenerator.roomCoordToMapCoord(firstRoom);
        Vector2Int room2 = (formatted) ? secondRoom : dungeonGenerator.roomCoordToMapCoord(secondRoom);

        List<Vector2Int> path = new List<Vector2Int>() { room1 };
        if (room1 == room2) return path;

        List<List<Vector2Int>> paths = new List<List<Vector2Int>>() { path };
        List<List<Vector2Int>> validPaths = new List<List<Vector2Int>>();
        List<Vector2Int> currentPath;

        int iteration = 0;
        while (iteration < 1000)
        {
            iteration++;
            if (iteration == 1000)
            {
                Debug.Log("wow");
                Debug.Log(validPaths.Count);
            }
            
            try
            {
                currentPath = paths.First(y => y.Count == paths.Min(x => x.Count));
                if (validPaths.Count != 0 && currentPath.Count > validPaths.Min(y => y.Count)) break;
            }
            catch (InvalidOperationException) // When "Sequence contains no matching element" on 84 (paths.Count == 0)
            {
                break;
            }
            Vector2Int endOfPath = currentPath[^1];
            List<Vector2Int[]> pathways = connections.FindAll(y => y.Contains(endOfPath));
            foreach (var corridor in pathways)
            {
                Vector2Int otherNode = corridor.First(y => y != endOfPath);
                if (currentPath.Contains(otherNode)) continue;

                List<Vector2Int> newPath = currentPath.Append(otherNode).ToList();
                if (otherNode == room2)
                {
                    validPaths.Add(newPath);
                }
                else
                {
                    paths.Add(newPath);
                }
            }
            paths.RemoveAll(y => y.SequenceEqual(currentPath));
        }

        path = validPaths.OrderBy(y => y.Count).First();
        return path;
    }

    public HashSet<Vector2Int> FindIntersectionsByConnections(int numberOfIntersections)
    {
        HashSet<Vector2Int> rooms = new HashSet<Vector2Int>();
        foreach (Vector2Int room in roomsList)
        {
            HashSet<Vector2Int[]> corridors = connections.FindAll(y => y.Contains(dungeonGenerator.roomCoordToMapCoord(room))).ToHashSet();
            if (corridors.Count == numberOfIntersections) rooms.Add(room);
        }
        return rooms;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReloadScene()
    {
        dungeonGenerator.RegenerateTiles();
    }
}
