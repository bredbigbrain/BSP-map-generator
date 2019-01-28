using System.Collections;
using System.Collections.Generic;
using Ugly.MapGenerators.BSP;
using UnityEngine;

public class Map : MonoBehaviour
{
    public CellsData cellsData;
    public float cellSize;
    [Space]
    public int height = 100;
    public int width = 100, maxLeafSize = 25, minLeafSize = 5, maxRoomSize = 25, minRoomSize = 5;

    private Cell[] cells = null;

    private LeafsHandler leafsHandler;

    private int[] roomsCellDataIds;

    private void Awake()
    {
        /*
        List<Vector2> positions = new List<Vector2>();
        var leaves = leafsHandler.leaves;
        for (int i = 0; i < leaves.Count; i++)
        {
            FillLeaf(leaves[i], i);
        }

        void FillLeaf(Leaf leaf, int z)
        {
            for (int y = leaf.y; y < leaf.y + leaf.height; y++)
            {
                for (int x = leaf.x; x < leaf.x + leaf.width; x++)
                {
                    var cell = CreateCell(x, y).transform;
                    Vector2 pos = cell.position;
                    if(positions.Contains(pos))
                    {
                        print("Duplicate: " + pos);
                    }
                    else
                    {
                        positions.Add(pos);
                    }
                    cell.position += Vector3.forward * z * 0.05f;
                }
            }
        }*/
        Generate();
    }

    public void UpdateMapData()
    {
        transform.position = new Vector3((-cellSize * width + cellSize) / 2f, (-cellSize * height + cellSize) / 2f, 0);

        DataBSP data = new DataBSP(minLeafSize, maxLeafSize, minRoomSize, maxRoomSize, width, height);

        height = data.mapHeigh;
        width = data.mapWidth;
        maxLeafSize = data.maxLeafSize;
        minLeafSize = data.minLeafSize;
        minRoomSize = data.minRoomSize;
        maxRoomSize = data.maxRoomSize;

        leafsHandler = new LeafsHandler(data);
        leafsHandler.CreateLeaves();

        int dataItemsCount = cellsData.dataItems.Length;

        roomsCellDataIds = new int[leafsHandler.leaves.Count];
        for (int i = 0; i < roomsCellDataIds.Length; i++)
        {
            roomsCellDataIds[i] = i % dataItemsCount;
        }
    }

    [EasyButtons.Button("Generate")]
    public void Generate()
    {
#if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Generation only in play mode!");
            return;
        }
#endif
        Clear();
        UpdateMapData();

        cells = new Cell[height * width];

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                cells[ToIdFromXY(x, y)] = CreateCell(x, y);
            }
        }
    }

    public void Clear()
    {
        if (cells != null)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Destroy(cells[i].gameObject);
            }
        }
    }

    private Cell CreateCell(int x, int y)
    {
        Cell cell = new GameObject($"Cell {x} {y} {ToIdFromXY(x, y)}", typeof(Cell), typeof(SpriteRenderer)).GetComponent<Cell>();
        Transform cellTrans = cell.transform;

        cellTrans.SetParent(transform);
        cellTrans.localPosition = new Vector3(x * cellSize, y * cellSize, 0);

        cell.map = this;
        cell.Init(x + y * width, x, y);

        int leafId = leafsHandler.GetLeafId(x, y);
        int cellDataId = roomsCellDataIds[leafId];

        cellsData.dataItems[cellDataId].SetupCell(cell, cellSize);

        bool isHall = false;
        var halls = leafsHandler.leaves[leafId].parent.halls;
        for (int i = 0; i < halls.Count; i++)
        {
            if (halls[i].Contains(x, y))
            {
                cell.spriteRenderer.color = Color.black;
                isHall = true;
                break;
            }
        }

        if (!isHall && leafsHandler.GetRoomId(x, y) != -1)
        {
            Color newColor = cell.spriteRenderer.color;
            newColor.a /= 4f;
            cell.spriteRenderer.color = newColor;
        }

        return cell;
    }

    public Cell GetCell(int id)
    {
        return cells[id];
    }

    public Cell GetCell(int x, int y)
    {
        return cells[x + y * width];
    }

    public void FromIdToXY(int id, out int x, out int y)
    {
        x = id % width;
        y = id / height;
    }

    public int ToIdFromXY(int x, int y)
    {
        return x + y * width;
    }
}
