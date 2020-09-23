using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter MeshFilter;
    private MeshRenderer MeshRenderer;

    private void Awake()
    {
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.sharedMesh = new Mesh();
        MeshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    public void GenerateMesh()
    {

    }

    public void RegenerateVoxel(int x, int y, int z)
    {

    }
}
