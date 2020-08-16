using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Ugly.MapGenerators.BinarySpacePartitioning;
using UnityEngine;

public class Map : MonoBehaviour
{
    public CellsData cellsData;
    public float cellSize;
    [Space]
    public int height = 100;
    public int width = 100, maxLeafSize = 25, minLeafSize = 5, maxRoomSize = 25, minRoomSize = 5, maxHallsWidht = 1;
    [Space, SerializeField]
    private bool generateOnAwake = true;
    [SerializeField]
    private bool clearConsoleOnGenerate = true;

    private Cell[] cells = null;

    public event Action<Map> OnCellsUpdated;

    [NonSerialized]
    public BSP bsp;

    private int[] roomsCellDataIds;

    private void Awake()
    {
        if(generateOnAwake)
            Generate();
    }

    [EasyButtons.Button("Generate")]
    public void Generate()
    {
        if (!CheckEditor())
            return;
        Clear();
        UpdateMapData();
    }

    [EasyButtons.Button("Save")]
    public void Save()
    {
        if (!CheckEditor())
            return;
        cellsData.savedBSPData = BSP_Serializer.Serialize(bsp);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(cellsData);
#endif
    }

    [EasyButtons.Button("Load")]
    public void Load()
    {
        if (!CheckEditor())
            return;
        Clear();
        bsp = BSP_Serializer.Deserialize(cellsData.savedBSPData);
        GetDataValues(bsp.DataBSP);
        UpdatePosition();
        UpdateCells();
    }

    private bool CheckEditor()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Generation only works in play mode!");
            return false;
        }
        if (clearConsoleOnGenerate)
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
        return true;
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
        UpdatePosition();

        var data = new DataBSP(minLeafSize, maxLeafSize, minRoomSize, maxRoomSize, width, height, maxHallsWidht);

        GetDataValues(data);

        bsp = new BSP(data);
        bsp.CreateLeaves();

        UpdateCells();
    }

    private void GetDataValues(DataBSP data)
    {
        height = data.mapHeight;
        width = data.mapWidth;
        maxLeafSize = data.maxLeafSize;
        minLeafSize = data.minLeafSize;
        minRoomSize = data.minRoomSize;
        maxRoomSize = data.maxRoomSize;
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3((-cellSize * width + cellSize) / 2f, (-cellSize * height + cellSize) / 2f, 0);
    }

    private void UpdateCells()
    {
        int dataItemsCount = cellsData.dataItems.Length;
        roomsCellDataIds = new int[bsp.leavesWithRooms.Count];
        for (int i = 0; i < roomsCellDataIds.Length; i++)
        {
            roomsCellDataIds[i] = i % dataItemsCount;
        }

        cells = new Cell[height * width];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[ToIdFromXY(x, y)] = CreateCell(x, y);
            }
        }

        OnCellsUpdated?.Invoke(this);
    }

    private Cell CreateCell(int x, int y)
    {
        Cell cell = new GameObject($"Cell_{x}_{y}_{ToIdFromXY(x, y)}", typeof(Cell), typeof(SpriteRenderer)).GetComponent<Cell>();
        Transform cellTrans = cell.transform;

        cellTrans.SetParent(transform);
        cellTrans.localPosition = new Vector3(x * cellSize, y * cellSize, 0);

        cell.map = this;
        cell.Init(x + y * width, x, y);

        int leafId = bsp.GetLeafId(x, y);
        int cellDataId = roomsCellDataIds[leafId];

        cellsData.dataItems[cellDataId].SetupCell(cell, cellSize);

        bool isHall = false;
        var halls = bsp.allHalls;
        for (int i = 0; i < halls.Count; i++)
        {
            if (halls[i].Contains(x, y))
            {
                cell.spriteRenderer.color = Color.black;
                isHall = true;
                break;
            }
        }

        if (bsp.GetRoomId(x, y) != -1)
        {
            if (isHall)
            {
                cellsData.dataItems[cellDataId].SetupCell(cell, cellSize);
            }
            Color newColor = Color.red;
            newColor.a /= 4f;
            cell.spriteRenderer.color = newColor;
        }
        else
        {
            if (!isHall)
            {
                var parent = bsp.leavesWithRooms[leafId].parent;
                if (parent.leftChild.room.x == -1 || parent.rightChild.room.x == -1)
                {
                    cell.spriteRenderer.color = Color.grey;
                }
            }
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

    public int GetCellCount()
    {
        return cells == null ? 0 : cells.Length;
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
