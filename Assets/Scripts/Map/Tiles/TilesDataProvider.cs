using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Ugly.MapGenerators.BinarySpacePartitioning;
using System.IO;
using System.Linq;
using UnityEngine.Assertions;

public interface ITilesDataProvider
{
    TileBase GetTile(Vector3Int position);
    Vector3 GetMapSize(int layer);
    int GetTilesCount();

    void ForEach(Action<Vector3Int, TileBase> action);
}

public abstract class TilesDataProvider : ScriptableObject, ITilesDataProvider
{
    public abstract void Init();

    public abstract void ForEach(Action<Vector3Int, TileBase> action);

    public abstract Vector3 GetMapSize(int layer);

    public abstract TileBase GetTile(Vector3Int position);

    public abstract int GetTilesCount();
}

[CreateAssetMenu(fileName = "TilesDataProvider_BSP", menuName ="", order = 0)]
public class TilesDataProvider_BSP : TilesDataProvider
{
    public DataBSP dataBSP;

    public TileBase wallTile, hallTile, roomTile, noneTile;

    private BSP BSP = null;

    public override void Init()
    {
        BSP = new BSP(dataBSP);
        BSP.CreateLeaves();
    }

    public enum TileType { Hall, Room, Wall, None }

    public TileType GetTileType(Vector3Int position)
    {
        if (IsRoom(position))
            return TileType.Room;

        if (IsHall(position))
            return TileType.Hall;

        for(int x = -1; x < 2; ++x)
        {
            for (int y = -1; y < 2; ++y)
            {
                Vector3Int neighbourPos = new Vector3Int(position.x + x, position.y + y, 0);
                if (IsRoom(neighbourPos) || IsHall(neighbourPos))
                    return TileType.Wall;
            }
        }

        return TileType.None;
    }

    private bool IsRoom(Vector3Int position)
    {
        return BSP.GetRoomId(position.x, position.y) != -1;
    }

    private bool IsHall(Vector3Int position)
    {
        return BSP.allHalls.Find(x => x.Contains(position.x, position.y)) != null;
    }

    public override void ForEach(Action<Vector3Int, TileBase> action)
    {
        for (int x = 0; x < BSP.DataBSP.mapWidth; x++)
        {
            for (int y = 0; y < BSP.DataBSP.mapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                action.Invoke(position, GetTile(position));
            }
        }
    }

    public override Vector3 GetMapSize(int layer)
    {
        return new Vector3Int(BSP.DataBSP.mapWidth, BSP.DataBSP.mapHeight, 0);
    }

    public override TileBase GetTile(Vector3Int position)
    {
        switch(GetTileType(position))
        {
            case TileType.Hall: return hallTile;
            case TileType.Wall: return wallTile;
            case TileType.Room: return roomTile;
            case TileType.None: return noneTile;
            default: throw new NotImplementedException();
        }
    }

    public override int GetTilesCount()
    {
        return BSP.DataBSP.mapWidth * BSP.DataBSP.mapHeight;
    }
}
