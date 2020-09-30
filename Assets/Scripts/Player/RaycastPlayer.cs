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
                int x = (int)pos.x;
                int y = (int)pos.y;
                int z = (int)pos.z;
                SelectedVoxel = new Vector3Int((x % Chunk.ChunkSize.x + Chunk.ChunkSize.x) % Chunk.ChunkSize.x, // To allow negative coordinates
                                          (y % Chunk.ChunkSize.y + Chunk.ChunkSize.y) % Chunk.ChunkSize.y,
                                          (z % Chunk.ChunkSize.z + Chunk.ChunkSize.z) % Chunk.ChunkSize.z);
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
