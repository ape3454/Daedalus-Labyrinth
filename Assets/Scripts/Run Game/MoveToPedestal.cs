using Unity.Cinemachine;
using UnityEngine;

public class MoveToPedestal : MonoBehaviour
{
    GameObject pedestal;
    [SerializeField]
    private float speed = 0.5f;
    Vector2 direction;

    private void Start()
    {
        pedestal = transform.parent.gameObject;
        direction = (transform.position - pedestal.transform.position + new Vector3(0, 0.5f, 0)).normalized;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        if (transform.position.y > pedestal.transform.position.y + 0.5)
        {
            Destroy(this.gameObject);
        }
    }
}