using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameEditor : MonoBehaviour
{
    public LevelSO levelSO;
    public void CreateNewLevelSO()
    {
        levelSO = Utils.Instance.CreateAsset<LevelSO>("Assets/Level.asset");
    }
}
[CustomEditor(typeof(GameEditor))]
[CanEditMultipleObjects]
public class GameEditorCE: Editor
{
    GameEditor gameEditor { get { return (GameEditor)target; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //EditorGUI.indentLevel++;
        if(gameEditor.levelSO!=null)
            Editor.CreateEditor(gameEditor.levelSO).OnInspectorGUI();
        //EditorGUI.indentLevel--;

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("New Level Scriptable Object"))
            gameEditor.CreateNewLevelSO();
        EditorGUILayout.EndVertical();

    }

    private void OnSceneGUI()
    {
        if (gameEditor.levelSO == null) return;
        for(int i = 0; i<gameEditor.levelSO.carpetSOs.Count; i++)
        {
            var polygon = gameEditor.levelSO.carpetSOs[i].Polygon;
            int n = polygon.Count;

            Vector3[] polygon3D = new Vector3[n];
            for (int j = 0; j < n; j++)
            {
                var v = polygon[j];
                var v2 = polygon[j == n - 1 ? 0 : j + 1];
                Vector3 p = gameEditor.transform.TransformPoint(new Vector3(v.x, 0, v.y));
                Vector3 p2 = gameEditor.transform.TransformPoint(new Vector3(v2.x, 0, v2.y));
                Vector3 handlePositionInWorldSpace = Handles.FreeMoveHandle(p, Quaternion.identity, 0.5f, Vector3.one, Handles.SphereHandleCap);
                var newPos = gameEditor.transform.InverseTransformPoint(handlePositionInWorldSpace);
                gameEditor.levelSO.carpetSOs[i].Polygon[j] = new Vector2(newPos.x, newPos.z);
                Handles.DrawLine(p, p2);

                polygon3D[j] = p;
            }
            Handles.color = gameEditor.levelSO.carpetSOs[i].Color;
            Handles.DrawAAConvexPolygon(polygon3D);
            Handles.color = Color.white;
        }
        EditorUtility.SetDirty(target);

    }
}
