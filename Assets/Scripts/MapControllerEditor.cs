using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    /// <summary>
    /// Field prefab used for generation (can be null)
    /// </summary>
    private GameObject defField;
    private int width = 1;
    private int height = 1;
    private MapController controller;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Map Generation", EditorStyles.boldLabel);
        
        defField = EditorGUILayout.ObjectField("Default Field", defField, typeof(GameObject), false) as GameObject;
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        controller = target as MapController;
        
        // map generation button click
        if (GUILayout.Button("Generate"))
        {
            // validate
            if (width <= 0 || height <= 0)
                Debug.LogError("Map size has to be positive!");
            else
            {
                GenerateMap(width, height);
            }
        }

        // row buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Row"))
        {
            RemoveRow();
        }

        if (GUILayout.Button("Add Row"))
        {
            AddRow();
        }
        GUILayout.EndHorizontal();

        // column buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Column"))
        {
            RemoveColumn();
        }

        if (GUILayout.Button("Add Column"))
        {
            AddColumn();
        }
        GUILayout.EndHorizontal();

        // space and usual map controller gui
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Controller", EditorStyles.boldLabel);
        base.OnInspectorGUI();
    }

    private GameObject NewRow(string name, Transform parent, Vector3 pos)
    {
        GameObject row = new GameObject(name);
        row.transform.SetParent(parent);
        row.transform.position = pos;
        return row;
    }

    private GameObject NewField(string name, Transform parent, Vector3 pos)
    {
        GameObject field;

        if (!defField)
        {
            // if default field is null then generate blank
            field = new GameObject(name);
            field.transform.SetParent(parent);
            Texture2D tex = new Texture2D(100, 100);
            field.AddComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, 100, 100), new Vector2(.5f, .5f));
        }
        else
        {
            // instantiate field prefab if not null
            field = Instantiate(defField, Vector3.zero, Quaternion.Euler(Vector3.zero), parent);
            field.name = name;
        }
        field.transform.localPosition = pos;

        return field;
    }

    private void GenerateMap(int width, int height)
    {
        // check if map exists
        if (controller.transform.childCount > 0)
        {
            if (!EditorUtility.DisplayDialog("Overwrite?", "Map is not empty. Overwrite existing?", "Yes", "No"))
                return;
            else
                for (int i = controller.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(controller.transform.GetChild(i).gameObject);
                }
        }

        // generate new map
        GameObject row;
        for (int y = 0; y < height; y++)
        {
            row = NewRow("Row" + y, controller.transform, new Vector3(0, y, 0));
            for (int x = 0; x < width; x++)
            {
                NewField("Field" + x, row.transform, new Vector3(x, 0, 0));
            }
        }
    }

    /// <summary>
    /// Adds new row on top.
    /// </summary>
    private void AddRow()
    {
        int rows = controller.transform.childCount;
        if (rows == 0)
        {
            GenerateMap(1, 1);
            return;
        }

        Transform lastRow = controller.transform.GetChild(rows - 1);
        int fields = lastRow.childCount;

        GameObject newRow = NewRow("Row" + rows, controller.transform, new Vector3(0, lastRow.position.y + 1, 0));
        for (int i = 0; i < fields; i++)
        {
            NewField("Field" + i, newRow.transform, new Vector3(i, 0, 0));
        }
    }

    /// <summary>
    /// Removes one row from top.
    /// </summary>
    private void RemoveRow()
    {
        if (controller.transform.childCount > 0)
            DestroyImmediate(controller.transform.GetChild(controller.transform.childCount - 1).gameObject);
    }

    /// <summary>
    /// Adds one field to each row.
    /// </summary>
    private void AddColumn()
    {
        if (controller.transform.childCount == 0)
        {
            GenerateMap(1, 1);
            return;
        }

        foreach (Transform child in controller.transform)
        {
            Transform lastField = child.GetChild(child.childCount - 1);
            NewField("Field" + child.childCount, child, new Vector3(lastField.position.x + 1, 0, 0));
        }
    }

    /// <summary>
    /// Removes one field from each row.
    /// </summary>
    private void RemoveColumn()
    {
        foreach (Transform child in controller.transform)
        {
            if (child.childCount <= 1)
            {
                RemoveRow();
                return;
            }
            DestroyImmediate(child.GetChild(child.childCount - 1).gameObject);
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/2D Object/Map")]
    public static void CreateMap()
    {
        GameObject map = new GameObject("Map");
        map.AddComponent<MapController>();
    }
#endif
}
