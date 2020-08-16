using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    public Tilemap tileMap;

    [SerializeField] private TilesDataProvider tilesProvider;

    public void SetupTileMap(TilesDataProvider tilesProvider)
    {
        this.tilesProvider = tilesProvider;

        tileMap.ClearAllTiles();

        tilesProvider.ForEach((Vector3Int p, TileBase t) => tileMap.SetTile(p, t));
    }

    [EasyButtons.Button("SetupTileMap")]
    public void Test()
    {
        tilesProvider.Init();
        SetupTileMap(tilesProvider);
    }


}
