using UnityEngine;

public class Inventory : MonoBehaviour
{
    public BlockType[] hotbar =
    {
        BlockType.Grass,
        BlockType.Dirt,
        BlockType.Stone,
        BlockType.Sand,
        BlockType.Wood,
        BlockType.Leaves
    };

    public int selectedIndex;

    public BlockType SelectedBlock => hotbar[Mathf.Clamp(selectedIndex, 0, hotbar.Length - 1)];

    private void Update()
    {
        if (hotbar == null || hotbar.Length == 0)
        {
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedIndex = (selectedIndex + 1) % hotbar.Length;
        }
        else if (scroll < 0f)
        {
            selectedIndex = (selectedIndex - 1 + hotbar.Length) % hotbar.Length;
        }

        for (int i = 0; i < hotbar.Length && i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                selectedIndex = i;
                break;
            }
        }
    }
}
