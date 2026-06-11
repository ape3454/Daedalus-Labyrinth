using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[System.Serializable]
public class WeightedTileSet
{
    public List<string> names;
    public List<TileBase> tiles;
    public List<float> weighting;
}


public class TilemapVisualiser : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap, backgroundTilemap, tangibleTilemap;
    [SerializeField]
    private TileBase wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight, wallInnerCornerUpRight, wallInnerCornerUpLeft,
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    [SerializeField]
    private WeightedTileSet alternativeFloorTiles, alternativeWallTiles, alternativeTangibleTiles;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, alternativeFloorTiles.tiles, alternativeFloorTiles.weighting);
    }

    public void PaintAlternativeTiles(Dictionary<Vector2Int, string> alternativeTiles)
    {
        foreach (var (pos, type) in alternativeTiles)
        {
            if (alternativeWallTiles.names.Contains(type))
            {
                PaintSingleTile(wallTilemap, alternativeWallTiles.tiles[alternativeWallTiles.names.IndexOf(type)], pos);
            }
            else if (alternativeTangibleTiles.names.Contains(type))
            {
                PaintSingleTile(wallTilemap, alternativeTangibleTiles.tiles[alternativeTangibleTiles.names.IndexOf(type)], pos);
            }
            else if (alternativeFloorTiles.names.Contains(type))
            {
                PaintSingleTile(wallTilemap, alternativeFloorTiles.tiles[alternativeFloorTiles.names.IndexOf(type)], pos);
            }
        }

    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, IEnumerable<TileBase> tiles)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tiles.ElementAt(Random.Range(0, tiles.Count())), position);
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, IEnumerable<TileBase> tiles, List<float> weighting)
    {
        List<float> cumulativeWeight = new List<float>();
        for (int i = 0; i < weighting.Count; i++) cumulativeWeight.Add(cumulativeWeight.ElementAtOrDefault(i - 1) + weighting[i]);
        float randomWeight;

        foreach (var position in positions)
        {
            randomWeight = Random.Range(0, cumulativeWeight[^1]);
            PaintSingleTile(tilemap, tiles.ElementAt(cumulativeWeight.FindIndex(y => randomWeight <= y)), position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallInnerCornerUpRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerUpRight;
        }
        else if (WallTypesHelper.wallInnerCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerUpLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallTop.Contains((typeAsInt + 256)))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains((typeAsInt + 256)))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains((typeAsInt + 256)))
        {
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallBottom.Contains((typeAsInt + 256)))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }
}
