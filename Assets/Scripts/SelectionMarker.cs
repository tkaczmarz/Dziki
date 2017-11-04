using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{ 
    public Unit selectedUnit = null;

    private void Start()
    {
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        MoveSelection();
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();   
        }
    }

    private void MoveSelection()
    {
        Vector2Int mousePos = MapController.GetMousePos();
        if (MapController.Instance.IsPointOnMap(mousePos.x, mousePos.y))
        {
            transform.position = MapController.GetMousePos3();
        }
    }

    private void SelectObject()
    {
        Field field = MapController.Instance.GetFieldAt(transform.position);
        if (field.Unit)
        {
            if (selectedUnit)
            {
                selectedUnit.Deselect();
                selectedUnit = null;
            }

            selectedUnit = field.Unit;
            selectedUnit.Select();
        }
    }
}
