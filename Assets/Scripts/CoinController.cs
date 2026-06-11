using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinController : MonoBehaviour
{
    GameManager gameManager;
    List<Vector2Int> coinCoords;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void GetStarted()
    {
        coinCoords = GetCoinCoords();
        Debug.Log(string.Join(" ", coinCoords));
    }

    private List<Vector2Int> GetCoinCoords()
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        List<Vector2Int> possibleCoords = new List<Vector2Int>();

        HashSet<Vector2Int> deadEnds = gameManager.FindIntersectionsByConnections(1).Except(new HashSet<Vector2Int>() { gameManager.spawnCoord, gameManager.bossCoord }).ToHashSet();

        List<Vector2Int[]> tooClose = new List<Vector2Int[]>();
        List<Vector2Int[]> temporaryTooClose;
        List<Vector2Int> temporaryPossibleCoords = new List<Vector2Int>();
        var (temp1, temp2) = CheckPositions(deadEnds);
        possibleCoords.AddRange(temp1);
        tooClose.AddRange(temp2);

        int connectingMin = 2;
        int connectingMin2 = 1;

        int iteration = 0;
        do
        {
            iteration++;
            if (iteration == 1000) Debug.Log("What?");

            if (possibleCoords.Count <= 4)
            {
                if (connectingMin > 4) gameManager.RestartScene();

                HashSet<Vector2Int> intersectionRooms = gameManager.FindIntersectionsByConnections(connectingMin);
                (temp1, temp2) = CheckPositions(intersectionRooms);
                possibleCoords.AddRange(temp1);
                tooClose.AddRange(temp2);
                connectingMin2 = 1;
                connectingMin++;
            }
            temporaryPossibleCoords = possibleCoords;
            temporaryTooClose = new List<Vector2Int[]>();

            List<Vector2Int> tooCloseCoords = tooClose.SelectMany(y => y).Distinct().ToList();
            foreach (Vector2Int position in tooCloseCoords)
            {
                if (temporaryTooClose.Any(y => y.Contains(position))) continue;
                List<Vector2Int> connections = tooClose.FindAll(y => y.Contains(position)).SelectMany(y => y).ToList().FindAll(y => y != position);
                if (connections.Count == connectingMin2) temporaryPossibleCoords.Add(position);
                temporaryTooClose.AddRange(tooClose.FindAll(y => y.Contains(position)));
            }
            connectingMin2++;

            if (temporaryPossibleCoords.Count > 4)
            {
                if (possibleCoords.Count < 4) possibleCoords.Union(temporaryPossibleCoords);
                List<List<Vector2Int>> permutations = GetPermutations(possibleCoords, 4);

                List<(List<Vector2Int>, int)> highestScoredPerms = new List<(List<Vector2Int>, int)>() { (new List<Vector2Int>(), 0) };
                foreach (List<Vector2Int> perm in permutations)
                {
                    int score = 0;
                    foreach (Vector2Int pos in perm)
                    {
                        foreach (Vector2Int position in perm)
                        {
                            score += gameManager.GetShortestPath(pos, position).Count;
                        }
                    }
                    if (score > highestScoredPerms[0].Item2) highestScoredPerms = new List<(List<Vector2Int>, int)>() { (perm, score) };
                }

                coords = highestScoredPerms[Random.Range(0, highestScoredPerms.Count - 1)].Item1;
            }
        }
        while (coords.Count != 4 && iteration < 1000);
        return coords;
    }

    private List<List<Vector2Int>> GetPermutations(List<Vector2Int> list, int numberOfItems)
    {
        List<List<Vector2Int>> perms = new List<List<Vector2Int>>();
        if (list.Count != 0)
        {
            foreach (Vector2Int item in list)
            {
                perms.AddRange(GetPermutations(list.Except(new List<Vector2Int>() { item }).ToList(), numberOfItems));
            }
        }
        return perms;
    }

    private (List<Vector2Int>, List<Vector2Int[]>) CheckPositions(HashSet<Vector2Int> deadEnds)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        foreach (Vector2Int position in deadEnds)
        {
            int toSpawn = gameManager.GetShortestPath(position, gameManager.spawnCoord).Count;
            int toBoss = gameManager.GetShortestPath(position, gameManager.bossCoord).Count;
            if (toSpawn < 7 | toBoss < 4) coords.Add(position);
        }

        List<Vector2Int[]> tooClose = new List<Vector2Int[]>();
        foreach (Vector2Int position in coords)
        {
            foreach (Vector2Int other in coords)
            {
                int shortestPath = gameManager.GetShortestPath(position, other).Count;
                if (shortestPath > 4 && shortestPath != 1) tooClose.Add(new Vector2Int[] { position, other });
            }
        }

        return (coords, tooClose);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
