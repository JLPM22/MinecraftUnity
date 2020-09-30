using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayer : MonoBehaviour
{
    public GameObject SelectorPrefab;
    public float Distance = 5.0f;

    private Camera Camera;
    private GameObject Selector;

    public Chunk SelectedChunk { get; private set; }
    public Vector3Int SelectedVoxel { get; private set; }
    public Chunk SelectedNextChunk { get; private set; }
    public Vector3Int SelectedNextVoxel { get; private set; }

    private void Awake()
    {
        Camera = Camera.main;
        Selector = GameObject.Instantiate<GameObject>(SelectorPrefab);
        DisableSelector();
    }

    void Update()
    {
        bool voxelFound = false;
        SelectedChunk = null;
        SelectedVoxel = Vector3Int.zero;

        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(ray, out RaycastHit hit, Distance))
        {
            Vector3 pos = hit.point - hit.normal * 0.05f; // Move the point a little bit inside the voxel
            int chunkX = Mathf.FloorToInt(pos.x / Chunk.ChunkSize.x);
            int chunkY = Mathf.FloorToInt(pos.y / Chunk.ChunkSize.y);
            int chunkZ = Mathf.FloorToInt(pos.z / Chunk.ChunkSize.z);
            SelectedChunk = ChunkManager.Instance.GetChunk(chunkX, chunkY, chunkZ);
            if (SelectedChunk != null)
            {
                int z = pos.z >= 0.0f ? (int)pos.z : (int)pos.z - 1;
                int y = pos.y >= 0.0f ? (int)pos.y : (int)pos.y - 1;
                int x = pos.x >= 0.0f ? (int)pos.x : (int)pos.x - 1;
                SelectedVoxel = new Vector3Int((x % Chunk.ChunkSize.x + Chunk.ChunkSize.x) % Chunk.ChunkSize.x, // To allow negative coordinates
                                               (y % Chunk.ChunkSize.y + Chunk.ChunkSize.y) % Chunk.ChunkSize.y,
                                               (z % Chunk.ChunkSize.z + Chunk.ChunkSize.z) % Chunk.ChunkSize.z);
                UpdateNextVoxel(hit.normal, SelectedChunk, SelectedVoxel);
                voxelFound = true;
            }
        }

        if (voxelFound)
        {
            EnableSelector(SelectedChunk, SelectedVoxel);
        }
        else
        {
            DisableSelector();
        }
    }

    private void UpdateNextVoxel(Vector3 normal, Chunk chunk, Vector3Int voxelPos)
    {
        if (Vector3.Dot(normal, Vector3.right) > 0)
        {
            if (voxelPos.x + 1 >= Chunk.ChunkSize.x)
            {
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x + 1, chunk.Index.y, chunk.Index.z);
                chunk = c;
                voxelPos.x = 0;
            }
            else
            {
                voxelPos.x += 1;
            }
        }
        else if (Vector3.Dot(normal, Vector3.left) > 0)
        {
            if (voxelPos.x - 1 < 0)
            {
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x - 1, chunk.Index.y, chunk.Index.z);
                chunk = c;
                voxelPos.x = Chunk.ChunkSize.x - 1;
            }
            else
            {
                voxelPos.x -= 1;
            }
        }
        else if (Vector3.Dot(normal, Vector3.up) > 0)
        {
            if (voxelPos.y + 1 >= Chunk.ChunkSize.y)
            {
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x, chunk.Index.y + 1, chunk.Index.z);
                chunk = c;
                voxelPos.y = 0;
            }
            else
            {
                voxelPos.y += 1;
            }
        }
        else if (Vector3.Dot(normal, Vector3.down) > 0)
        {
            if (voxelPos.y - 1 < 0)
            {
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x, chunk.Index.y - 1, chunk.Index.z);
                chunk = c;
                voxelPos.y = Chunk.ChunkSize.y - 1;
            }
            else
            {
                voxelPos.y -= 1;
            }
        }
        else if (Vector3.Dot(normal, Vector3.forward) > 0)
        {
            if (voxelPos.z + 1 >= Chunk.ChunkSize.z)
            {
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x, chunk.Index.y, chunk.Index.z + 1);
                chunk = c;
                voxelPos.z = 0;
            }
            else
            {
                voxelPos.z += 1;
            }
        }
        else
        {
            if (voxelPos.z - 1 < 0)
            {
                Chunk c = ChunkManager.Instance.GetChunk(chunk.Index.x, chunk.Index.y, chunk.Index.z - 1);
                chunk = c;
                voxelPos.z = Chunk.ChunkSize.z - 1;
            }
            else
            {
                voxelPos.z -= 1;
            }
        }

        SelectedNextChunk = chunk;
        SelectedNextVoxel = voxelPos;
    }

    private void EnableSelector(Chunk chunk, Vector3Int voxelPos)
    {
        Selector.transform.position = Vector3.Scale(chunk.Index, Chunk.ChunkSize) + voxelPos + new Vector3(0.5f, 0.5f, 0.5f);
        if (!Selector.activeSelf) Selector.SetActive(true);
    }

    private void DisableSelector()
    {
        if (Selector.activeSelf) Selector.SetActive(false);
    }
}
