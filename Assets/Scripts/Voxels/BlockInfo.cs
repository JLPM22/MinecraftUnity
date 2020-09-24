using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Block
{
    AIR = 0,
    DIRT = 1,
    GRASS = 2,
    COBBLESTONE = 3,
    STONE = 4,
    LOG = 5,
    WOOD = 6,
    LEAVES = 7,
    WATER = 8
}

public class BlockInfo
{
    public bool Visible;
    public bool Opaque;

    public readonly static BlockInfo[] Blocks = new BlockInfo[]
    {
        new BlockInfo(false, false), // AIR
        new BlockInfo(true, true),   // DIRT
        new BlockInfo(true, true),   // GRASS
        new BlockInfo(true, true),   // COBBLESTONE
        new BlockInfo(true, true),   // STONE
        new BlockInfo(true, true),   // LOG
        new BlockInfo(true, true),   // WOOD
        new BlockInfo(true, false),  // LEAVES
        new BlockInfo(true, false),  // WATER
    };

    public BlockInfo(bool visible, bool opaque)
    {
        Visible = visible;
        Opaque = opaque;
    }
}