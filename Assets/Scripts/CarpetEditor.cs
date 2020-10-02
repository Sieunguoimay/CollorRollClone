using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetEditor : MonoBehaviour
{
    [SerializeField] PolygonVertex polygonVertexPrefab;

    public PolygonVertex[] PolygonVertices;

    private GameObject polygonVerticesContainer;

    private CarpetMeshCreator carpetCreator = null;

    // Start is called before the first frame update
    void Start()
    {
        polygonVerticesContainer = new GameObject("PolygonVerticesContainer");
        polygonVerticesContainer.transform.parent = transform;

        carpetCreator = GetComponent<CarpetMeshCreator>();
    }

    private void setupVertices()
    {
        //clear the container
        if (polygonVerticesContainer.transform.childCount > 0)
            foreach (Transform child in polygonVerticesContainer.transform)
                Destroy(child.gameObject);

        PolygonVertices = new PolygonVertex[carpetCreator.carpetSO.Polygon.Length];
        for (int i = 0; i < carpetCreator.carpetSO.Polygon.Length; i++)
        {
            Vector3 position = transform.TransformPoint(new Vector3(carpetCreator.carpetSO.Polygon[i].x, 0, carpetCreator.carpetSO.Polygon[i].y));
            PolygonVertices[i] = Instantiate(polygonVertexPrefab, position, Quaternion.identity, polygonVerticesContainer.transform);
            PolygonVertices[i].PositionChangedEvent += OnPolygonVertexChanged;
            PolygonVertices[i].Init(i);
        }
    }

    public void OnPolygonVertexChanged(PolygonVertex polygonVertex)
    {
        Vector3 localVertexPosition = transform.InverseTransformPoint(polygonVertex.transform.position);
        carpetCreator.carpetSO.Polygon[polygonVertex.Index] = new Vector2(localVertexPosition.x, localVertexPosition.z);
    }

    public void OnCarpetMeshRebuilt(Carpet carpet)
    {
        if (PolygonVertices.Length != carpet.Polygon.Length)
            setupVertices();
    }
}
