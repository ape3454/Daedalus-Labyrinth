using UnityEngine;

public class CoinFragment : MapItem
{
    private CoinManager controller;

    public void MoveTo(Vector2Int position)
    {
        this.transform.position = new Vector3(position.x, position.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("This");
        if (other.transform.tag == "Player")
        {
            AddToInventory(transform.gameObject, other.gameObject);
        }
        Destroy(this.gameObject);
    }
}