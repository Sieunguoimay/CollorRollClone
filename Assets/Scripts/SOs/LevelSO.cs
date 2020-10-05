using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelSO : ScriptableObject
{
    public List<CarpetSO> carpetSOs = new List<CarpetSO>();
}

[CustomEditor(typeof(LevelSO))]
[CanEditMultipleObjects]
public class LevelSOCE : Editor
{
    LevelSO levelSO { get { return (LevelSO)target; } }

    private void OnEnable()
    {
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        for (int i = 0; i < levelSO.carpetSOs.Count; i++)
        {
            Editor.CreateEditor(levelSO.carpetSOs[i]).OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool status = GUILayout.Button("Remove");
            EditorGUILayout.EndHorizontal();

            if (status)
                levelSO.carpetSOs.RemoveAt(i);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New Carpet"))
        {
            levelSO.carpetSOs.Add(ScriptableObject.CreateInstance<CarpetSO>() as CarpetSO);
        }
        EditorGUILayout.EndHorizontal();

    }
}