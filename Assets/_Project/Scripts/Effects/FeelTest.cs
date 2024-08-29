using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeelTest : MonoBehaviour
{
    public GameObject testPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GameObject newGameObject = Instantiate(testPrefab);
    }
}
