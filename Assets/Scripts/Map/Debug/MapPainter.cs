using System.Collections;
using System.Collections.Generic;
using Ugly.MapGenerators.BinarySpacePartitioning;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Map))]
public class MapPainter : MonoBehaviour
{
    public ConnectionsDrawer connectionsDrawer;
    
    [EasyButtons.Button("Draw Connections")]
    public bool DrawConnections()
    {
        return connectionsDrawer.Draw(GetComponent<Map>());
    }
    
}

public class Drawer
{
    protected BSP bsp => cells.bsp;
    protected CellsWrapper cells;

    public class CellsWrapper
    {
        public CellsWrapper(Map map) { this.map = map; }

        public Map map;
        public BSP bsp => map.bsp;
        public int Length => map.GetCellCount();
        public Cell this[int index] { get => map.GetCell(index); }
    }

    public void Setup(Map map) 
    { 
        cells = new CellsWrapper(map); 
    }
}

[System.Serializable]
public class ConnectionsDrawer : Drawer
{
    public float otherAlpha = 0.5f;

    public bool Draw(Map map)
    {
        Setup(map);

        if (cells == null)
            return false;

        var leaf = bsp.leavesWithRooms.Find((x) => x.connections.Count > 4);
        if (leaf != null)
        {
            Cell cell;
            bool discard;
            for (int i = 0; i < cells.Length; i++)
            {
                discard = true;
                cell = cells[i];

                foreach (var hall in leaf.halls)
                {
                    if (hall.Contains(cell.x, cell.y))
                    {
                        discard = false;
                        cells[i].spriteRenderer.color = Color.black;
                        break;
                    }
                }

                if (leaf.room.Contains(cell.x, cell.y))
                {
                    discard = false;
                    cell.spriteRenderer.color = Color.red;
                }
                else
                {
                    foreach (var connecition in leaf.connections)
                    {
                        if (connecition.room.Contains(cell.x, cell.y))
                        {
                            discard = false;
                            cell.spriteRenderer.color = Color.green;
                        }
                    }
                }

                if (discard)
                {
                    var newColor = cells[i].spriteRenderer.color;
                    newColor.a = otherAlpha;
                    cells[i].spriteRenderer.color = newColor;
                }
            }
        }
        return leaf != null;
    }
}