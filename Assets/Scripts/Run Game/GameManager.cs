using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GridFirstDungeonGenerator dungeonGenerator;
    private CoinManager coinController;
    private SwordController swordController;

    public PlayerController player;
    public Minotaur minotaur;

    [HideInInspector]
    public List<Vector2Int> spawnToBoss;
    [HideInInspector]
    public HashSet<Vector2Int> roomsList;
    private List<Vector2Int[]> connections;
    private HashSet<Vector2Int> roomsFloor;
    private HashSet<Vector2Int> corridorsFloor;

    [HideInInspector]
    public Vector2Int spawnCoord, bossCoord;
    [HideInInspector]
    public int gridWidth, gridHeight;

    public void ResetElements()
    {
        Stage2();
        Stage3();
    }

    public void EndGame()
    {
        minotaur.enabled = false;
        player.enabled = false;
    }

    public void RestartScene()
    {
        SceneManager.LoadSceneAsync("MenuScene");
    }

    private void Awake()
    {
        dungeonGenerator = this.GetComponent<GridFirstDungeonGenerator>();
        coinController = GameObject.Find("CoinFragments").GetComponent<CoinManager>();
        swordController = GameObject.Find("SwordController").GetComponent<SwordController>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NewScene();
    }

    void NewScene()
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
        roomsFloor = dungeonGenerator.floor;
        corridorsFloor = dungeonGenerator.corridorFloor;
        gridWidth = dungeonGenerator.gridWidth;
        gridHeight = dungeonGenerator.gridHeight;
        return;
    }

    private void Stage3()
    {
        coinController.GetStarted();
        swordController.GetStarted();
        
        player.resetRun();
        player.MoveTo((Vector2)roomCoordToMapCoord(spawnCoord) + new Vector2(0.5f, -2));

        minotaur.resetRun();
        minotaur.MoveTo((Vector2)roomCoordToMapCoord(bossCoord) + new Vector2(0.5f, 0));
        
        spawnToBoss = GetShortestPath(spawnCoord, bossCoord);
        return;
    }

    public List<Vector2Int> GetShortestPath(Vector2Int firstRoom, Vector2Int secondRoom, bool formatted = false)
    {
        Vector2Int room1 = (formatted) ? firstRoom : roomCoordToMapCoord(firstRoom);
        Vector2Int room2 = (formatted) ? secondRoom : roomCoordToMapCoord(secondRoom);

        List<Vector2Int> path = new List<Vector2Int>() { room1 };
        if (room1 == room2) return path;

        List<List<Vector2Int>> paths = new List<List<Vector2Int>>() { path };
        List<List<Vector2Int>> validPaths = new List<List<Vector2Int>>();
        List<Vector2Int> currentPath;

        int iteration = 0;
        while (iteration < 10000)
        {
            iteration++;
            
            try
            {
                currentPath = paths.First(y => y.Count == paths.Min(x => x.Count));
                if (validPaths.Count != 0 && currentPath.Count > validPaths.Min(y => y.Count)) break;
            }
            catch (InvalidOperationException) // When "Sequence contains no matching element" on 115 (paths.Count == 0)
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
            HashSet<Vector2Int[]> corridors = connections.FindAll(y => y.Contains(roomCoordToMapCoord(room))).ToHashSet();
            if (corridors.Count == numberOfIntersections) rooms.Add(room);
        }
        return rooms;
    }

    // Update is called once per frame

    bool pressed = false;
    void Update()
    {

    }

    public List<Vector2Int> roomCoordToMapCoord(List<Vector2Int> roomCoords)
    {
        return dungeonGenerator.roomCoordToMapCoord(roomCoords);
    }

    public Vector2Int roomCoordToMapCoord(Vector2Int roomCoord)
    {
        return dungeonGenerator.roomCoordToMapCoord(roomCoord);
    }

    public void printNestedList(List<List<Vector2Int>> nestedList)
    {
        string who = "";
        foreach (var x in nestedList)
        {
            foreach (var y in x) who += y + " --> ";
            who += " |--| ";
        }
        print(who);
    }

    public HashSet<Vector2Int> GetPathFloor(List<Vector2Int> path)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        List<(Vector2Int, Vector2Int)> connections = new List<(Vector2Int, Vector2Int)>();
        for (int i = 0; i < path.Count - 1; i++)
        {
            connections.Add((path[i], path[i + 1]));
        }

        foreach ((Vector2Int, Vector2Int) connection in connections)
        {
            if (connection.Item1.x == connection.Item2.x)
            {
                int direction = (connection.Item1.y < connection.Item2.y) ? 1 : -1;
                for (int i = connection.Item1.y; i != connection.Item2.y; i += direction)
                {
                    Vector2Int position = new Vector2Int(connection.Item1.x, i);
                    floor.Add(position);
                }
            }
            else if (connection.Item1.y == connection.Item2.y)
            {
                int direction = (connection.Item1.x < connection.Item2.x) ? 1 : -1;
                for (int i = connection.Item1.x; i != connection.Item2.x; i += direction)
                {
                    Vector2Int position = new Vector2Int(i, connection.Item1.y);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    public HashSet<Vector2Int> GetUnnecessaryFloor(HashSet<Vector2Int> exceptFloor)
    {
        HashSet<Vector2Int> unnecessaryFloor = corridorsFloor;
        unnecessaryFloor.ExceptWith(exceptFloor);
        unnecessaryFloor.ExceptWith(roomsFloor);
        return unnecessaryFloor;
    }

    public void PaintNewFloor(HashSet<Vector2Int> corridorFloor)
    {
        HashSet<Vector2Int> totalFloor = roomsFloor.Union(corridorFloor).ToHashSet();
        dungeonGenerator.PaintFloor(totalFloor);
    }
}