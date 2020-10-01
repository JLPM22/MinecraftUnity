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

    private Vector2 TextureIDTop, TextureIDBottom, TextureIDSide;
    public Vector2[] TextureIDs;

    private readonly static int AtlasSize = 16;
    public readonly static float TexStep = 1.0f / AtlasSize;

    public readonly static BlockInfo[] Blocks = new BlockInfo[]
    {
        new BlockInfo(false, false, 0),           // AIR
        new BlockInfo(true, true, 249),           // DIRT
        new BlockInfo(true, true, 253, 249, 251), // GRASS
        new BlockInfo(true, true, 242),           // COBBLESTONE
        new BlockInfo(true, true, 240),           // STONE
        new BlockInfo(true, true, 2, 2, 0),       // LOG
        new BlockInfo(true, true, 4),             // WOOD
        new BlockInfo(true, false, 255),          // LEAVES
    };

    public BlockInfo(bool visible, bool opaque, int texID)
    {
        Visible = visible;
        Opaque = opaque;
        TextureIDTop = new Vector2((texID % AtlasSize) / 16.0f, (texID / AtlasSize) / 16.0f);
        TextureIDBottom = TextureIDTop;
        TextureIDSide = TextureIDTop;
        TextureIDs = new Vector2[] { TextureIDBottom, TextureIDSide, TextureIDSide, TextureIDSide, TextureIDSide, TextureIDTop };
    }

    public BlockInfo(bool visible, bool opaque, int texTop, int texBottom, int texSide)
    {
        Visible = visible;
        Opaque = opaque;
        TextureIDTop = new Vector2((texTop % AtlasSize) / 16.0f, (texTop / AtlasSize) / 16.0f);
        TextureIDBottom = new Vector2((texBottom % AtlasSize) / 16.0f, (texBottom / AtlasSize) / 16.0f);
        TextureIDSide = new Vector2((texSide % AtlasSize) / 16.0f, (texSide / AtlasSize) / 16.0f);
        TextureIDs = new Vector2[] { TextureIDBottom, TextureIDSide, TextureIDSide, TextureIDSide, TextureIDSide, TextureIDTop };
    }
}