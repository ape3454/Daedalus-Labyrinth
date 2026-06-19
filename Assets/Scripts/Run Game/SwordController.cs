using UnityEngine;

public class SwordController : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField]
    private GameObject swordPrefab;
    private GameObject sword;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void GetStarted()
    {
        for (int i = 1; i < gameManager.gridHeight; i++)
        {
            Vector2Int position = gameManager.spawnCoord + Vector2Int.up * i;
            if (gameManager.roomsList.Contains(position))
            {
                sword = Instantiate(swordPrefab, new Vector3(gameManager.roomCoordToMapCoord(position).x + 0.5f, gameManager.roomCoordToMapCoord(position).y, 0), Quaternion.identity, this.transform);
                sword.name = swordPrefab.name;
                break;
            }
        }
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
