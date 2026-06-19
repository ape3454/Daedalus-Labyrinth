using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> coinFragmentsPrefab;
    private List<GameObject> coinFragments = new List<GameObject>();
    [SerializeField]
    private GameObject coinPrefab;
    private GameObject coinClone;


    public IEnumerator CreateCoin()
    {
        for (int i = 0; i < coinFragmentsPrefab.Count; i++)
        {
            GameObject gameObject = Instantiate(coinFragmentsPrefab[i], transform.position + new Vector3(i - 1.5f, -0.5f, 0), transform.rotation, transform);
            coinFragments.Add(gameObject);
            coinFragments[i].AddComponent(typeof(MoveToPedestal));
            UIHandler.instance.ElementSetVisible(coinFragmentsPrefab[i].name, false);
        }

        yield return new WaitUntil(() => coinFragments.All(y => y == null));
        coinClone = Instantiate(coinPrefab, transform.position + new Vector3(0, 0.75f, 0), transform.rotation);
        coinClone.name = "coin_Full";
    }
}
