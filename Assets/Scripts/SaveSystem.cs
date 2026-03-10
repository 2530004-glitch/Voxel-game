using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFolder = "VoxelSaves";

    public static void SaveChunk(ChunkData chunk)
    {
        ChunkSaveData saveData = new ChunkSaveData
        {
            x = chunk.chunkCoord.x,
            z = chunk.chunkCoord.y,
            modifications = new List<BlockModification>()
        };

        foreach (KeyValuePair<int, BlockType> kv in chunk.GetAllModifications())
        {
            saveData.modifications.Add(new BlockModification
            {
                index = kv.Key,
                blockType = (int)kv.Value
            });
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(GetChunkPath(chunk.chunkCoord), json);
    }

    public static ChunkData LoadChunk(Vector2Int coord)
    {
        string path = GetChunkPath(coord);
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        ChunkSaveData saveData = JsonUtility.FromJson<ChunkSaveData>(json);
        ChunkData data = new ChunkData(new Vector2Int(saveData.x, saveData.z));

        if (saveData.modifications != null)
        {
            foreach (BlockModification mod in saveData.modifications)
            {
                Vector3Int pos = IndexToLocal(mod.index);
                data.SetLocalBlock(pos, (BlockType)mod.blockType);
            }
        }

        return data;
    }

    public static Dictionary<Vector2Int, ChunkData> LoadAllChunks()
    {
        EnsureDirectory();

        Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();
        string folder = GetSaveDirectory();
        string[] files = Directory.GetFiles(folder, "chunk_*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            ChunkSaveData saveData = JsonUtility.FromJson<ChunkSaveData>(json);
            ChunkData data = new ChunkData(new Vector2Int(saveData.x, saveData.z));

            if (saveData.modifications != null)
            {
                foreach (BlockModification mod in saveData.modifications)
                {
                    Vector3Int pos = IndexToLocal(mod.index);
                    data.SetLocalBlock(pos, (BlockType)mod.blockType);
                }
            }

            chunks[data.chunkCoord] = data;
        }

        return chunks;
    }

    private static string GetSaveDirectory()
    {
        return Path.Combine(Application.persistentDataPath, SaveFolder);
    }

    private static string GetChunkPath(Vector2Int coord)
    {
        EnsureDirectory();
        return Path.Combine(GetSaveDirectory(), $"chunk_{coord.x}_{coord.y}.json");
    }

    private static void EnsureDirectory()
    {
        string dir = GetSaveDirectory();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private static Vector3Int IndexToLocal(int index)
    {
        int y = index / (VoxelData.ChunkWidth * VoxelData.ChunkWidth);
        int rem = index % (VoxelData.ChunkWidth * VoxelData.ChunkWidth);
        int z = rem / VoxelData.ChunkWidth;
        int x = rem % VoxelData.ChunkWidth;

        return new Vector3Int(x, y, z);
    }
}

[System.Serializable]
public class ChunkSaveData
{
    public int x;
    public int z;
    public List<BlockModification> modifications;
}

[System.Serializable]
public class BlockModification
{
    public int index;
    public int blockType;
}
