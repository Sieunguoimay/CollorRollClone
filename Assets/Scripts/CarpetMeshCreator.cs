using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MeshRebuiltEvent: UnityEvent<Carpet> { }

public class Carpet
{
    public Vector2[] Polygon;
    public Vector2 TopMost;
    public Vector2 BottomMost;
}

public class CarpetMeshCreator : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public CarpetSO carpetSO;
    [SerializeField] bool shouldRebuild = false;

    public MeshRebuiltEvent MeshRebuiltCallback = null;


    void Start()
    {
        RebuildMesh();
    }

    public void RebuildMesh(bool shouldTriggerEvent = true)
    {

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        //Change the material color by changing the material??
        Material m = new Material(meshRenderer.sharedMaterial);
        m.color = carpetSO.Color;
        meshRenderer.sharedMaterial = m;

        ShapeGenerator shapeGenerator = new ShapeGenerator();
        shapeGenerator.GeneratePolygon(carpetSO.Polygon.ToArray(),//vertices2D,
            0.05f, 0.025f, out Vector3[] vertices, out int[] indices, out Vector2 bottomMost, out Vector2 topMost); ;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        Carpet carpet = new Carpet()
        {
            Polygon = carpetSO.Polygon.ToArray(),//vertices2D,
            TopMost = topMost,
            BottomMost = bottomMost
        };
        
        if(shouldTriggerEvent) MeshRebuiltCallback?.Invoke(carpet);
    }
}

[CustomEditor(typeof(CarpetMeshCreator))]
[CanEditMultipleObjects]
public class CarpetMeshCreatorCE : Editor
{
    CarpetMeshCreator carpetMeshCreator;
    private void OnEnable()
    {
        carpetMeshCreator = target as CarpetMeshCreator;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Carpet Scriptable Object");
        Editor.CreateEditor(carpetMeshCreator.carpetSO).OnInspectorGUI();
        if (GUILayout.Button("Rebuild Mesh"))
            carpetMeshCreator.RebuildMesh();
        EditorGUILayout.EndVertical();


        //EditorGUILayout.ObjectField(carpetMeshCreator.carpetSO, typeof(CarpetSO), true);
        //EditorGUILayout.PropertyField(vertices2D);
        //if (GUILayout.Button("Rebuild Mesh"))
        //    shouldRebuild.boolValue = true;
        //EditorGUILayout.PropertyField(MeshRebuiltCallback);
        //EditorGUILayout.EventField(carpetMeshCreator.MeshRebuiltCallback, typeof(MeshRebuiltEvent), true);
        //serializedObject.Update();
        //serializedObject.ApplyModifiedProperties();
    }


    private void OnSceneGUI()
    {
        int n = carpetMeshCreator.carpetSO.Polygon.Count;
        var polygon = carpetMeshCreator.carpetSO.Polygon;

        Handles.BeginGUI();
        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(200), GUILayout.Height(0));
        

        EditorGUILayout.LabelField("Carpet Vertex Editor");
        
        for (var i = 0; i < n; i++) polygon[i] = EditorGUILayout.Vector2Field("", polygon[i]);
        
        if (GUILayout.Button("Rebuild Mesh"))
            carpetMeshCreator.RebuildMesh();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rotation Y");
        Vector3 oldAngle = carpetMeshCreator.transform.localEulerAngles;
        float angle = EditorGUILayout.FloatField(oldAngle.y);
        carpetMeshCreator.transform.localEulerAngles = new Vector3(oldAngle.x, angle, oldAngle.z);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
        Handles.EndGUI();



        for (int i = 0; i<n; i++)
        {
            var v = carpetMeshCreator.carpetSO.Polygon[i];
            var v2 = carpetMeshCreator.carpetSO.Polygon[i==n-1?0:i+1];
            Vector3 p = carpetMeshCreator.transform.TransformPoint(new Vector3(v.x, 0, v.y));
            Vector3 p2 = carpetMeshCreator.transform.TransformPoint(new Vector3(v2.x, 0, v2.y));
            Vector3 handlePositionInWorldSpace = Handles.FreeMoveHandle(p, Quaternion.identity, 0.5f, Vector3.one, Handles.SphereHandleCap);
            var newPos = carpetMeshCreator.transform.InverseTransformPoint(handlePositionInWorldSpace);
            carpetMeshCreator.carpetSO.Polygon[i] = new Vector2(newPos.x, newPos.z);
            Handles.DrawLine(p, p2);
        }
    }
}



//GameObject plane = new GameObject("Triangulator");
//MeshRenderer _meshRenderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
//MeshFilter meshFilter = plane.AddComponent(typeof(MeshFilter)) as MeshFilter;
//shapeGenerator.GenerateTriangle(new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1.5f),
//    0.05f, out Vector2[] vertices2D2, out int[] indices2);

//Vector2[] vertices2D = new Vector2[(vertices2D1.Length + vertices2D2.Length)*2];
//vertices2D1.CopyTo(vertices2D, 0);
//vertices2D2.CopyTo(vertices2D, vertices2D1.Length);
//vertices2D1.CopyTo(vertices2D, vertices2D1.Length + vertices2D2.Length);
//vertices2D2.CopyTo(vertices2D, vertices2D1.Length*2 + vertices2D2.Length);

//for (int i = 0; i< indices2.Length; i++)
//    indices2[i] += vertices2D1.Length;

//int[] indices = new int[(indices1.Length + indices2.Length)*2];
//indices1.CopyTo(indices, 0);
//indices2.CopyTo(indices, indices1.Length);


//opposite face
//int startIIndex = indices1.Length + indices2.Length;
//int startVIndex = vertices2D1.Length + vertices2D2.Length;
//for (int i = 0; i< (indices1.Length + indices2.Length)/3; i++)
//{
//    indices[startIIndex + i*3] = startVIndex + indices[i*3];
//    indices[startIIndex + i*3+1] = startVIndex + indices[i*3+2];
//    indices[startIIndex + i*3+2] = startVIndex + indices[i*3+1];
//}



//Vector3[] vertices = new Vector3[vertices2D.Length];
//for (int i = 0; i < vertices.Length; i++)
//    if(i< vertices.Length/2)
//        vertices[i] = new Vector3(vertices2D[i].x, 0.01f, vertices2D[i].y);
//    else
//        vertices[i] = new Vector3(vertices2D[i].x, -0.01f, vertices2D[i].y);

//add border triangles


//Vector3[] vertices = new Vector3[]
//{
//    new Vector3(-1,0,-1),
//    new Vector3(-1,0,1),
//    new Vector3(1,0,1),

//    new Vector3(1,0,1),
//    new Vector3(0,-1,0),
//    new Vector3(-1,0,-1),
//};
//int[] indices = new int[]
//{
//    0,1,2,3,4,5
//};
