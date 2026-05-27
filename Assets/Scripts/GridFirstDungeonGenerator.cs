using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
using UnityEngine;
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
    public Vector2Int spawnPosition, bossPosition;
    [SerializeField]
    private int bossCorridorLength = 2;
    [SerializeField]
    private bool randomWalkRooms = false;

    private int gridWidth, gridHeight;
    private List<Vector2Int> roomCoords, mapCoords;

    private List<Vector2Int[]> branches = new List<Vector2Int[]>();

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
        gridWidth = (dungeonWidth + 2 * offset + 1) / ((int)(Mathf.Floor(roomWidth / 2) * 2 + 1) + 2 * offset + 1);
        gridHeight = (dungeonHeight + 2 * offset + 1) / ((int)(Mathf.Floor(roomHeight / 2) * 2 + 1) + 2 * offset + 1);
        spawnPosition = new Vector2Int(Mathf.RoundToInt(gridWidth / 2), 0);
        bossPosition = new Vector2Int(Mathf.RoundToInt(gridWidth / 2), Mathf.RoundToInt(2 * gridHeight / 3));
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                roomPositions.Add(new Vector2Int(i, j));
                position.y += 1;
            }
            position.x += 1;
        }

        int roomToCreateCount = Mathf.RoundToInt(roomPositions.Count * roomPercent);
        List<Vector2Int> randomOrderRooms = roomPositions.OrderBy(x => Guid.NewGuid()).ToList();
        roomCoords = randomOrderRooms.Take(roomToCreateCount).ToList();
        roomCoords = roomCoords.Union(new HashSet<Vector2Int>() { spawnPosition, bossPosition, bossPosition + Vector2Int.up * (bossCorridorLength + 1) }).ToList();
        Vector2Int pos = bossPosition;
        for (int i = 0; i < bossCorridorLength; i++)
        {
            pos += Vector2Int.up;
            roomCoords.Remove(pos);
        }

        List<Vector2Int> potentialIntersectionCoords = roomCoordToMapCoord(randomOrderRooms.Except(roomCoords).ToList());
        List<Vector2Int> intersectionCoords = new List<Vector2Int>();
        mapCoords = roomCoordToMapCoord(roomCoords);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(mapCoords);
        }
        else
        {
            floor = CreateSimpleRooms(mapCoords);
        } 
        HashSet<Vector2Int> corridors = ConnectRooms(mapCoords, potentialIntersectionCoords, ref intersectionCoords);
        floor.UnionWith(corridors);
        tilemapVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualiser);
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> mapCoords, List<Vector2Int> potentialIntersectionCoords, ref List<Vector2Int> intersectionCoords)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        List<List<Vector2Int>> rooms = new List<List<Vector2Int>>();
        foreach (var coord in mapCoords)
        {
            rooms.Add(new List<Vector2Int> { coord });
        }
        HashSet<Vector2Int> connections = new HashSet<Vector2Int>();

        connections = JoinRooms(rooms, potentialIntersectionCoords, ref intersectionCoords);
        connections.UnionWith(CreateExtraCorridors(mapCoords, potentialIntersectionCoords, ref intersectionCoords));
        
        return connections;
    }

    private IEnumerable<Vector2Int> CreateExtraCorridors(List<Vector2Int> mapCoords, List<Vector2Int> potentialIntersectionCoords, ref List<Vector2Int> intersectionCoords)
    {
        List<Vector2Int> nodes = mapCoords.Union(intersectionCoords).ToList();
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

                if (node == bossPosition) continue;
                while ((newNode.x < dungeonWidth && newNode.x >= 0) && (newNode.y < dungeonHeight && newNode.y >= 0))
                {
                    newNode += direction;
                    if (newNode == bossPosition) break;

                    if (nodes.Contains(newNode))
                    {
                        if (branches.Any(y => y.Contains(node) && y.Contains(newNode))) break;
                        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
                        corridor = CreateCorridor(node, newNode, potentialIntersectionCoords, ref intersectionCoords);
                        Debug.Log(string.Join(" ", corridor));
                        corridors.UnionWith(corridor);
                        createdCorridors++;
                        Debug.Log(node + " " + newNode);
                        break;
                    }
                }
            }
        }
        return corridors;
    }

    private HashSet<Vector2Int> JoinRooms(List<List<Vector2Int>> roomsList, List<Vector2Int> potentialIntersectionCoords,  ref List<Vector2Int> intersectionCoords)
    {
        List<List<Vector2Int>> trees = roomsList;
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        List<Vector2Int> preset = roomCoordToMapCoord(new List<Vector2Int>() { spawnPosition, bossPosition, bossPosition + Vector2Int.up * (bossCorridorLength + 1) });
        bool presetFinished = false;

        int iteration = 0;
        while (trees.Count > 1 && iteration < 100000)
        {
            iteration++;
            List<Vector2Int> tree;
            Vector2Int node, newNode, direction;
            if (presetFinished)
            {
                tree = trees[Random.Range(0, trees.Count())];
                node = tree[Random.Range(0, tree.Count())];
                newNode = node;
                direction = Direction2D.GetRandomCardinalDirection();
                if (node == roomCoordToMapCoord(bossPosition)) continue;
            }
            else
            {
                tree = new List<Vector2Int>() { preset[0] };
                node = preset[0];
                newNode = node;
                direction = (preset[0] == roomCoordToMapCoord(bossPosition)) ? Vector2Int.up : Direction2D.GetRandomCardinalDirection();
                preset.RemoveAt(0);
            }
            if (preset.Count == 0) presetFinished = true;

            int smallestTree = trees.Min(y => y.Count);
            int smallTree = trees.OrderBy(y => y.Count).ElementAt(1).Count();

            int iteration1 = 0;
            List<Vector2Int> room;
            while ((newNode.x < dungeonWidth && newNode.x >= 0) && (newNode.y < dungeonHeight && newNode.y >= 0) && iteration1 < 1000)
            {
                iteration1++;
                newNode += direction;
                if (intersectionCoords.Contains(newNode)) break;
                if (newNode == roomCoordToMapCoord(bossPosition)) break;
                if (tree.Contains(newNode)) break;

                room = trees.FirstOrDefault(y => y.Contains(newNode));

                if (room == null) continue;
                if (room.Count != smallestTree && tree.Count > smallTree) break;

                HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
                corridor = CreateCorridor(node, newNode, potentialIntersectionCoords, ref intersectionCoords);
                corridors.UnionWith(corridor);

                if (!trees.Remove(tree))
                {
                    int item = trees.FindIndex(y => y[0].x == tree[0].x && y[0].y == tree[0].y && y.Count == 1);
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

    private void printNestedList(List<List<Vector2Int>> nestedList)
    {
        string who = "";
        foreach (var x in nestedList) foreach (var y in x) who += y + " --> ";
        print(who);
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int roomCentre, Vector2Int destination, List<Vector2Int> potentialIntersectionCoords, ref List<Vector2Int> intersectionCoords)
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
            for (int col = room.x - (int)Math.Floor((decimal)(roomWidth / 2)); col <= room.x + (int)Math.Floor((decimal)(roomWidth / 2)); col++)
            {
                for (int row = room.y - (int)Math.Floor((decimal)(roomHeight / 2)); row <= room.y + (int)Math.Floor((decimal)(roomHeight / 2)); row++)
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
                if ((position.x >= roomCentre.x - (int)Math.Floor((decimal)(roomWidth / 2)) && position.x <= roomCentre.x + (int)Math.Floor((decimal)(roomWidth / 2))) && (position.y >= roomCentre.y - (int)Math.Floor((decimal)(roomHeight / 2)) && position.y <= roomCentre.y + (int)Math.Floor((decimal)(roomHeight / 2))))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    public List<Vector2Int> roomCoordToMapCoord(List<Vector2Int> roomCoord)
    {
        List<Vector2Int> mapCoords = new List<Vector2Int>();
        foreach (var coord in roomCoord)
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