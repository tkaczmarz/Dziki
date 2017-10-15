using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;
    public Grid grid;

    private Transform selectionMarker;

    private void Start()
    {
        if (!grid)
            grid = FindObjectOfType<Grid>();

        if (!selectionMarkerPrefab)
        {
            Debug.LogError("Selection marker missing!");
            Debug.Break();
        }
        else
        {
            Vector3 cellPos = grid.CellToWorld(Vector3Int.zero);
            selectionMarker = Instantiate(selectionMarkerPrefab, cellPos, Quaternion.Euler(Vector3.zero)).transform;
        }
    }

    private void Update()
    {
        MoveSelection();
    }

    private void MoveSelection()
    {
        if (!selectionMarker)
            return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int intMousePos = new Vector3Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y), Mathf.FloorToInt(mousePos.z));
        Vector3 cellPos = grid.CellToWorld(intMousePos);
        selectionMarker.position = cellPos;
    }
}
