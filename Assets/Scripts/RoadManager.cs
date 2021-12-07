using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public List<GameObject> roadPrefab = new List<GameObject>();
    
    void Start()
    {
        int roadCount = transform.childCount;
        for(int i = 0; i < roadCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            roadPrefab.Add(transform.GetChild(i).gameObject);
        }
    }
}