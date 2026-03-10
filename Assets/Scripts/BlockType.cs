using System;
using UnityEngine;

public enum BlockType
{
    Air = 0,
    Grass = 1,
    Dirt = 2,
    Stone = 3,
    Sand = 4,
    Wood = 5,
    Leaves = 6
}

public enum BlockFace
{
    Top,
    Bottom,
    Side
}

[Serializable]
public class BlockDefinition
{
    public BlockType type;
    public bool isSolid = true;
    public int topTexture = 0;
    public int bottomTexture = 0;
    public int sideTexture = 0;

    public int GetTextureIndex(BlockFace face)
    {
        return face switch
        {
            BlockFace.Top => topTexture,
            BlockFace.Bottom => bottomTexture,
            _ => sideTexture
        };
    }
}
