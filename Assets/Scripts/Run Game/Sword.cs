using UnityEngine;

public class Sword : MapItem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            AddToInventory(this.gameObject, other.gameObject);
        }
        Destroy(this.gameObject);
    }
}