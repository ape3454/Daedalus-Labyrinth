using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.GraphToolkit.Editor;
using UnityEditor.MemoryProfiler;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class GridFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
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
    [SerializeField]
    private Vector2Int spawnPosition, bossPosition;
    [SerializeField]
    private bool randomWalkRooms = false;

    protected override void RunProceduralGeneration()
    {
        GridFirstGeneration();
    }

    private void GridFirstGeneration()
    {
        HashSet<Vector2Int> gridPositions = new HashSet<Vector2Int>();

        CreateRooms(gridPositions);
    }

    private void CreateRooms(HashSet<Vector2Int> roomPositions)
    {
        Vector2Int position = startPosition;
        int gridWidth = dungeonWidth / (roomWidth + offset);
        int gridHeight = dungeonHeight / (roomHeight + offset);
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                roomPositions.Add(new Vector2Int(i, j));
                position.y += 1;
            }
            position.x += 1;
        }

        Debug.Log(1);
        int roomToCreateCount = Mathf.RoundToInt(roomPositions.Count * roomPercent);
        Debug.Log(2);
        List<Vector2Int> roomCoords = roomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        List<Vector2Int> mapCoords = roomCoordToMapCoord(roomCoords);
        foreach (var tile in roomCoords) Debug.Log(tile);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(mapCoords);
        }
        else
        {
            floor = CreateSimpleRooms(mapCoords);
        }
        Debug.Log(3);
        foreach (var tile in floor) Debug.Log(tile);
        HashSet<Vector2Int> corridors = ConnectRooms(mapCoords);
        floor.UnionWith(corridors);
        Debug.Log(4);
        foreach (var tile in corridors) print(tile);
        tilemapVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualiser);
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> mapCoords)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        List<List<Vector2Int>> rooms = new List<List<Vector2Int>>();
        foreach (var coord in mapCoords)
        {
            rooms.Add(new List<Vector2Int> { coord });
        }
        HashSet<Vector2Int> connections = new HashSet<Vector2Int>();
        /*
        connections = JoinRooms(rooms);
        */
        return connections;
    }

    private HashSet<Vector2Int> JoinRooms(List<List<Vector2Int>> roomsList)
    {
        List<List<Vector2Int>> trees = roomsList;
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        while (trees.Count > 0)
        {
            List<Vector2Int> tree = trees[Random.Range(0, trees.Count())];
            Vector2Int node = tree[Random.Range(0, tree.Count())];
            Vector2Int newNode = node;
            Vector2Int direction = Direction2D.GetRandomCardinalDirection();
            int smallestTree = trees.Min(y => y.Count);

            while ((newNode.x < dungeonWidth && newNode.x > 0) && (newNode.y < dungeonHeight && newNode.y > 0) | true)
            {
                bool breakLoop = false;
                newNode += direction;
                foreach (var room in trees)
                {
                    if (room == tree) continue;
                    if (room.Count != smallestTree && tree.Count != smallestTree) continue;
                    if (room.Contains(newNode))
                    {
                        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
                        corridor = CreateCorridor(node, newNode);
                        corridors.UnionWith(corridor);

                        trees.Remove(tree);
                        tree.AddRange(room);
                        trees.Remove(room);
                        trees.Add(tree);

                        breakLoop = true;
                        break;
                    }
                }
                if (breakLoop) break;
            }
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int roomCentre, Vector2Int destination)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        var position = roomCentre;
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
            floor.Add(position);
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<Vector2Int> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = room.x - offset; col < room.x + offset; col++)
            {
                for (int row = room.y - offset; row < room.y + offset; row++)
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
            var roomCentre = roomsList[i];
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCentre);
            foreach (var position in roomFloor)
            {
                if ((position.x > roomCentre.x - offset && position.x < roomCentre.x + offset) && (position.y > roomCentre.y - offset && position.y < roomCentre.y + offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private List<Vector2Int> roomCoordToMapCoord(List<Vector2Int> roomCoord)
    {
        List<Vector2Int> mapCoords = new List<Vector2Int>();
        foreach (var coord in roomCoord)
        {
            Vector2Int translateFactor = new Vector2Int (roomWidth + 2 * offset, roomHeight + 2 * offset);
            Vector2Int mapCoord = coord * translateFactor;
            mapCoords.Add(mapCoord);
        }
        return mapCoords;
    }

    private HashSet<Vector2Int> roomCoordToMapCoord(HashSet<Vector2Int> roomCoord)
    {
        HashSet<Vector2Int> mapCoords = new HashSet<Vector2Int>();
        foreach (var coord in roomCoord)
        {
            Vector2Int translateFactor = new Vector2Int(roomWidth + 2 * offset, roomHeight + 2 * offset);
            Vector2Int mapCoord = coord * translateFactor;
            mapCoords.Add(mapCoord);
        }
        return mapCoords;
    }
}