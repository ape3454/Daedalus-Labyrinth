using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> coinFragmentsPrefab;
    private List<GameObject> coinFragments;
    [SerializeField]
    private GameObject coinPrefab;


    public void CreateCoin()
    {
        for (int i = 0; i < coinFragmentsPrefab.Count; i++)
        {
            coinFragments.Add(Instantiate(coinFragmentsPrefab[i], transform.position + new Vector3(i - 1.5f, -0.5f, 0), transform.rotation, transform));
            coinFragments[i].AddComponent(typeof(MoveToPedestal));
        }

        if (coinFragments.All(y => y == null))
        {
            Instantiate(coinPrefab, transform.position + new Vector3(0, 0.5f, 0), transform.rotation);
        }

    }
}
