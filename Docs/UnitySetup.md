# Unity Voxel Sandbox Setup Guide

## Recommended Folder Structure

Create this structure inside `Assets/`:

- `Assets/Materials/`
  - `M_ChunkAtlas.mat`
- `Assets/Textures/`
  - `T_BlockAtlas.png` (texture atlas, point-filtered)
- `Assets/Prefabs/` (optional)
- `Assets/Scripts/`
  - `World.cs`
  - `Chunk.cs`
  - `ChunkData.cs`
  - `VoxelData.cs`
  - `BlockType.cs`
  - `TerrainGenerator.cs`
  - `PlayerController.cs`
  - `Inventory.cs`
  - `BlockInteraction.cs`
  - `ChunkLoader.cs`
  - `MeshGenerator.cs`
  - `SaveSystem.cs`

## Scene Setup

1. Create an empty GameObject named `World`.
   - Attach `World` script.
   - Set `Texture Atlas Size In Blocks` to match your atlas grid (example: `4` for a 4x4 atlas).
   - Assign `Chunk Material` with `M_ChunkAtlas`.
   - Assign `Player` (the Player transform).
   - Assign `Sun Light` (directional light in scene).

2. Create an empty GameObject named `ChunkLoader`.
   - Attach `ChunkLoader` script.
   - Assign `World` reference.
   - Assign `Player` transform.

3. Create a `Player` GameObject.
   - Add `CharacterController`.
   - Add `PlayerController`, `Inventory`, and `BlockInteraction`.
   - Make Camera a child of Player at local position `(0, 1.6, 0)`.
   - Assign camera in both `PlayerController` and `BlockInteraction`.
   - Assign `World` and `Inventory` references in `BlockInteraction`.

4. Add a `Directional Light` named `Sun`.
   - Assign it in the `World` script.

5. Tag and Layer setup (optional but recommended).
   - Keep chunks on default layer or a dedicated `VoxelWorld` layer.

## Block Definitions in World Inspector

Set `Block Definitions` size to `7` and add:

- Air: `isSolid = false`
- Grass: `Top=0`, `Side=1`, `Bottom=2`
- Dirt: `Top=2`, `Side=2`, `Bottom=2`
- Stone: `Top=3`, `Side=3`, `Bottom=3`
- Sand: `Top=4`, `Side=4`, `Bottom=4`
- Wood: `Top=6`, `Side=5`, `Bottom=6`
- Leaves: `Top=7`, `Side=7`, `Bottom=7`, `isSolid = true`

(Indices assume your texture atlas ordering from left-to-right, bottom-to-top.)

## Texture & Material Requirements

- Texture atlas should contain all block textures in one image.
- Import settings for atlas:
  - Filter Mode: `Point (no filter)`
  - Compression: `None` (for crisp voxels)
  - Wrap: `Repeat`
- Material:
  - Shader: `Universal Render Pipeline/Lit` or `Standard`
  - Assign atlas texture to BaseMap/Albedo.

## Controls

- Move: `WASD`
- Look: Mouse
- Jump: `Space`
- Break block: `Left Click`
- Place block: `Right Click`
- Hotbar switch: Mouse wheel or `1-9`

## Save System

Modified chunks are auto-saved to `Application.persistentDataPath/VoxelSaves`.
Loading occurs automatically on startup.

