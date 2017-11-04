using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        
        // generate off-mesh links
        for (int y = 0; y < fields.Count; y++)
        {
            for (int x = 0; x < fields[y].Count; x++)
            {
                if ((y + 1) < fields.Count)
                    CreateOffMeshLink(fields[y][x], fields[y + 1][x]);

                if ((x + 1) < fields[y].Count)
                    CreateOffMeshLink(fields[y][x], fields[y][x + 1]);
            }
        }
    }

    private void CreateOffMeshLink(Field from, Field to)
    {
        OffMeshLink link = from.gameObject.AddComponent<OffMeshLink>();
        link.startTransform = from.transform;
        link.endTransform = to.transform;
        link.area = NavMesh.GetAreaFromName(to.terrain.ToString());
    }

    public Field GetFieldAt(float x, float y)
    {
        int w = Mathf.RoundToInt(x);
        int h = Mathf.RoundToInt(y);
        if (h >= fields.Count || h < 0)
        {
            Debug.LogWarning("Wrong field coordinates!");
            return null;
        }
        else if (w >= fields[h].Count || w < 0)
        {
            Debug.LogWarning("Wrong field coordinates!");
            return null;
        }

        return fields[h][w];
    }

    public Field GetFieldAt(Vector3 pos)
    {
        int w = Mathf.RoundToInt(pos.x);
        int h = Mathf.RoundToInt(pos.z);
        if (h >= fields.Count || h < 0)
        {
            Debug.LogWarning("Wrong field coordinates!");
            return null;
        }
        else if (w >= fields[h].Count || w < 0)
        {
            Debug.LogWarning("Wrong field coordinates!");
            return null;
        }

        return fields[h][w];
    }

    public static Vector2Int GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePos.x);
        int y = Mathf.RoundToInt(mousePos.z);

        return new Vector2Int(x, y);
    }

    public static Vector3 GetMousePos3()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePos.x);
        int y = Mathf.RoundToInt(mousePos.z);

        return new Vector3(x, 0, y);
    }

    public bool IsPointOnMap(int x, int y)
    {
        if (y >= 0 && y < fields.Count && x >= 0 && x < fields[0].Count)
            return true;
        else
            return false;
    }
}
