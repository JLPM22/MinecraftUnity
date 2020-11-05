using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            GenerateTrees(chunk, heightMap);
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
                    else if (startY + y >= (heightMap[x, z] - 3))
                        chunk.Voxels[x, y, z] = Block.DIRT;
                    else
                        chunk.Voxels[x, y, z] = Block.STONE;
                }
            }
        }
    }

    private static void GenerateTrees(Chunk chunk, int[,] heightMap)
    {
        System.Random random = new System.Random(Seed + chunk.Index.x + chunk.Index.z);

        const int numberTreesPerChunk = 3;
        int numberTrees = random.Next(0, numberTreesPerChunk);

        for (int i = 0; i < numberTrees; ++i)
        {
            int voxelX = random.Next(2, Chunk.ChunkSize.x - 3); // To avoid having to manage tree generation at the border of a chunk...
            int voxelZ = random.Next(2, Chunk.ChunkSize.z - 3); // to be changed in the future

            int height = heightMap[voxelX, voxelZ];

            int chunkHeight = chunk.Index.y * Chunk.ChunkSize.y;
            if (chunkHeight + Chunk.ChunkSize.y > height &&
                chunkHeight <= height)
            {
                int voxelY = height - chunkHeight;
                GenerateTree(chunk, new Vector3Int(voxelX, voxelY, voxelZ), random);
            }
        }
    }

    private static void GenerateTree(Chunk chunk, Vector3Int voxelPos, System.Random random)
    {
        int treeHeight = random.Next(4, 9);
        int nextY = voxelPos.y;
        for (int i = 0; i < treeHeight; ++i)
        {
            if (nextY >= Chunk.ChunkSize.y)
            {
                if (Chunk.ChunkSize.y + 1 == Chunk.NumberVerticalChunks) return;
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x, chunk.Index.y + 1, chunk.Index.z);
                // This should be improved (the following two whiles)... but it's ok for now :D
                Stopwatch sw = Stopwatch.StartNew();
                while (c == null && sw.ElapsedMilliseconds < 1000)
                {
                    Thread.Sleep(5);
                    c = ChunkManager.Instance.GetChunk(chunk.Index.x, chunk.Index.y + 1, chunk.Index.z);
                }
                if (c == null) continue;
                chunk = c;
                sw = Stopwatch.StartNew();
                while (!chunk.Generated && sw.ElapsedMilliseconds < 1000) Thread.Sleep(5);
                nextY = 0;
            }
            // LOG
            chunk.Voxels[voxelPos.x, nextY, voxelPos.z] = i == treeHeight - 1 ? Block.LEAVES : Block.LOG;
            // LEAVES
            if (i >= treeHeight - 4)
            {
                int radius = i < treeHeight - 2 ? 2 : 1;
                for (int x = voxelPos.x - radius; x <= voxelPos.x + radius; ++x)
                {
                    for (int z = voxelPos.z - radius; z <= voxelPos.z + radius; ++z)
                    {
                        if (x == voxelPos.x && z == voxelPos.z) continue;
                        if (((x == voxelPos.x - radius && z == voxelPos.z - radius) ||
                             (x == voxelPos.x + radius && z == voxelPos.z + radius) ||
                             (x == voxelPos.x + radius && z == voxelPos.z - radius) ||
                             (x == voxelPos.x - radius && z == voxelPos.z + radius)) && random.Next(0, 10) > 4) continue; // remove some leaves
                        chunk.Voxels[x, nextY, z] = Block.LEAVES;
                    }
                }
            }
            // INCREMENT
            nextY += 1;
        }
    }
}
