using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetMPBlock : MonoBehaviour
{
    public MaterialPropertyBlock Block { get; private set; }
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public Material sharedMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        Block = new MaterialPropertyBlock();
        //meshRenderer.material = sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMesh(meshFilter.mesh, transform.position, transform.rotation, sharedMaterial, 0, null, 0, Block);
    }
}
