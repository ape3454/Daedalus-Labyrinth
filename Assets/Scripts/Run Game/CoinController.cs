using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinManager : MonoBehaviour
{
    GameManager gameManager;
    List<Vector2Int> coinCoords;
    [SerializeField]
    private List<GameObject> coinFragments;
    private List<GameObject> fragments = new List<GameObject>();

    [SerializeField]
    private GameObject minorPedestal;
    private List<GameObject> pedestals = new List<GameObject>();
    

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void GetStarted()
    {
        coinCoords = GetCoinCoords();
        Debug.Log(string.Join(" ", coinCoords));
        for (int i = 0; i < 4; i++)
        {
            Vector2Int position = gameManager.roomCoordToMapCoord(coinCoords[i]);
            pedestals.Add(Instantiate(minorPedestal, new Vector3(position.x + 0.5f, position.y - 0.5f, 0), Quaternion.identity, this.transform));
            pedestals[i].name = minorPedestal.name;
            fragments.Add(Instantiate(coinFragments[i], new Vector3(position.x + 0.5f, position.y, -1), Quaternion.identity, this.transform));
            fragments[i].name = coinFragments[i].name;
        }
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
            if (iteration == 100) Debug.Log("What?");

            if (possibleCoords.Count < 4)
            {
                if (connectingMin > 4)
                {
                    Debug.Log("how");
                    //gameManager.RestartScene();
                    break;
                }

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
                List<Vector2Int> connections = tooClose.FindAll(y => y.Contains(position)).SelectMany(y => y).ToList().FindAll(y => y != position).Distinct().ToList();
                if (connections.Count <= connectingMin2) temporaryPossibleCoords.Add(position);
                temporaryTooClose.AddRange(tooClose.FindAll(y => y.Contains(position)));
            }
            connectingMin2++;
            
            if (temporaryPossibleCoords.Count >= 4)
            {
                if (possibleCoords.Count < 4) possibleCoords.Union(temporaryPossibleCoords);
                List<List<Vector2Int>> combinations = GetCombinations(possibleCoords);
                
                List<(List<Vector2Int>, int)> highestScoredPerms = new List<(List<Vector2Int>, int)>() { (new List<Vector2Int>(), 0) };
                Dictionary<HashSet<Vector2Int>, int> lengths = new Dictionary<HashSet<Vector2Int>, int>();
                foreach (List<Vector2Int> perm in combinations)
                {
                    int score = 0;
                    foreach (Vector2Int pos in perm)
                    {
                        List<int> distances = new List<int>();
                        foreach (Vector2Int position in perm.Except(new List<Vector2Int>() { pos }))
                        {
                            if (!lengths.Keys.Any(y => y.SetEquals(new HashSet<Vector2Int>() { pos, position })))
                            {
                                lengths.Add(new HashSet<Vector2Int>() { pos, position }, (int)Mathf.Pow(gameManager.GetShortestPath(pos, position).Count, 2));
                            }
                            distances.Add(lengths.FirstOrDefault(y => y.Key.SetEquals(new HashSet<Vector2Int>() { pos, position })).Value);
                        }
                        score += (int)Mathf.Pow((pos - new Vector2Int(0, 0)).magnitude, 2);
                        score += (int)Mathf.Pow((pos - new Vector2Int(0, gameManager.gridHeight)).magnitude, 2);
                        score += (int)Mathf.Pow((pos - new Vector2Int(gameManager.gridWidth, gameManager.gridHeight)).magnitude, 2);
                        score += (int)Mathf.Pow((pos - new Vector2Int(gameManager.gridWidth, 0)).magnitude, 2);
                        score += (int)(distances.Sum() / GetStd(distances));
                    }

                    if (score > highestScoredPerms[0].Item2) highestScoredPerms = new List<(List<Vector2Int>, int)>() { (perm, score) };
                }
                coords = highestScoredPerms[Random.Range(0, highestScoredPerms.Count - 1)].Item1;
                break;
            }
        }
        while (coords.Count != 4 && iteration < 100);
        return coords;
    }

    private double GetStd(List<int> values)
    {
        double avg = values.Average();
        double sum = values.Sum(y => Math.Pow(y - avg, 2));
        return Math.Sqrt((sum) / (values.Count() - 1));
    }

    private List<List<Vector2Int>> GetCombinations(List<Vector2Int> list)
    {
        List<List<Vector2Int>> combs = new List<List<Vector2Int>>();
        int start = 0;
        foreach (Vector2Int item in list.Skip(start))
        {
            int start1 = 0;
            foreach (Vector2Int item1 in list.Except(new List<Vector2Int>() { item }).Skip(start1))
            {
                int start2 = 0;
                foreach (Vector2Int item2 in list.Except(new List<Vector2Int>() { item, item1 }).Skip(start2))
                {
                    int start3 = 0;
                    foreach (Vector2Int item3 in list.Except(new List<Vector2Int>() { item, item1, item2 }).Skip(start3))
                    {
                        List<Vector2Int> newComb = new List<Vector2Int>() { item, item1, item2, item3 }.OrderBy(y => y.x).ThenBy(y => y.y).ToList();
                        if (!combs.Any(y => y.SequenceEqual(newComb))) combs.Add(newComb);
                    }
                }
            }
        }
        return combs;
    }

    private (List<Vector2Int>, List<Vector2Int[]>) CheckPositions(HashSet<Vector2Int> positions)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        List<Vector2Int[]> tooClose = new List<Vector2Int[]>();
        List<Vector2Int> deleteFromCoords = new List<Vector2Int>();
        
        foreach (Vector2Int position in positions)
        {
            int toSpawn = gameManager.GetShortestPath(position, gameManager.spawnCoord).Count;
            int toBoss = gameManager.GetShortestPath(position, gameManager.bossCoord).Count;
            if (toSpawn > 5 && toBoss > 4) coords.Add(position);
        }

        foreach (Vector2Int position in coords)
        {
            foreach (Vector2Int other in coords.Except(new List<Vector2Int>() { position }))
            {
                int shortestPath = gameManager.GetShortestPath(position, other).Count;
                if (shortestPath < 4)
                {
                    deleteFromCoords.Add(position);
                    deleteFromCoords.Add(other);
                    tooClose.Add(new Vector2Int[] { position, other });
                }
            }
        }
        coords.RemoveAll(y => deleteFromCoords.Contains(y));

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
