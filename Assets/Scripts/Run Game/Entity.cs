using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected EnemyAwarenessOfPlayer playerDetection;

    protected Vector2Int spawnPosition;

    protected Animator animator;
    protected Vector2 moveDirection;

    public int maxHealth;
    protected int currentHealth;
    public int health { get { return currentHealth; } }

    public float speed;

    public void MoveTo(Vector2 destination)
    {
        transform.position = new Vector3(destination.x, destination.y, 0);
    }

    public virtual void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    protected abstract void RunReset();
}