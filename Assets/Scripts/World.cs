using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Material chunkMaterial;
    public Light sunLight;

    [Header("World")]
    public int seed = 1337;
    public int viewDistanceInChunks = 8;
    public int textureAtlasSizeInBlocks = 4;

    [Header("Terrain")]
    public float terrainScale = 0.02f;
    public int terrainHeight = 30;
    public int baseGroundHeight = 50;
    public int waterLevel = 48;
    public float caveScale = 0.05f;
    [Range(0f, 1f)] public float caveThreshold = 0.72f;
    public int treeSpawnChance = 60;

    [Header("Day Night")]
    public float dayLengthInSeconds = 180f;

    [Header("Blocks")]
    public BlockDefinition[] blockDefinitions;

    private readonly Dictionary<Vector2Int, Chunk> activeChunks = new();
    private readonly Dictionary<Vector2Int, ChunkData> chunkDatas = new();
    private readonly Dictionary<BlockType, BlockDefinition> blockLookup = new();

    private float dayTimer;

    public IReadOnlyDictionary<Vector2Int, Chunk> ActiveChunks => activeChunks;

    private void Awake()
    {
        BuildBlockLookup();

        Dictionary<Vector2Int, ChunkData> loaded = SaveSystem.LoadAllChunks();
        foreach (KeyValuePair<Vector2Int, ChunkData> kv in loaded)
        {
            chunkDatas[kv.Key] = kv.Value;
        }
    }

    private void Update()
    {
        UpdateDayNightCycle();
    }

    public ChunkData GetOrCreateChunkData(Vector2Int coord)
    {
        if (chunkDatas.TryGetValue(coord, out ChunkData data))
        {
            return data;
        }

        data = new ChunkData(coord);
        chunkDatas[coord] = data;
        return data;
    }

    public void RegisterChunk(Vector2Int coord, Chunk chunk)
    {
        activeChunks[coord] = chunk;
    }

    public void UnregisterChunk(Vector2Int coord)
    {
        activeChunks.Remove(coord);
    }

    public bool TryGetActiveChunk(Vector2Int coord, out Chunk chunk)
    {
        return activeChunks.TryGetValue(coord, out chunk);
    }

    public BlockType GetBlock(Vector3Int worldPos)
    {
        if (worldPos.y < 0 || worldPos.y >= VoxelData.ChunkHeight)
        {
            return BlockType.Air;
        }

        Vector2Int chunkCoord = VoxelData.WorldToChunkCoord(worldPos);
        ChunkData data = GetOrCreateChunkData(chunkCoord);
        Vector3Int localPos = VoxelData.WorldToLocalBlockPos(worldPos);
        return data.GetLocalBlock(this, localPos);
    }

    public bool SetBlock(Vector3Int worldPos, BlockType blockType)
    {
        if (worldPos.y < 0 || worldPos.y >= VoxelData.ChunkHeight)
        {
            return false;
        }

        Vector2Int chunkCoord = VoxelData.WorldToChunkCoord(worldPos);
        Vector3Int localPos = VoxelData.WorldToLocalBlockPos(worldPos);
        ChunkData data = GetOrCreateChunkData(chunkCoord);
        data.SetLocalBlock(localPos, blockType);
        SaveSystem.SaveChunk(data);

        RefreshChunkAndNeighbors(chunkCoord, localPos);
        return true;
    }

    public void RefreshChunk(Vector2Int coord)
    {
        if (activeChunks.TryGetValue(coord, out Chunk chunk))
        {
            chunk.RebuildMesh();
        }
    }

    public bool IsSolid(BlockType blockType)
    {
        if (blockType == BlockType.Air)
        {
            return false;
        }

        return GetBlockDefinition(blockType).isSolid;
    }

    public BlockDefinition GetBlockDefinition(BlockType blockType)
    {
        if (blockLookup.TryGetValue(blockType, out BlockDefinition definition))
        {
            return definition;
        }

        return blockLookup[BlockType.Dirt];
    }

    private void RefreshChunkAndNeighbors(Vector2Int chunkCoord, Vector3Int localPos)
    {
        RefreshChunk(chunkCoord);

        if (localPos.x == 0) RefreshChunk(chunkCoord + Vector2Int.left);
        if (localPos.x == VoxelData.ChunkWidth - 1) RefreshChunk(chunkCoord + Vector2Int.right);
        if (localPos.z == 0) RefreshChunk(chunkCoord + new Vector2Int(0, -1));
        if (localPos.z == VoxelData.ChunkWidth - 1) RefreshChunk(chunkCoord + new Vector2Int(0, 1));
    }

    private void BuildBlockLookup()
    {
        blockLookup.Clear();
        foreach (BlockDefinition definition in blockDefinitions)
        {
            blockLookup[definition.type] = definition;
        }

        if (!blockLookup.ContainsKey(BlockType.Air))
        {
            blockLookup[BlockType.Air] = new BlockDefinition { type = BlockType.Air, isSolid = false };
        }

        if (!blockLookup.ContainsKey(BlockType.Dirt))
        {
            blockLookup[BlockType.Dirt] = new BlockDefinition { type = BlockType.Dirt, isSolid = true, topTexture = 1, sideTexture = 1, bottomTexture = 1 };
        }
    }

    private void UpdateDayNightCycle()
    {
        if (sunLight == null || dayLengthInSeconds <= 0f)
        {
            return;
        }

        dayTimer += Time.deltaTime;
        float normalizedTime = (dayTimer % dayLengthInSeconds) / dayLengthInSeconds;
        float sunAngle = normalizedTime * 360f - 90f;

        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        float intensity = Mathf.Clamp01(Mathf.Cos((normalizedTime - 0.25f) * Mathf.PI * 2f) * 1.2f + 0.1f);
        sunLight.intensity = Mathf.Lerp(0.1f, 1.2f, intensity);
        RenderSettings.ambientIntensity = Mathf.Lerp(0.2f, 1f, intensity);
    }
}
