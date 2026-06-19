using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }


    [SerializeField]
    private int roomWidth = 4, roomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(1, 10)]
    private int offset;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.5f;
    [SerializeField]
    private int extraCorridorCount;
    public Vector2Int spawnPosition, bossPosition;
    [SerializeField]
    private int bossCorridorLength = 2;
    [SerializeField]
    private bool randomWalkRooms = false;

    public int gridWidth, gridHeight;
    public List<Vector2Int> roomCoords;
    private List<Vector2Int> mapCoords;
    private HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

    public List<Vector2Int[]> connections { get { return branches; } }
    List<Vector2Int[]> branches = new List<Vector2Int[]>();

    private List<Vector2Int> potentialIntersectionCoords, intersectionCoords;

    public Dictionary<Vector2Int, string> importantTiles;

    protected override void RunProceduralGeneration()
    {
        GridFirstGeneration();
    }

    protected override void ResolveTiles()
    {
        RenegerateTiles();
    }

    private void RenegerateTiles()
    {
        tilemapVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualiser);
    }

    private void GridFirstGeneration()
    {
        HashSet<Vector2Int> gridPositions = new HashSet<Vector2Int>();

        CreateRooms(gridPositions);
        GenerateMap();
    }

    private void GenerateMap()
    {
        importantTiles = new Dictionary<Vector2Int, string>();

        Vector2Int mapSpawnEdge = roomCoordToMapCoord(spawnPosition) + Vector2Int.down * Mathf.FloorToInt(roomHeight / 2);
        importantTiles.Add(mapSpawnEdge, "spawn");

        //importantTiles.Add(mapSpawnEdge + Vector2Int.down, "entranceDoor");

        Vector2Int mapBossSpawn = roomCoordToMapCoord(bossPosition);
        importantTiles.Add(mapBossSpawn, "bossSpawn");

        //Add corridor

        tilemapVisualiser.PaintAlternativeTiles(importantTiles);
    }

    private void CreateRooms(HashSet<Vector2Int> roomPositions)
    {
        gridWidth = (dungeonWidth + 2 * offset + 1) / ((int)(Mathf.Floor(roomWidth / 2) * 2 + 1) + 2 * offset + 1);
        gridHeight = (dungeonHeight + 2 * offset + 1) / ((int)(Mathf.Floor(roomHeight / 2) * 2 + 1) + 2 * offset + 1);
        spawnPosition = new Vector2Int(Mathf.RoundToInt(gridWidth / 2), 0);
        bossPosition = new Vector2Int(Mathf.RoundToInt(gridWidth / 2), Mathf.RoundToInt(2 * gridHeight / 3));
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                roomPositions.Add(new Vector2Int(i, j));
            }
        }

        int roomToCreateCount = Mathf.RoundToInt(roomPositions.Count * roomPercent);
        List<Vector2Int> randomOrderRooms = roomPositions.OrderBy(x => Guid.NewGuid()).ToList();
        roomCoords = randomOrderRooms.Take(roomToCreateCount).ToList();
        roomCoords = roomCoords.Union(new HashSet<Vector2Int>() { spawnPosition, bossPosition, bossPosition + Vector2Int.up * (bossCorridorLength + 1) }).ToList();
        for (int i = 0; i < bossCorridorLength; i++)
        {
            roomCoords.Remove(bossPosition + Vector2Int.up * (i + 1));
        }

        potentialIntersectionCoords = roomCoordToMapCoord(randomOrderRooms.Except(roomCoords).ToList());
        intersectionCoords = new List<Vector2Int>();
        mapCoords = roomCoordToMapCoord(roomCoords);
        
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(mapCoords);
        }
        else
        {
            floor = CreateSimpleRooms(mapCoords);
        } 
        HashSet<Vector2Int> corridors = ConnectRooms();
        floor.UnionWith(corridors);
        tilemapVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualiser);
    }

    private HashSet<Vector2Int> ConnectRooms()
    {
        List<List<Vector2Int>> rooms = new List<List<Vector2Int>>();
        foreach (var coord in mapCoords)
        {
            rooms.Add(new List<Vector2Int> { coord });
        }
        HashSet<Vector2Int> connections = new HashSet<Vector2Int>();

        connections = JoinRooms(rooms);
        connections.UnionWith(CreateExtraCorridors());
        
        return connections;
    }

    private IEnumerable<Vector2Int> CreateExtraCorridors()
    {
        List<Vector2Int> nodes = mapCoords.Union(intersectionCoords).ToList();
        List<Vector2Int> exceptions = roomCoordToMapCoord(new List<Vector2Int>() { spawnPosition, bossPosition });
        for (int i = 1; i <= bossCorridorLength; i++) exceptions.Add(roomCoordToMapCoord(new Vector2Int(bossPosition.x, bossPosition.y + i)));
        nodes.RemoveAll(y => exceptions.Contains(y));
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        int createdCorridors = 0;

        for (int i = 0; i < extraCorridorCount; i++)
        {
            int iteration = 0;
            while (createdCorridors == i && iteration < 1000)
            {
                iteration++;
                Vector2Int node = nodes[Random.Range(0, nodes.Count())];
                Vector2Int newNode = node;
                Vector2Int direction = Direction2D.GetRandomCardinalDirection();
                int nodeDistance = 0;

                while ((newNode.x < dungeonWidth && newNode.x >= 0) && (newNode.y < dungeonHeight && newNode.y >= 0))
                {
                    newNode += direction;

                    if (newNode.x == roomCoordToMapCoord(bossPosition).x && newNode.y >= roomCoordToMapCoord(bossPosition).y && newNode.y < roomCoordToMapCoord(bossPosition + Vector2Int.up * (bossCorridorLength + 1)).y) break;
                    if (newNode == roomCoordToMapCoord(spawnPosition)) break;
                    if (potentialIntersectionCoords.Contains(newNode)) nodeDistance++;

                    if (nodes.Contains(newNode))
                    {
                        if (branches.Any(y => y.Contains(node) && y.Contains(newNode))) break;
                        if (mapCoords.Contains(newNode)) nodeDistance++;
                        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();

                        int distanceBetweenNodes = (int)roomCoordToMapCoord(Vector2Int.up).magnitude;
                        for (int j = 1; j <= nodeDistance; j++)
                        {
                            corridor = CreateCorridor(node + (j - 1) * direction * distanceBetweenNodes, node + j * direction * distanceBetweenNodes);
                            corridors.UnionWith(corridor);
                        }
                        nodes.Union(intersectionCoords);
                        createdCorridors++;
                        break;
                    }
                }
            }
        }
        return corridors;
    }

    private HashSet<Vector2Int> JoinRooms(List<List<Vector2Int>> roomsList)
    {
        List<List<Vector2Int>> trees = roomsList;
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        List<Vector2Int> preset = roomCoordToMapCoord(new List<Vector2Int>() { spawnPosition, bossPosition, bossPosition + Vector2Int.up * (bossCorridorLength + 1) });
        bool presetFinished = false;

        int iteration = 0;
        while (trees.Count > 1 && iteration < 10000)
        {
            iteration++;
            if (iteration == 10000)
            {
                Debug.Log("wow");
                gameManager.RestartScene();
                break;
            }
            List<Vector2Int> tree;
            Vector2Int node, newNode, direction;

            int smallestTree = trees.Min(y => y.Count);
            int smallTree = trees.OrderBy(y => y.Count).ElementAt(1).Count();

            if (presetFinished)
            {
                tree = trees[trees.FindIndex(y => y.Count == smallestTree | y.Count == smallTree)];
                node = tree[Random.Range(0, tree.Count())];
                newNode = node;
                direction = Direction2D.GetRandomCardinalDirection();
                if (node == roomCoordToMapCoord(bossPosition) | node == roomCoordToMapCoord(spawnPosition)) continue;
            }
            else
            {
                tree = new List<Vector2Int>() { preset[0] };
                //Debug.Log("Preset: " + tree[0]);
                node = preset[0];
                newNode = node;
                direction = Vector2Int.up;
                preset.RemoveAt(0);
            }
            if (preset.Count == 0) presetFinished = true;

            int nodeDistance = 0;

            List<Vector2Int> room;
            while ((newNode.x < dungeonWidth && newNode.x >= 0) && (newNode.y < dungeonHeight && newNode.y >= 0))
            {
                newNode += direction;
                if (intersectionCoords.Contains(newNode)) break;

                if (newNode.x == roomCoordToMapCoord(bossPosition).x && newNode.y >= roomCoordToMapCoord(bossPosition).y && newNode.y < roomCoordToMapCoord(bossPosition + Vector2Int.up * (bossCorridorLength + 1)).y && presetFinished) break;
                if (newNode == roomCoordToMapCoord(spawnPosition)) break;
                if (tree.Contains(newNode)) break;

                room = trees.FirstOrDefault(y => y.Contains(newNode));

                if (potentialIntersectionCoords.Contains(newNode)) nodeDistance++;
                if (room == null) continue;
                if (room.Count != smallestTree && tree.Count > smallTree) break;

                HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
                nodeDistance++;
                int distanceBetweenNodes = (int)roomCoordToMapCoord(Vector2Int.up).magnitude;

                for (int i = 1; i <= nodeDistance; i++)
                {
                    corridor = CreateCorridor(node + (i - 1) * direction * distanceBetweenNodes, node + i * direction * distanceBetweenNodes);
                    corridors.UnionWith(corridor);
                }

                if (!trees.Remove(tree))
                {
                    int item = trees.FindIndex(y => y.SequenceEqual(tree));
                    if (item != -1) trees.RemoveAt(item);
                }
                tree.AddRange(room);
                trees.Remove(room);
                trees.Add(tree);
                break;
            }   
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int roomCentre, Vector2Int destination)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        Vector2Int position = roomCentre;
        floor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            
            if (potentialIntersectionCoords.Contains(position))
            {
                intersectionCoords.Add(position);
            }

            floor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            if (potentialIntersectionCoords.Contains(position))
            {
                intersectionCoords.Add(position);
            }

            floor.Add(position);
        }
        Vector2Int[] branch = new Vector2Int[2] { roomCentre, destination };
        branches.Add(branch);
        return floor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<Vector2Int> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = room.x - Mathf.FloorToInt(roomWidth / 2); col <= room.x + Mathf.FloorToInt(roomWidth / 2); col++)
            {
                for (int row = room.y - Mathf.FloorToInt(roomHeight / 2); row <= room.y + Mathf.FloorToInt(roomHeight / 2); row++)
                {
                    Vector2Int position = new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<Vector2Int> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            Vector2Int roomCentre = roomsList[i];
            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomCentre);
            foreach (var position in roomFloor)
            {
                if ((position.x >= roomCentre.x - (int)Math.Floor((decimal)(roomWidth / 2)) && position.x <= roomCentre.x + (int)Math.Floor((decimal)(roomWidth / 2))) && (position.y >= roomCentre.y - (int)Math.Floor((decimal)(roomHeight / 2)) && position.y <= roomCentre.y + (int)Math.Floor((decimal)(roomHeight / 2))))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
    
    public List<Vector2Int> roomCoordToMapCoord(List<Vector2Int> roomCoords)
    {
        List<Vector2Int> mapCoords = new List<Vector2Int>();
        foreach (var coord in roomCoords)
        {
            Vector2Int translateFactor = new Vector2Int (roomWidth + 2 * offset + 1, roomHeight + 2 * offset + 1);
            Vector2Int mapCoord = coord * translateFactor;
            mapCoords.Add(mapCoord);
        }
        return mapCoords;
    }

    public Vector2Int roomCoordToMapCoord(Vector2Int roomCoord)
    {
        Vector2Int translateFactor = new Vector2Int(roomWidth + 2 * offset + 1, roomHeight + 2 * offset + 1);
        Vector2Int mapCoord = roomCoord * translateFactor;
        return mapCoord;
    }
}