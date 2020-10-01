using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static readonly Vector3Int ChunkSize = new Vector3Int(16, 16, 16);
    public static readonly int NumberVerticalChunks = 2;
    //public static readonly Vector3Int[] DirectionToIncrement = new Vector3Int[] { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down, new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1) };

    public Vector3Int Index;
    public Block[,,] Voxels = new Block[ChunkSize.x, ChunkSize.y, ChunkSize.z];

    private ChunkRenderer ChunkRenderer;

    public bool Delete;
    public bool Generated;
    public bool AsyncFinished;

    public void SetChunkRenderer(ChunkRenderer chunkRenderer)
    {
        ChunkRenderer = chunkRenderer;
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
    LEFT,
    TOP,
    BOTTOM,
    FORWARD,
    BACK
}