using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="CarpetSO",menuName ="ScriptableObjects/CarpetSO", order =1)]
public class CarpetSO : ScriptableObject
{
    public Vector2[] Polygon;
    public Color Color;
    public Vector3 Position;
}

[CustomEditor(typeof(CarpetSO))]
public class CarpetSOCE : Editor
{
    //SerializedProperty Polygon;
    //SerializedProperty Color;
    CarpetSO carpetSO;

    private void OnEnable()
    {
        //Polygon = serializedObject.FindProperty("Polygon");
        //Color = serializedObject.FindProperty("Color");
        carpetSO = target as CarpetSO;
    }

    public override void OnInspectorGUI()
    {
        //serializedObject.Update();
        //EditorGUILayout.PropertyField(Polygon);
        //EditorGUILayout.PropertyField(Color);

        //EditorGUILayout.fie

        //DrawDefaultInspector();
        base.OnInspectorGUI();


        if (GUILayout.Button("Button"))
        {
            carpetSO.Color = new Color(0, 0, 1, 0);
        }

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

}