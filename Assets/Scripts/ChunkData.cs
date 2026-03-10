using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkCoord;
    private readonly Dictionary<int, BlockType> modifiedBlocks = new();

    public ChunkData(Vector2Int coord)
    {
        chunkCoord = coord;
    }

    public bool TryGetModifiedBlock(Vector3Int localPos, out BlockType blockType)
    {
        int index = VoxelData.ToBlockIndex(localPos);
        return modifiedBlocks.TryGetValue(index, out blockType);
    }

    public BlockType GetLocalBlock(World world, Vector3Int localPos)
    {
        if (TryGetModifiedBlock(localPos, out BlockType modified))
        {
            return modified;
        }

        Vector3Int worldPos = LocalToWorld(localPos);
        return TerrainGenerator.GetBlockType(world, worldPos);
    }

    public void SetLocalBlock(Vector3Int localPos, BlockType blockType)
    {
        int index = VoxelData.ToBlockIndex(localPos);
        modifiedBlocks[index] = blockType;
    }

    public Dictionary<int, BlockType> GetAllModifications()
    {
        return modifiedBlocks;
    }

    public Vector3Int LocalToWorld(Vector3Int localPos)
    {
        return new Vector3Int(
            chunkCoord.x * VoxelData.ChunkWidth + localPos.x,
            localPos.y,
            chunkCoord.y * VoxelData.ChunkWidth + localPos.z);
    }
}
