using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get { return instance; } }
    private static MapController instance = null;

    private List<List<Field>> fields = new List<List<Field>>();
    private LineRenderer lineRenderer;

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
                if ((y - 1) >= 0)
                    CreateOffMeshLink(fields[y][x], fields[y - 1][x]);
                if ((x + 1) < fields[y].Count)
                    CreateOffMeshLink(fields[y][x], fields[y][x + 1]);
                if ((x - 1) >= 0)
                    CreateOffMeshLink(fields[y][x], fields[y][x - 1]);
            }
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    private void CreateOffMeshLink(Field from, Field to)
    {
        OffMeshLink link = from.gameObject.AddComponent<OffMeshLink>();
        link.biDirectional = false;
        link.startTransform = from.transform;
        link.endTransform = to.transform;
        link.area = NavMesh.GetAreaFromName(to.terrain.ToString());
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

    public void DrawPath(NavMeshPath path)
    {
        if (path == null)
        {
            lineRenderer.positionCount = 0;
        }
        else if (path.corners.Length > 1)
        {
            List<Vector3> corners = new List<Vector3>();
            corners.Add(path.corners[0]);
            for (int i = 1; i < path.corners.Length; i++)
            {
                Vector3 a = path.corners[i - 1];
                Vector3 b = path.corners[i];
                if (Vector3.Distance(a, b) > 0.1f)
                    corners.Add(b);
            }

            lineRenderer.positionCount = corners.Count;
            lineRenderer.SetPositions(corners.ToArray());
        }
        else
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
    }
}
