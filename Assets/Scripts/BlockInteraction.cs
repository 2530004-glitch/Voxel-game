using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public World world;
    public Camera playerCamera;
    public Inventory inventory;
    public float reachDistance = 6f;

    private void Update()
    {
        if (world == null || playerCamera == null || inventory == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryBreakBlock();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryPlaceBlock();
        }
    }

    private void TryBreakBlock()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, reachDistance))
        {
            Vector3 point = hit.point - hit.normal * 0.01f;
            Vector3Int blockPos = Vector3Int.FloorToInt(point);
            BlockType block = world.GetBlock(blockPos);

            if (block != BlockType.Air)
            {
                world.SetBlock(blockPos, BlockType.Air);
            }
        }
    }

    private void TryPlaceBlock()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, reachDistance))
        {
            Vector3 point = hit.point + hit.normal * 0.01f;
            Vector3Int placePos = Vector3Int.FloorToInt(point);

            if (world.GetBlock(placePos) != BlockType.Air)
            {
                return;
            }

            world.SetBlock(placePos, inventory.SelectedBlock);
        }
    }
}
