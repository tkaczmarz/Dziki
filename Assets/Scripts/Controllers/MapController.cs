using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapController : MonoBehaviour
{
    public Material highlightMaterial;

    public static MapController Instance { get { return instance; } }
    private static MapController instance = null;

    /// <summary>Full list of fields in 2D map.</summary>
    private List<List<Field>> fields = new List<List<Field>>();
    private LineRenderer lineRenderer;
    /// <summary>Costs of moving through all types of terrains. Used to calculate path cost.</summary>
    private float[] terrainCosts = { 5, 3, 2, 1.5f, 1 };

    private void Awake() 
    {
        if (!instance)
        {
            instance = this;
        }
        else
            DestroyImmediate(gameObject);

        if (!highlightMaterial)
            Debug.LogWarning("No highlight material added!");

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
        if (lineRenderer)
        {
            lineRenderer.positionCount = 0;
        }
    }

    /// <summary>Method generates an off mesh link between two given fields.</summary>
    private void CreateOffMeshLink(Field from, Field to)
    {
        OffMeshLink link = from.gameObject.AddComponent<OffMeshLink>();
        link.biDirectional = false;
        link.startTransform = from.transform;
        link.endTransform = to.transform;
        link.area = NavMesh.GetAreaFromName(to.terrain.ToString());
    }

    /// <summary>Method returns field on given position.</summary>
    /// <param name="pos">3D position where 'y' axis is ignored and 'z' axis becomes 'y': (x, 0, y).</param>
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

    /// <summary>Method returns mouse position rounded to integer.</summary>
    public static Vector2Int GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePos.x);
        int y = Mathf.RoundToInt(mousePos.z);

        return new Vector2Int(x, y);
    }

    /// <summary>Method returns full 3D mouse position.</summary>
    public static Vector3 GetMousePos3()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePos.x);
        int y = Mathf.RoundToInt(mousePos.z);

        return new Vector3(x, 0, y);
    }

    /// <summary>Method checks if given point is actually on map.</summary>
    /// <returns>True if point is on map.</returns>
    public bool IsPointOnMap(int x, int y)
    {
        if (y >= 0 && y < fields.Count && x >= 0 && x < fields[0].Count)
            return true;
        else
            return false;
    }

    #region Path methods
    public float PathLength(NavMeshPath path)
    {
        if (path == null)
            return 0;
        if (path.corners.Length < 2)
            return 0;

        float length = 0;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    /// <summary>Method calculates cost of moving through all terrains on given path.</summary>
    public float PathCost(NavMeshPath path)
    {
        if (path == null)
            return 0;

        float cost = 0;
        List<Vector3> corners = new List<Vector3>(GetUniquePathCorners(path));
        corners.RemoveAt(0);    // ignore position where unit already stands
        foreach (Vector3 corner in corners)
        {
            Field f = GetFieldAt(corner);
            cost += terrainCosts[(int)f.terrain];
        }

        return cost;
    }

    /// <summary>
    /// Method checks if path corners are not too close to each other  
    /// and returns an array of unique path positions.
    /// </summary>
    public Vector3[] GetUniquePathCorners(NavMeshPath path)
    {
        if (path.corners.Length > 2)
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
            return corners.ToArray();
        }
        else
            return path.corners;
    }

    /// <summary>Method draws red line which includes all path corners.</summary>
    public void DrawPath(NavMeshPath path, Unit unit = null)
    {
        if (unit)
        {
            if (PathCost(path) > unit.movementRange)
            {
                lineRenderer.positionCount = 0;
                return;
            }
        }

        if (path == null)
        {
            lineRenderer.positionCount = 0;
        }
        else if (path.corners.Length > 1)
        {
            Vector3[] corners = GetUniquePathCorners(path);
            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);
        }
        else
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
    }
    #endregion
}
