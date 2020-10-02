using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MeshRebuiltEvent: UnityEvent<Carpet> { }

public class Carpet/*: ScriptableObject*/
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

    //public Vector2[] vertices2D;

    public MeshRebuiltEvent MeshRebuiltCallback = null;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Carpet Mesh creator");

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        rebuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRebuild)
        {
            shouldRebuild = false;
            rebuildMesh();
        }
    }

    private void rebuildMesh(bool shouldTriggerEvent = true)
    {

        meshRenderer.material.SetColor("_Color",carpetSO.Color);
        ShapeGenerator shapeGenerator = new ShapeGenerator();
        //shapeGenerator.GenerateTriangle(new Vector2(0,0),new Vector2(1,0), new Vector2(2,1),
        //    0.05f,0.025f, out Vector3[] vertices, out int[] indices);


        //float t = 0.5f;
        //Vector2[] vertices2D = new Vector2[] {
        //    new Vector2(-t,0),
        //    new Vector2(-t*2.0f,t*2.0f),
        //    new Vector2(-t,t*3.5f),
        //    new Vector2(0,t*3.0f),
        //    new Vector2(t,t*2.5f),
        //    new Vector2(t*1.5f,t*0.5f),
        //};
        shapeGenerator.GeneratePolygon(carpetSO.Polygon,//vertices2D,
            0.05f, 0.025f, out Vector3[] vertices, out int[] indices, out Vector2 bottomMost, out Vector2 topMost); ;


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

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        //GameObject plane = new GameObject("Triangulator");
        //MeshRenderer _meshRenderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //MeshFilter meshFilter = plane.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = mesh;
        Carpet carpet = new Carpet()
        {
            Polygon = carpetSO.Polygon,//vertices2D,
            TopMost = topMost,
            BottomMost = bottomMost
        };
        
        if(shouldTriggerEvent)
            MeshRebuiltCallback?.Invoke(carpet);
        Debug.Log("Mesh Bounds " + meshRenderer.bounds.size);
    }
}

[CustomEditor(typeof(CarpetMeshCreator))]
[CanEditMultipleObjects]
public class CarpetMeshCreatorCE : Editor
{
    //SerializedProperty vertices2D;
    //SerializedProperty shouldRebuild;
    //SerializedProperty MeshRebuiltCallback;
    //SerializedProperty carpetSO;

    CarpetMeshCreator carpetMeshCreator;

    private void OnEnable()
    {
        //vertices2D = serializedObject.FindProperty("vertices2D");
        //shouldRebuild = serializedObject.FindProperty("shouldRebuild");
        //MeshRebuiltCallback = serializedObject.FindProperty("MeshRebuiltCallback");
        //carpetSO = serializedObject.FindProperty("carpetSO");
        carpetMeshCreator = target as CarpetMeshCreator;
    }
    public override void OnInspectorGUI()
    {

        //serializedObject.Update();

        //base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Carpet Scriptable Object");
        EditorGUILayout.ObjectField(carpetMeshCreator.carpetSO,typeof(CarpetSO),true);
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.PropertyField(vertices2D);

        //if (GUILayout.Button("Rebuild Mesh"))
        //    shouldRebuild.boolValue = true;


        //EditorGUILayout.PropertyField(MeshRebuiltCallback);
        //EditorGUILayout.EventField(carpetMeshCreator.MeshRebuiltCallback, typeof(MeshRebuiltEvent), true);

        //serializedObject.ApplyModifiedProperties();


    }


    private void OnSceneGUI()
    {        

        Handles.BeginGUI();
        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(100), GUILayout.Height(100));
        EditorGUILayout.LabelField("Hello Custom Editor");
        if (GUILayout.Button("Rebuild Mesh"))
        {            

        }

        EditorGUILayout.EndVertical();
        Handles.EndGUI();
        int n = carpetMeshCreator.carpetSO.Polygon.Length;
        for (int i = 0; i<n; i++)
        {
            var v = carpetMeshCreator.carpetSO.Polygon[i];
            var v2 = carpetMeshCreator.carpetSO.Polygon[i==n-1?0:i+1];
            carpetMeshCreator.carpetSO.Polygon[i] = Handles.PositionHandle(new Vector3(v.x,0,v.y),Quaternion.identity);
            //carpetMeshCreator.carpetSO.Polygon[i] = Handles.FreeMoveHandle(v, Quaternion.identity, 0.5f, Vector3.one, Handles.SphereHandleCap);
            Handles.DrawLine(new Vector3(v.x,0,v.y), new Vector3(v2.x,0,v2.y));
        }
    }
}
