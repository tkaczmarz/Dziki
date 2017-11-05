using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{ 
    public Unit SelectedUnit { get { return selectedUnit; } }
    public bool PositionChanged { get { return positionChanged; } }

    private Unit selectedUnit = null;
    private bool positionChanged = false;

    private void Start()
    {
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        positionChanged = false;
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
            Vector3 newPos = MapController.GetMousePos3();
            if (Vector3.Distance(transform.position, newPos) > 0.1f)
            {
                transform.position = newPos;
                positionChanged = true;
            }
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
