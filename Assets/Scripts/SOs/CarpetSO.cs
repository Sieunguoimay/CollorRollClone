using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="CarpetSO",menuName ="ScriptableObjects/CarpetSO", order =1)]
public class CarpetSO : ScriptableObject
{
    public List<Vector2> Polygon = new List<Vector2>();
    public Color Color;
    //public Vector3 Position;
}

[CustomEditor(typeof(CarpetSO))]
public class CarpetSOCE : Editor
{
    CarpetSO carpetSO;

    private void OnEnable()
    {
        carpetSO = target as CarpetSO;
        //carpetSO.Polygon = new Vector2[0];
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Carpet");
        carpetSO.Color = EditorGUILayout.ColorField(carpetSO.Color);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Vertices "+carpetSO.Polygon.Count);
        for (int i = 0; i < carpetSO.Polygon.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            carpetSO.Polygon[i] = EditorGUILayout.Vector2Field("", carpetSO.Polygon[i]);
            if (GUILayout.Button("Del"))
                carpetSO.Polygon.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool status = GUILayout.Button("New Vertex");
        EditorGUILayout.EndHorizontal();

        if (status)
        {
            carpetSO.Polygon.Add(new Vector2(0, 0));
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        //DrawDefaultInspector();
        //EditorUtility.SetDirty(target);
        //serializedObject.ApplyModifiedProperties();
    }

}