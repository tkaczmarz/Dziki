﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour 
{
    public List<SelectableObject> Troops { get { return troops; } }

	public int nr = 0;
    public Color color;
    private List<SelectableObject> troops = new List<SelectableObject>();

    public void RefreshTroops()
    {
        foreach (SelectableObject o in troops)
        {
            o.Refresh();
        }
    }
}