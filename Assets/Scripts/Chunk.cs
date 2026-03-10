using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public ChunkData Data { get; private set; }

    private World world;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public void Initialize(World worldRef, ChunkData chunkData)
    {
        world = worldRef;
        Data = chunkData;

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if (meshCollider == null)
        {
            meshCollider = GetComponent<MeshCollider>();
        }

        transform.position = new Vector3(
            Data.chunkCoord.x * VoxelData.ChunkWidth,
            0,
            Data.chunkCoord.y * VoxelData.ChunkWidth);

        gameObject.name = $"Chunk_{Data.chunkCoord.x}_{Data.chunkCoord.y}";

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = world.chunkMaterial;

        RebuildMesh();
    }

    public void RebuildMesh()
    {
        MeshData meshData = MeshGenerator.BuildChunkMesh(world, Data);
        Mesh mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        mesh.SetVertices(meshData.Vertices);
        mesh.SetTriangles(meshData.Triangles, 0);
        mesh.SetUVs(0, meshData.UVs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
}
