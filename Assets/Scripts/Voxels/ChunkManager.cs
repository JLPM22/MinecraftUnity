using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;

    public GameObject Player;
    public int Radius;

    private Dictionary<Vector3Int, Chunk> Chunks = new Dictionary<Vector3Int, Chunk>();

    private Vector2Int LastPlayerChunk = new Vector2Int(int.MaxValue, int.MaxValue);

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        int chunkX = Mathf.FloorToInt(Player.transform.position.x / Chunk.ChunkSize.x);
        int chunkZ = Mathf.FloorToInt(Player.transform.position.z / Chunk.ChunkSize.z);

        if (chunkX != LastPlayerChunk.x || chunkZ != LastPlayerChunk.y)
        {
            RemoveOldChunks(chunkX, chunkZ);
            AddNewChunks(chunkX, chunkZ);
        }

        LastPlayerChunk = new Vector2Int(chunkX, chunkZ);
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        return Chunks[new Vector3Int(x, y, z)];
    }

    private void RemoveOldChunks(int chunkX, int chunkZ)
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, Chunk> entry in Chunks)
        {
            entry.Value.Delete = true;
            if (Mathf.Abs(entry.Key.x - chunkX) > Radius ||
                Mathf.Abs(entry.Key.z - chunkZ) > Radius) toRemove.Add(entry.Key);
        }
        foreach (Vector3Int index in toRemove) Chunks.Remove(index);
    }

    private void AddNewChunks(int chunkX, int chunkZ)
    {
        for (int z = chunkZ - Radius; z <= chunkZ + Radius; ++z)
        {
            for (int x = chunkX - Radius; x <= chunkX + Radius; ++x)
            {
                for (int y = 0; y < Chunk.NumberVerticalChunks; ++y)
                {
                    Vector3Int index = new Vector3Int(x, y, z);
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
    }

    private void CreateChunk(Vector3Int index)
    {
        // Create GameObject, Componentes, Position and Parent
        GameObject newChunk = new GameObject(index.ToString());
        Chunk c = newChunk.AddComponent<Chunk>();
        c.Index = index;
        ChunkRenderer renderer = newChunk.AddComponent<ChunkRenderer>();
        newChunk.transform.position = Vector3.Scale(Chunk.ChunkSize, index);
        newChunk.transform.SetParent(transform); // Chunk is a child of ChunkManager
                                                 // Add to Dictionary
        Chunks.Add(index, c);
        // Procedural Generation
        ProceduralGeneration.GenerateChunk(c);
        // Mesh
        renderer.RegenerateMesh();
    }
}
