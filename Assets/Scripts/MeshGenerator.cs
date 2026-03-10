using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData BuildChunkMesh(World world, ChunkData chunkData)
    {
        MeshData meshData = new MeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {
                for (int x = 0; x < VoxelData.ChunkWidth; x++)
                {
                    Vector3Int localPos = new Vector3Int(x, y, z);
                    BlockType block = chunkData.GetLocalBlock(world, localPos);
                    if (!world.IsSolid(block))
                    {
                        continue;
                    }

                    AddVisibleFaces(world, chunkData, localPos, block, ref meshData);
                }
            }
        }

        return meshData;
    }

    private static void AddVisibleFaces(World world, ChunkData chunkData, Vector3Int localPos, BlockType blockType, ref MeshData meshData)
    {
        Vector3Int worldPos = chunkData.LocalToWorld(localPos);

        for (int face = 0; face < 6; face++)
        {
            Vector3Int neighborWorldPos = worldPos + VoxelData.FaceChecks[face];
            BlockType neighborBlock = world.GetBlock(neighborWorldPos);

            if (world.IsSolid(neighborBlock))
            {
                continue;
            }

            AddFace(world, localPos, face, blockType, ref meshData);
        }
    }

    private static void AddFace(World world, Vector3Int localPos, int faceIndex, BlockType blockType, ref MeshData meshData)
    {
        int vertexIndex = meshData.Vertices.Count;

        meshData.Vertices.Add(localPos + VoxelData.Verts[VoxelData.FaceTriangles[faceIndex, 0]]);
        meshData.Vertices.Add(localPos + VoxelData.Verts[VoxelData.FaceTriangles[faceIndex, 1]]);
        meshData.Vertices.Add(localPos + VoxelData.Verts[VoxelData.FaceTriangles[faceIndex, 2]]);
        meshData.Vertices.Add(localPos + VoxelData.Verts[VoxelData.FaceTriangles[faceIndex, 3]]);

        meshData.Triangles.Add(vertexIndex + 0);
        meshData.Triangles.Add(vertexIndex + 1);
        meshData.Triangles.Add(vertexIndex + 2);
        meshData.Triangles.Add(vertexIndex + 2);
        meshData.Triangles.Add(vertexIndex + 1);
        meshData.Triangles.Add(vertexIndex + 3);

        BlockFace blockFace = VoxelData.FaceToBlockFace[faceIndex];
        int texIndex = world.GetBlockDefinition(blockType).GetTextureIndex(blockFace);
        AddUVs(world, texIndex, ref meshData);
    }

    private static void AddUVs(World world, int textureIndex, ref MeshData meshData)
    {
        float normalized = 1f / world.textureAtlasSizeInBlocks;
        int y = textureIndex / world.textureAtlasSizeInBlocks;
        int x = textureIndex - (y * world.textureAtlasSizeInBlocks);

        Vector2 uv00 = new Vector2(x * normalized, y * normalized);
        Vector2 uv10 = new Vector2((x + 1) * normalized, y * normalized);
        Vector2 uv01 = new Vector2(x * normalized, (y + 1) * normalized);
        Vector2 uv11 = new Vector2((x + 1) * normalized, (y + 1) * normalized);

        meshData.UVs.Add(uv01);
        meshData.UVs.Add(uv00);
        meshData.UVs.Add(uv11);
        meshData.UVs.Add(uv10);
    }
}

public struct MeshData
{
    public List<Vector3> Vertices;
    public List<int> Triangles;
    public List<Vector2> UVs;

    public MeshData()
    {
        Vertices = new List<Vector3>(8192);
        Triangles = new List<int>(8192);
        UVs = new List<Vector2>(8192);
    }
}
