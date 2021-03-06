﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{ 
    public SelectableObject SelectedObject { get { return selectedObject; } }
    public SelectableObject PointedObject { get { return pointedObject; } }
    /// <summary>Property returns true if marker's position has changed in comparison to previous frame.</summary>
    public bool PositionChanged { get { return positionChanged; } }

    private SelectableObject selectedObject = null;
    private SelectableObject pointedObject = null;
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
            if (selectedObject == null)
                SelectObject();   
        }
        else if (Input.GetMouseButtonDown(1))
            DeselectObject();
    }

    /// <summary>Method moves marker to position (field) pointed by mouse.</summary>
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

                // set pointed object
                pointedObject = MapController.Instance.GetFieldAt(transform.position).Selectable;
            }
        }
    }

    private void SelectObject()
    {
        Field field = MapController.Instance.GetFieldAt(transform.position);
        if (field.Selectable)
        {
            if (selectedObject)
            {
                DeselectObject();
            }

            if (field.Selectable.Select())
                selectedObject = field.Selectable;
        }
    }

    public void DeselectObject()
    {
        if (selectedObject)
        {
            selectedObject = null;
        }
    }
}
