using UnityEngine;

public class CoinFragment : MapItem
{
    private CoinManager controller;

    public void MoveTo(Vector2Int position)
    {
        this.transform.position = new Vector3(position.x, position.y, 0);
    }

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
        if (other.name == "Player")
        {

        }
    }
}
