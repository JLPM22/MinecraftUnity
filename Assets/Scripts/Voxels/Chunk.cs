using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static readonly Vector3Int ChunkSize = new Vector3Int(16, 16, 16);
    public static readonly int NumberVerticalChunks = 8;

    public Vector3Int Index;
    public Block[,,] Voxels = new Block[ChunkSize.x, ChunkSize.y, ChunkSize.z];

    private ChunkRenderer ChunkRenderer;

    public bool Delete;

    private void Awake()
    {
        ChunkRenderer = GetComponent<ChunkRenderer>();
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        Voxels[x, y, z] = block;
        ChunkRenderer.RegenerateMesh();
    }

    public Block GetBlock(int x, int y, int z)
    {
        return Voxels[x, y, z];
    }

    public bool IsVisible(int x, int y, int z)
    {
        return BlockInfo.Blocks[(int)GetBlock(x, y, z)].Visible;
    }

    public bool IsOpaque(int x, int y, int z)
    {
        return BlockInfo.Blocks[(int)GetBlock(x, y, z)].Opaque;
    }

    private void LateUpdate()
    {
        if (Delete)
        {
            Destroy(gameObject);
        }
    }
}

public enum Direction
{
    RIGHT,
    LET,
    TOP,
    BOTTOM,
    FORWARD,
    BACK
}