using UnityEngine;

public class MapItem : MonoBehaviour
{
    private Vector2Int location;

    protected void AddToInventory(GameObject currentObject, GameObject other)
    {
        other.GetComponent<PlayerController>().AddToInventory(currentObject);
    }
}
