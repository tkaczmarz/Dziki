using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance = null;

    public GameObject selectionMarkerPrefab;

    [HideInInspector]
    public SelectionMarker marker;

    private GameController() { }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            DestroyImmediate(gameObject);

        if (selectionMarkerPrefab)
        {
            marker = Instantiate(selectionMarkerPrefab).GetComponent<SelectionMarker>();
        }
        else
            Debug.LogError("Selection marker prefab missing!");
    }
}
