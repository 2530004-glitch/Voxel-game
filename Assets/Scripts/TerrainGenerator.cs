using UnityEngine;

public static class TerrainGenerator
{
    public static BlockType GetBlockType(World world, Vector3Int worldPos)
    {
        if (worldPos.y < 0 || worldPos.y >= VoxelData.ChunkHeight)
        {
            return BlockType.Air;
        }

        int height = GetTerrainHeight(world, worldPos.x, worldPos.z);

        if (worldPos.y > height)
        {
            if (worldPos.y <= world.waterLevel)
            {
                return BlockType.Sand;
            }

            if (TryGetTreeBlock(world, worldPos, out BlockType treeBlock))
            {
                return treeBlock;
            }

            return BlockType.Air;
        }

        float caveNoise = Mathf.PerlinNoise(
            (worldPos.x + world.seed * 0.123f) * world.caveScale,
            (worldPos.y + worldPos.z + world.seed * 0.456f) * world.caveScale);

        if (worldPos.y > 8 && worldPos.y < height - 3 && caveNoise > world.caveThreshold)
        {
            return BlockType.Air;
        }

        if (worldPos.y == height)
        {
            return height <= world.waterLevel + 1 ? BlockType.Sand : BlockType.Grass;
        }

        if (worldPos.y >= height - 3)
        {
            return BlockType.Dirt;
        }

        return BlockType.Stone;
    }

    public static int GetTerrainHeight(World world, int x, int z)
    {
        float n1 = Mathf.PerlinNoise((x + world.seed) * world.terrainScale, (z + world.seed) * world.terrainScale);
        float n2 = Mathf.PerlinNoise((x - world.seed) * world.terrainScale * 0.5f, (z - world.seed) * world.terrainScale * 0.5f);
        float combined = n1 * 0.7f + n2 * 0.3f;

        return Mathf.FloorToInt(combined * world.terrainHeight) + world.baseGroundHeight;
    }

    private static bool TryGetTreeBlock(World world, Vector3Int worldPos, out BlockType treeBlock)
    {
        treeBlock = BlockType.Air;

        for (int ox = -2; ox <= 2; ox++)
        {
            for (int oz = -2; oz <= 2; oz++)
            {
                int centerX = worldPos.x + ox;
                int centerZ = worldPos.z + oz;
                int hash = Mathf.Abs(Hash(centerX, centerZ, world.seed));

                if (hash % world.treeSpawnChance != 0)
                {
                    continue;
                }

                int baseY = GetTerrainHeight(world, centerX, centerZ);
                if (baseY <= world.waterLevel + 1)
                {
                    continue;
                }

                int treeHeight = 4 + (hash % 3);

                if (worldPos.x == centerX && worldPos.z == centerZ && worldPos.y > baseY && worldPos.y <= baseY + treeHeight)
                {
                    treeBlock = BlockType.Wood;
                    return true;
                }

                int leafBaseY = baseY + treeHeight - 2;
                if (worldPos.y >= leafBaseY && worldPos.y <= baseY + treeHeight + 1)
                {
                    int dx = Mathf.Abs(worldPos.x - centerX);
                    int dz = Mathf.Abs(worldPos.z - centerZ);
                    int dy = worldPos.y - leafBaseY;
                    int radius = dy == 3 ? 1 : 2;

                    if (dx <= radius && dz <= radius)
                    {
                        if (!(dx == 0 && dz == 0 && worldPos.y <= baseY + treeHeight))
                        {
                            treeBlock = BlockType.Leaves;
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private static int Hash(int x, int z, int seed)
    {
        int h = x;
        h = h * 374761393 + z * 668265263 + seed * 1443059017;
        h = (h ^ (h >> 13)) * 1274126177;
        return h ^ (h >> 16);
    }
}
