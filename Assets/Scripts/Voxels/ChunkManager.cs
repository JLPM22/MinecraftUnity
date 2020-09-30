using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;

    [Header("Player")]
    public GameObject Player;
    public int Radius;

    [Header("Terrain")]
    public float Scale = 50.0f;
    [Tooltip("Number of functions adding detail (each one is less influent than the previous one)")]
    public int Octaves = 6;
    [Tooltip("Amplitude magnification factor of each octave (exponential)")]
    [Range(0.0f, 1.0f)]
    public float Persistance = 0.5f;
    [Tooltip("Frequency magnification factor of each octave (exponential)")]
    public float Lacunarity = 2.0f;
    public int Seed;

    private Dictionary<Vector3Int, Chunk> Chunks = new Dictionary<Vector3Int, Chunk>();
    private Vector2Int LastPlayerChunk = new Vector2Int(int.MaxValue, int.MaxValue);
    private List<Chunk> NotFinishedChunks = new List<Chunk>(); // Main Thread have to do some Unity functions (not thread-safe)

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

        // Finish chunk creation
        for (int i = 0; i < NotFinishedChunks.Count; ++i)
        {
            Chunk c = NotFinishedChunks[i];
            if (c.AsyncFinished)
            {
                // Physics
                MeshCollider collider = c.gameObject.AddComponent<MeshCollider>();
                c.gameObject.GetComponent<ChunkRenderer>().MeshCollider = collider;
                collider.sharedMesh = c.gameObject.GetComponent<MeshFilter>().sharedMesh;
                collider.cookingOptions = MeshColliderCookingOptions.None; // Fast recompute
                // Remove to the pending chunks list
                NotFinishedChunks.RemoveAt(i--);
            }
        }
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        if (Chunks.TryGetValue(new Vector3Int(x, y, z), out Chunk c))
            return c;
        return null;
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
        c.SetChunkRenderer(renderer);
        newChunk.transform.position = Vector3.Scale(Chunk.ChunkSize, index) + new Vector3(0.5f, 0.5f, 0.5f);
        newChunk.transform.SetParent(transform, true); // Chunk is a child of ChunkManager
                                                       // Add to Dictionary
        Chunks.Add(index, c);
        // Procedural Generation & Mesh
        ProceduralGeneration.AsyncGenerateChunk(c, () => renderer.RegenerateMesh());
        // Main Thread operations
        NotFinishedChunks.Add(c);
    }

    // Debug Purposes
    public void RegenerateAll()
    {
        foreach (Chunk c in Chunks.Values)
        {
            ChunkRenderer renderer = c.GetComponent<ChunkRenderer>();
            // Procedural Generation & Mesh
            ProceduralGeneration.AsyncGenerateChunk(c, () => renderer.RegenerateMesh());
        }
    }
}

// Debug Purposes
#if UNITY_EDITOR
[CustomEditor(typeof(ChunkManager))]
public class ChunkManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        if (GUILayout.Button("Regenerate"))
        {
            ((ChunkManager)target).RegenerateAll();
        }
    }
}
#endif
