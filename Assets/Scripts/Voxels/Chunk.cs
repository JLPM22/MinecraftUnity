using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkRenderer))]
public class Chunk : MonoBehaviour
{
    public static readonly Vector3Int ChunkSize = new Vector3Int(16, 16, 16);
    public static readonly int VerticalChunks = 16;

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
        ChunkRenderer.RegenerateVoxel(x, y, z);
    }

    public Block GetBlock(int x, int y, int z)
    {
        return Voxels[x, y, z];
    }

    private void LateUpdate()
    {
        if (Delete)
        {
            Destroy(gameObject);
        }
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
