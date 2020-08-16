using System.Collections;
using System.Collections.Generic;
using Ugly.MapGenerators.BinarySpacePartitioning;
using UnityEngine;

public class LocationController_BSP : MonoBehaviour
{
    [SerializeField] TilesDataProvider_BSP tilesDataProvider;

    [SerializeField] TileMapController tileMapController;
    [SerializeField] LocationDecorator decorator;

    private void Awake()
    {
        tilesDataProvider.Init();


        tileMapController.SetupTileMap(tilesDataProvider);
        decorator.PlaceDecorations(tilesDataProvider);
    }

}
