using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour 
{
    /// <summary>All units and structures that belong to this team.</summary>
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

    public bool HasUnits()
    {
        foreach (SelectableObject obj in troops)
        {
            if (obj is Unit)
                return true;
        }
        return false;
    }
}
