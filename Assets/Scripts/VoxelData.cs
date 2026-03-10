using UnityEngine;

public static class VoxelData
{
    public const int ChunkWidth = 16;
    public const int ChunkHeight = 256;
    public const float BlockSize = 1f;

    public static readonly Vector3[] Verts =
    {
        new(0, 0, 0), // 0
        new(1, 0, 0), // 1
        new(1, 1, 0), // 2
        new(0, 1, 0), // 3
        new(0, 0, 1), // 4
        new(1, 0, 1), // 5
        new(1, 1, 1), // 6
        new(0, 1, 1)  // 7
    };

    public static readonly int[,] FaceTriangles =
    {
        { 0, 3, 1, 2 }, // back (z-)
        { 5, 6, 4, 7 }, // front (z+)
        { 3, 7, 2, 6 }, // top (y+)
        { 1, 5, 0, 4 }, // bottom (y-)
        { 4, 7, 0, 3 }, // left (x-)
        { 1, 2, 5, 6 }  // right (x+)
    };

    public static readonly Vector3Int[] FaceChecks =
    {
        new(0, 0, -1),
        new(0, 0, 1),
        new(0, 1, 0),
        new(0, -1, 0),
        new(-1, 0, 0),
        new(1, 0, 0)
    };

    public static readonly BlockFace[] FaceToBlockFace =
    {
        BlockFace.Side,
        BlockFace.Side,
        BlockFace.Top,
        BlockFace.Bottom,
        BlockFace.Side,
        BlockFace.Side
    };

    public static int ToBlockIndex(Vector3Int localPos)
    {
        return localPos.x + ChunkWidth * (localPos.z + ChunkWidth * localPos.y);
    }

    public static Vector2Int WorldToChunkCoord(Vector3Int worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / (float)ChunkWidth);
        int z = Mathf.FloorToInt(worldPos.z / (float)ChunkWidth);
        return new Vector2Int(x, z);
    }

    public static Vector3Int WorldToLocalBlockPos(Vector3Int worldPos)
    {
        int lx = Mod(worldPos.x, ChunkWidth);
        int lz = Mod(worldPos.z, ChunkWidth);
        return new Vector3Int(lx, worldPos.y, lz);
    }

    public static int Mod(int value, int m)
    {
        int result = value % m;
        return result < 0 ? result + m : result;
    }
}
