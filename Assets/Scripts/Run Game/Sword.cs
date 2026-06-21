using UnityEngine;

public class Sword : MapItem
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            AddToInventory(this.gameObject, other.gameObject);
        }
        Destroy(this.gameObject);
    }
}