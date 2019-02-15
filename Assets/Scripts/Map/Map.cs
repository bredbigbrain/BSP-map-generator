using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Ugly.MapGenerators.BSP;
using UnityEngine;

public class Map : MonoBehaviour
{
    public CellsData cellsData;
    public float cellSize;
    [Space]
    public int height = 100;
    public int width = 100, maxLeafSize = 25, minLeafSize = 5, maxRoomSize = 25, minRoomSize = 5;

    [SerializeField] private bool clearConsoleOnGenerate = true;

    private Cell[] cells = null;

    private LeafsHandler leafsHandler;

    private int[] roomsCellDataIds;

    private void Awake()
    {
        Generate();
    }

    [EasyButtons.Button("Generate")]
    public void Generate()
    {
#if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Generation only works in play mode!");
            return;
        }
        if(clearConsoleOnGenerate)
        {
            try
            {
                var assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);
            }
            catch
            {
                Debug.LogError("Cannot clear console in this versrion of Unity");
                clearConsoleOnGenerate = false;
            }
        }
#endif
        Clear();
        UpdateMapData();

        cells = new Cell[height * width];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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

    private Cell CreateCell(int x, int y)
    {
        Cell cell = new GameObject($"Cell_{x}_{y}_{ToIdFromXY(x, y)}", typeof(Cell), typeof(SpriteRenderer)).GetComponent<Cell>();
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
        var parent = leafsHandler.leaves[leafId].parent;
        if (parent.leftChild.room.x == -1 || parent.rightChild.room.x == -1)
        {
            cell.spriteRenderer.color = Color.grey;
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
