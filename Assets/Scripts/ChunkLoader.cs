using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public World world;
    public Transform player;
    public int maxChunkUpdatesPerFrame = 4;

    private readonly Queue<Chunk> pool = new();
    private readonly HashSet<Vector2Int> requiredCoords = new();

    private void Update()
    {
        if (world == null || player == null)
        {
            return;
        }

        UpdateVisibleChunks();
    }

    private void UpdateVisibleChunks()
    {
        requiredCoords.Clear();

        Vector2Int playerChunk = VoxelData.WorldToChunkCoord(Vector3Int.FloorToInt(player.position));
        int radius = world.viewDistanceInChunks;

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int coord = playerChunk + new Vector2Int(x, z);
                requiredCoords.Add(coord);
            }
        }

        int updates = 0;
        foreach (Vector2Int coord in requiredCoords)
        {
            if (updates >= maxChunkUpdatesPerFrame)
            {
                break;
            }

            if (!world.TryGetActiveChunk(coord, out _))
            {
                LoadChunk(coord);
                updates++;
            }
        }

        List<Vector2Int> toUnload = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Chunk> kv in world.ActiveChunks)
        {
            if (!requiredCoords.Contains(kv.Key))
            {
                toUnload.Add(kv.Key);
            }
        }

        foreach (Vector2Int coord in toUnload)
        {
            UnloadChunk(coord);
        }
    }

    private void LoadChunk(Vector2Int coord)
    {
        Chunk chunk;
        if (pool.Count > 0)
        {
            chunk = pool.Dequeue();
            chunk.gameObject.SetActive(true);
        }
        else
        {
            GameObject chunkObj = new GameObject();
            chunk = chunkObj.AddComponent<Chunk>();
        }

        ChunkData chunkData = world.GetOrCreateChunkData(coord);
        chunk.Initialize(world, chunkData);
        world.RegisterChunk(coord, chunk);
    }

    private void UnloadChunk(Vector2Int coord)
    {
        if (!world.TryGetActiveChunk(coord, out Chunk chunk))
        {
            return;
        }

        world.UnregisterChunk(coord);
        chunk.gameObject.SetActive(false);
        pool.Enqueue(chunk);
    }
}
