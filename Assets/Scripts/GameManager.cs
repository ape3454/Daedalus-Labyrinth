using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GridFirstDungeonGenerator dungeonGenerator;

    public PlayerController player;


    private void Awake()
    {
        dungeonGenerator = this.GetComponent<GridFirstDungeonGenerator>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonGenerator.GenerateDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
