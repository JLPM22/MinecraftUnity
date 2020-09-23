using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkRenderer))]
public class Chunk : MonoBehaviour
{
    public const int ChunkSize = 16;
    public const int VerticalChunks = 16;

    private Block[,,] BlockID = new Block[ChunkSize, ChunkSize, ChunkSize];

    private ChunkRenderer ChunkRenderer;

    private void Awake()
    {
        ChunkRenderer = GetComponent<ChunkRenderer>();
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        BlockID[x, y, z] = block;
        ChunkRenderer.RegenerateVoxel(x, y, z);
    }

    public Block GetBlock(int x, int y, int z)
    {
        return BlockID[x, y, z];
    }
}

public enum Block
{
    AIR = 0,
    DIRT = 1,
    GRASS = 2,
    COBBLESTONE = 3,
    STONE = 4,
    LOG = 5,
    WOOD = 6,
    LEAVES = 7,
    WATER = 8
}
