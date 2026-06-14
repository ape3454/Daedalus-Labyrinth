using UnityEngine;

public class MapItem : MonoBehaviour
{
    public string nameOfItem { get { return itemName; } }
    private string itemName;
    private Vector2Int location;

    protected void AddToInventory(GameObject currentObject, GameObject other)
    {
        other.GetComponent<PlayerController>().AddToInventory(currentObject);
    }
}
