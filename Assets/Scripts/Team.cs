using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour 
{
    /// <summary>All units and structures that belong to this team.</summary>
    public List<SelectableObject> Troops { get { return troops; } }

	public int nr = 0;
    public Color color;
    public string leader;
    public bool isAdmin = false;
    private List<SelectableObject> troops = new List<SelectableObject>();

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        GameObject team = GameObject.Find("Team" + nr);
        if (team)
        {
            List<GameObject> troops = new List<GameObject>();
            foreach (Transform child in team.transform)
            {
                troops.Add(child.gameObject);
            }
            foreach (GameObject unit in troops)
            {
                unit.SetActive(true);
                SelectableObject s = unit.GetComponent<SelectableObject>();
                if (s)
                {
                    s.AssignToTeam(this);
                    Troops.Add(s);
                }
            }
        }
        else
            Debug.Log("Team not found!");
    }

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
