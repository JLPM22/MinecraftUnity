using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGeneration
{
    public static void GenerateChunk(Chunk chunk)
    {
        for (int z = 0; z < Chunk.ChunkSize.z; ++z)
        {
            for (int y = 0; y < Chunk.ChunkSize.y; ++y)
            {
                for (int x = 0; x < Chunk.ChunkSize.x; ++x)
                {
                    chunk.Voxels[x, y, z] = Block.DIRT;
                }
            }
        }
    }
}
