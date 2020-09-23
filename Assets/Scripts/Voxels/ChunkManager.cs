using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject Player;
    public int Radius;

    private Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    private Vector2Int LastPlayerChunk = new Vector2Int(int.MaxValue, int.MaxValue);

    private void Update()
    {
        int chunkX = Mathf.FloorToInt(Player.transform.position.x / Chunk.ChunkSize.x);
        int chunkZ = Mathf.FloorToInt(Player.transform.position.z / Chunk.ChunkSize.z);

        if (chunkX != LastPlayerChunk.x || chunkZ != LastPlayerChunk.y)
        {
            RemoveOldChunks();
            AddNewChunks(chunkX, chunkZ);
        }

        LastPlayerChunk = new Vector2Int(chunkX, chunkZ);
    }

    private void RemoveOldChunks()
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Chunk> entry in Chunks)
        {
            entry.Value.Delete = true;
            toRemove.Add(entry.Key);
        }
        foreach (Vector2Int index in toRemove) Chunks.Remove(index);
    }

    private void AddNewChunks(int chunkX, int chunkZ)
    {
        for (int z = chunkZ - Radius; z <= chunkZ + Radius; ++z)
        {
            for (int x = chunkX - Radius; x <= chunkX + Radius; ++x)
            {
                Vector2Int index = new Vector2Int(x, z);
                if (Chunks.TryGetValue(index, out Chunk c))
                {
                    // Already exists
                    c.Delete = false;
                }
                else
                {
                    // Create new chunk
                    CreateChunk(index);
                }
            }
        }
    }

    private void CreateChunk(Vector2Int index)
    {
        for (int y = 0; y < Chunk.ChunkSize.y; ++y)
        {
            // Create GameObject, Componentes, and Position
            GameObject newChunk = new GameObject(index.ToString());
            Chunk c = gameObject.AddComponent<Chunk>();
            ChunkRenderer renderer = gameObject.AddComponent<ChunkRenderer>();
            gameObject.transform.position = Vector3.Scale(Chunk.ChunkSize, new Vector3Int(index.x, y, index.y));
            // Add to Dictionary
            Chunks.Add(index, c);
            // Procedural Generation
            ProceduralGeneration.GenerateChunk(c);
            // Mesh
            renderer.GenerateMesh();
        }
    }
}
