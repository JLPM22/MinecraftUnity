using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    private Chunk Chunk;
    private MeshFilter MeshFilter;
    private MeshRenderer MeshRenderer;
    private static Material VoxelMaterial;
    public MeshCollider MeshCollider;

    // Reuse list to avoid garbage allocations
    private List<Vector3> Vertices = new List<Vector3>();
    private List<Vector3> Normals = new List<Vector3>();
    private List<int> Triangles = new List<int>();
    private List<Vector2> UVs = new List<Vector2>(); // Texture Coordinates
    private int NumberCubes = 0;

    // Async Variables
    private object RegenerateLock = new object();
    private bool Regenerate;
    private bool Regenerating;
    private bool RegenerationComplete;

    private void Awake()
    {
        if (VoxelMaterial == null)
        {
            VoxelMaterial = Resources.Load<Material>("VoxelsMat");
        }
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.sharedMesh = new Mesh();
        MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshRenderer.sharedMaterial = VoxelMaterial;
        Chunk = GetComponent<Chunk>();
    }

    private static int LastSlowOperationFrame;

    private void LateUpdate()
    {
        if (RegenerationComplete)
        {
            if (LastSlowOperationFrame == Time.frameCount) return;
            RegenerationComplete = false;
            // Assign Vertices & Triangles to the Mesh
            MeshFilter.sharedMesh.Clear();
            MeshFilter.sharedMesh.SetVertices(Vertices);
            MeshFilter.sharedMesh.SetNormals(Normals);
            MeshFilter.sharedMesh.SetTriangles(Triangles, 0);
            MeshFilter.sharedMesh.SetUVs(0, UVs);
            MeshFilter.sharedMesh.RecalculateBounds();
            if (MeshCollider != null) MeshCollider.sharedMesh = MeshFilter.sharedMesh;
            LastSlowOperationFrame = Time.frameCount;
        }

        // Regenerate
        if (Regenerate && !Regenerating)
        {
            Regenerate = false;
            Regenerating = true;
            AsyncChunkRenderer.Instance.TasksQueue.Enqueue(AsyncRegenerateMesh);
        }
    }

    public void RegenerateMesh()
    {
        Regenerate = true;
        Thread.MemoryBarrier();
    }

    private void AsyncRegenerateMesh()
    {
        // Clear structures
        Vertices.Clear();
        Normals.Clear();
        Triangles.Clear();
        UVs.Clear();
        NumberCubes = 0;

        // Create Vertices & Triangles
        for (int z = 0; z < Chunk.ChunkSize.z; ++z)
        {
            for (int y = 0; y < Chunk.ChunkSize.y; ++y)
            {
                for (int x = 0; x < Chunk.ChunkSize.x; ++x)
                {
                    BlockInfo block = BlockInfo.Blocks[(int)Chunk.GetBlock(x, y, z)];
                    // Test if is visible the voxel itself, and if it's surrounded by opaque voxels
                    if (block.Visible && IsOpaqueNotSurrounded(x, y, z)) DrawCube(x, y, z, block);
                }
            }
        }

        Regenerating = false;
        RegenerationComplete = true;
    }

    private void DrawCube(int x, int y, int z, BlockInfo block)
    {
        Vector3 offset = new Vector3(x, y, z);

        for (int i = 0; i < CubeMesh.Vertices.Length; ++i)
        {
            // Vertices
            Vertices.Add(CubeMesh.Vertices[i] + offset);
            // Normals
            Normals.Add(CubeMesh.Normals[i]);
            // UVs
            UVs.Add(block.TextureIDs[i >> 2] + CubeMesh.UVs[i]);
        }
        // Triangles
        for (int i = 0; i < CubeMesh.Triangles.Length; ++i)
        {
            Triangles.Add(CubeMesh.Triangles[i] + NumberCubes * CubeMesh.Vertices.Length);
        }

        NumberCubes++;
    }

    // Return true if a voxel is visible, false if surrounded by other opaque voxels
    // Return true if it's a boundary voxel except for vertical voxels which are tested as interior voxels
    private bool IsOpaqueNotSurrounded(int x, int y, int z)
    {
        // Test Boundary Voxel
        if (x + 1 >= Chunk.ChunkSize.x || x - 1 < 0 ||
            z + 1 >= Chunk.ChunkSize.z || z - 1 < 0)
        {
            return true;
        }

        // Test Vertical Voxels
        if (y + 1 >= Chunk.ChunkSize.y)
        {
            if (Chunk.Index.y == Chunk.NumberVerticalChunks - 1) return true;
            Chunk c = ChunkManager.Instance.GetChunk(Chunk.Index.x, Chunk.Index.y + 1, Chunk.Index.z);
            // This should be improved (the following two whiles)... but it's ok for now :D
            while (c == null)
            {
                Thread.Sleep(5);
                c = ChunkManager.Instance.GetChunk(Chunk.Index.x, Chunk.Index.y + 1, Chunk.Index.z);
            }
            if (c == null) return false;
            while (!c.Generated) Thread.Sleep(5); ;
            if (!c.IsOpaque(x, 0, z) || !Chunk.IsOpaque(x, y - 1, z)) return true;
        }
        else if (y - 1 < 0)
        {
            if (Chunk.Index.y == 0) return true;
            Chunk c = ChunkManager.Instance.GetChunk(Chunk.Index.x, Chunk.Index.y - 1, Chunk.Index.z);
            // This should be improved (the following two whiles)... but it's ok for now :D
            while (c == null)
            {
                Thread.Sleep(5);
                c = ChunkManager.Instance.GetChunk(Chunk.Index.x, Chunk.Index.y - 1, Chunk.Index.z);
            }
            if (c == null) return false;
            while (!c.Generated) Thread.Sleep(5);
            if (!c.IsOpaque(x, Chunk.ChunkSize.y - 1, z) || !Chunk.IsOpaque(x, y + 1, z)) return true;
        }
        else if (!Chunk.IsOpaque(x, y + 1, z) ||
                 !Chunk.IsOpaque(x, y - 1, z))
        {
            return true;
        }

        // Test Visible Voxel (X and Z)
        if (!Chunk.IsOpaque(x + 1, y, z) ||
            !Chunk.IsOpaque(x - 1, y, z) ||
            !Chunk.IsOpaque(x, y, z + 1) ||
            !Chunk.IsOpaque(x, y, z - 1))
        {
            return true;
        }
        // Voxel surrounded by opaque voxels
        return false;
    }
}