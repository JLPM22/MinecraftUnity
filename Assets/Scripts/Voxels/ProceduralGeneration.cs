using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class ProceduralGeneration
{
    private static float Scale
    {
        get
        {
            return ChunkManager.Instance.Scale;
        }
    }
    private static int Octaves
    {
        get
        {
            return ChunkManager.Instance.Octaves;
        }
    }
    private static float Persistance
    {
        get
        {
            return ChunkManager.Instance.Persistance;
        }
    }
    private static float Lacunarity
    {
        get
        {
            return ChunkManager.Instance.Lacunarity;
        }
    }
    private static int Seed
    {
        get
        {
            return ChunkManager.Instance.Seed;
        }
    }

    public static void AsyncGenerateChunk(Chunk chunk, Action callback)
    {
        new Thread(() =>
        {
            int[,] heightMap = new int[Chunk.ChunkSize.x, Chunk.ChunkSize.z];
            CreateHeightMap(chunk.Index.x, chunk.Index.z, heightMap);
            AssignHeightMap(chunk, heightMap);
            chunk.Generated = true;
            Thread.MemoryBarrier(); // Multithreding sync
            callback();
            chunk.AsyncFinished = true;
            Thread.MemoryBarrier(); // Multithreding sync
        }).Start();
    }

    private static void CreateHeightMap(int chunkX, int chunkZ, int[,] heightMap)
    {
        System.Random random = new System.Random(Seed);
        Vector2[] octaveOffsets = new Vector2[Octaves];

        float amplitude = 1.0f;
        float frequency = 1.0f;

        //Prepare Octaves Offsets
        for (int i = 0; i < Octaves; ++i)
        {
            float offsetX = random.Next(-100000, 100000) + chunkX * Chunk.ChunkSize.x;
            float offsetZ = random.Next(-100000, 100000) + chunkZ * Chunk.ChunkSize.z;
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        //Compute Height Map
        for (int z = 0; z < Chunk.ChunkSize.z; ++z)
        {
            for (int x = 0; x < Chunk.ChunkSize.x; ++x)
            {
                amplitude = 1.0f;
                frequency = 1.0f;
                float noiseHeight = 0.0f;

                //Each octave represents a type of noise (mountains, hills, rocks.. etc.)
                //since each octave has less importance than the previous one (because of persistance)
                //and each octave has more frequency than the previous one (because of persistance)
                for (int octave = 0; octave < Octaves; ++octave)
                {
                    float sampleX = ((x + octaveOffsets[octave].x) / Scale) * frequency;
                    float sampleZ = ((z + octaveOffsets[octave].y) / Scale) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2.0f - 1.0f; //-1.0f to 1.0f
                    noiseHeight += perlinValue * amplitude;

                    //Update next octave's amplitude and frequency
                    amplitude *= Persistance;
                    frequency *= Lacunarity;
                }

                heightMap[x, z] = Mathf.FloorToInt(((noiseHeight + 1.0f) / 2.0f) * Chunk.NumberVerticalChunks * Chunk.ChunkSize.y);
            }
        }
    }

    private static void AssignHeightMap(Chunk chunk, int[,] heightMap)
    {
        for (int z = 0; z < Chunk.ChunkSize.z; z++)
        {
            for (int y = 0; y < Chunk.ChunkSize.y; ++y)
            {
                for (int x = 0; x < Chunk.ChunkSize.x; x++)
                {
                    chunk.Voxels[x, y, z] = Block.AIR;
                }
            }
        }

        for (int z = 0; z < Chunk.ChunkSize.z; z++)
        {
            for (int x = 0; x < Chunk.ChunkSize.x; x++)
            {
                int startY = chunk.Index.y * Chunk.ChunkSize.y;
                for (int y = 0; y < Chunk.ChunkSize.y && (startY + y) < heightMap[x, z]; y++)
                {
                    if (startY + y == (heightMap[x, z] - 1))
                        chunk.Voxels[x, y, z] = Block.GRASS;
                    else
                        chunk.Voxels[x, y, z] = Block.DIRT;
                }
            }
        }
    }
}
