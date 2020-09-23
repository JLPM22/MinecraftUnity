using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject Player;
    public int Radius;

    private Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    private void Update()
    {
        int chunkX = Mathf.FloorToInt(Player.transform.position.x / Chunk.ChunkSize);
        int chunkZ = Mathf.FloorToInt(Player.transform.position.z / Chunk.ChunkSize);

        for (int z = chunkZ - Radius; z <= chunkZ + Radius; ++z)
        {
            for (int x = chunkX - Radius; x <= chunkX + Radius; ++x)
            {

            }
        }
    }
}
