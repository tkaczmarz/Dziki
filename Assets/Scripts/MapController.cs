using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get { return instance; } }
    private static MapController instance = null;

    private List<List<Field>> fields = new List<List<Field>>();

    private void Awake() 
    {
        if (!instance)
        {
            instance = this;
        }
        else
            DestroyImmediate(gameObject);

        // get map data
        foreach (Transform row in transform)
        {
            fields.Add(new List<Field>());
            foreach (Field field in row.GetComponentsInChildren<Field>())
            {
                fields[fields.Count - 1].Add(field);
            }
        }
    }

    public Field GetFieldAt(int x, int y)
    {
        if (y >= fields.Count || y < 0)
        {
            Debug.LogWarning("Wrong field coordinates!");
            return null;
        }
        else if (x >= fields[y].Count || x < 0)
        {
            Debug.LogWarning("Wrong field coordinates!");
            return null;
        }

        return fields[y][x];
    }
}
