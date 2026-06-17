using UnityEngine;

public class EnemyAwarenessOfPlayer : MonoBehaviour
{
    public bool awareOfPlayer { get; private set; }

    [SerializeField]
    private float playerAwarenessRange;
    private Transform player;
    public Vector2 directionToPlayer { get; private set; }
    [HideInInspector]
    public Vector2 enemyToPlayerVector;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        enemyToPlayerVector = player.position - transform.position;
        directionToPlayer = enemyToPlayerVector.normalized;

        if (enemyToPlayerVector.magnitude <= playerAwarenessRange)
        {
            awareOfPlayer = true;
        }
        else
        {
            awareOfPlayer = false;
        }
    }
}