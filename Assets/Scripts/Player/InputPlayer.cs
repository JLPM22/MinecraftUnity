using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlayer : MonoBehaviour
{
    public float TimeBetweenClicks = 0.25f;

    private Block CurrentSelectedBlock = Block.DIRT;
    private RaycastPlayer RaycastPlayer;
    private float LastClickTime;

    private void Awake()
    {
        RaycastPlayer = GetComponent<RaycastPlayer>();
    }

    void Update()
    {
        if (RaycastPlayer.SelectedChunk == null || LastClickTime + TimeBetweenClicks > Time.time) return;

        // Remove Block
        if (Input.GetMouseButton(0)) // Left Click
        {
            Vector3Int voxel = RaycastPlayer.SelectedVoxel;
            RaycastPlayer.SelectedChunk.SetBlock(voxel.x, voxel.y, voxel.z, Block.AIR);
            LastClickTime = Time.time;
        }
        // Add Block
        if (Input.GetMouseButton(1) && RaycastPlayer.SelectedNextChunk != null) // Right Click
        {
            Vector3Int voxel = RaycastPlayer.SelectedNextVoxel;
            RaycastPlayer.SelectedChunk.SetBlock(voxel.x, voxel.y, voxel.z, CurrentSelectedBlock);
            LastClickTime = Time.time;
        }
    }
}
