using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{ 
    private void Start()
    {
        transform.position = GameController.Instance.grid.CellToWorld(Vector3Int.zero);
    }

    private void Update()
    {
        MoveSelection();
    }

    private void MoveSelection()
    {
        Vector3 lastPos = transform.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int intMousePos = new Vector3Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y), Mathf.FloorToInt(mousePos.z));
        Vector3 cellPos = GameController.Instance.grid.CellToWorld(intMousePos);
        transform.position = cellPos;
    }
}
