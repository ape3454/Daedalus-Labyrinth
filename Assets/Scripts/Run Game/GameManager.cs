using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public InputAction reset;
    public InputActionReference re;

    private GridFirstDungeonGenerator dungeonGenerator;
    private CoinManager coinController;
    private SwordController swordController;

    public PlayerController player;

    [HideInInspector]
    public List<Vector2Int> spawnToBoss;
    [HideInInspector]
    public HashSet<Vector2Int> roomsList;
    private List<Vector2Int[]> connections;

    [HideInInspector]
    public Vector2Int spawnCoord, bossCoord;
    [HideInInspector]
    public int gridWidth, gridHeight;

    public void ResetElements()
    {
        Stage2();
        Stage3();
    }

    public void RestartScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        Debug.Log("Restarting Scene...");
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
        reset.Enable();
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
        gridWidth = dungeonGenerator.gridWidth;
        gridHeight = dungeonGenerator.gridHeight;
        return;
    }

    private void Stage3()
    {
        coinController.GetStarted();
        swordController.GetStarted();
        player.runReset();
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
        while (iteration < 10000)
        {
            iteration++;
            if (iteration == 10000)
            {
                Debug.Log("wow");
                Debug.Log(validPaths.Count);
            }
            
            try
            {
                currentPath = paths.First(y => y.Count == paths.Min(x => x.Count));
                if (validPaths.Count != 0 && currentPath.Count > validPaths.Min(y => y.Count)) break;
            }
            catch (InvalidOperationException) // When "Sequence contains no matching element" on 96 (paths.Count == 0)
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

    bool pressed = false;
    void Update()
    {
        if (reset.WasPressedThisFrame())
        {
            Debug.Log("this happened");
            float value = reset.ReadValue<float>();
            if (value < 0)
            {
                Debug.Log("pressed");
                pressed = true;
                RestartScene();
            }
            else if (value > 0)
            {
                Debug.Log("released");
                pressed = false;
            }
        }
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
}