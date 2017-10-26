using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    private GameObject defField;
    private int width = 0;
    private int height = 0;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Map Generation");

        defField = EditorGUILayout.ObjectField("Default Field", defField, typeof(GameObject), false, null) as GameObject;
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        MapController controller = target as MapController;

        // map generation button click
        if (GUILayout.Button("Generate"))
        {
            if (width <= 0 || height <= 0)
                Debug.LogWarning("Map size has to be positive!");
            else
            {
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

                
                Texture2D tex = new Texture2D(100, 100);
                GameObject row;
                GameObject field;
                for (int y = 0; y < height; y++)
                {
                    row = new GameObject("Row" + y);
                    row.transform.SetParent(controller.transform);
                    row.transform.position = new Vector3(0, y, 0);
                    for (int x = 0; x < width; x++)
                    {
                        if (!defField)
                        {
                            field = new GameObject("Field" + x);
                            field.transform.SetParent(row.transform);
                            field.AddComponent<SpriteRenderer>();
                            field.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, 100, 100), new Vector2(.5f, .5f));
                        }
                        else
                        {
                            field = Instantiate(defField, Vector3.zero, Quaternion.Euler(Vector3.zero), row.transform);
                            field.name = "Field" + x;
                        }
                        
                        field.transform.localPosition = new Vector3(x, 0, 0);
                    }
                }
                Debug.Log("Map generated!");
            }
        }

        // space and usual map controller gui
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}
