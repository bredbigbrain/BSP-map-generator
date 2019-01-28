using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ugly.MapGenerators.BSP
{
    public struct DataBSP
    {
        /// <summary>
        /// Map generation parameters
        /// </summary>
        /// <param name="minLeafSize">minimum 5</param>
        /// <param name="maxLeafSize">>= minLeafSize</param>
        /// <param name="minRoomSize">minimum 3</param>
        /// <param name="maxRoomSize">>= minRoomSize</param>
        /// <param name="mapWidth">minimum 2 * minLeafSize</param>
        /// <param name="mapHeigh">minimum 2 * minLeafSize</param>
        public DataBSP(int minLeafSize, int maxLeafSize, int minRoomSize, int maxRoomSize, int mapWidth, int mapHeigh)
        {
            this.minLeafSize = minLeafSize < 5 ? 5 : minLeafSize;
            this.maxLeafSize = maxLeafSize < minLeafSize ? minLeafSize : maxLeafSize;

            this.minRoomSize = minRoomSize < 3 ? 3 : minRoomSize;
            this.maxRoomSize = maxRoomSize < minRoomSize ? minRoomSize : maxRoomSize;

            this.mapWidth = mapWidth < minLeafSize * 2 ? minLeafSize * 2 : mapWidth;
            this.mapHeigh = mapHeigh < minLeafSize * 2 ? minLeafSize * 2 : mapHeigh;
        }

        public readonly int mapWidth;
        public readonly int mapHeigh;
        public readonly int minLeafSize;
        public readonly int maxLeafSize;
        public readonly int minRoomSize;
        public readonly int maxRoomSize;
    }

    public class LeafsHandler
    {
        private DataBSP dataBSP;

        public List<Leaf> leaves = new List<Leaf>();

        public DataBSP DataBSP
        {
            get
            {
                return dataBSP;
            }
            private set
            {
                dataBSP = value;
            }
        }

        public LeafsHandler(DataBSP dataBSP)
        {
            DataBSP = dataBSP;
        }

        public void CreateLeaves()
        {
            leaves.Clear();
            var allLeaves = new List<Leaf>
            {
                new Leaf(0, 0, dataBSP.mapWidth, dataBSP.mapHeigh, dataBSP.minLeafSize)
            };

            Leaf _leaf;
            for (int i = 0; i < allLeaves.Count; i++)
            {
                _leaf = allLeaves[i];
                if (_leaf.leftChild == null && _leaf.rightChild == null)
                {
                    if (_leaf.width > dataBSP.maxLeafSize || _leaf.height > dataBSP.maxLeafSize)
                    {
                        if (_leaf.Split())
                        {
                            allLeaves.Add(_leaf.leftChild);
                            allLeaves.Add(_leaf.rightChild);
                        }
                        else
                        {
                            _leaf.CreateRoom(dataBSP.minRoomSize, dataBSP.maxRoomSize);
                            leaves.Add(_leaf);
                        }
                    }
                    else
                    {
                        _leaf.CreateRoom(dataBSP.minRoomSize, dataBSP.maxRoomSize);
                        leaves.Add(_leaf);
                    }
                }
            }

            for (int i = 0; i < leaves.Count; i++)
            {
                leaves[i]?.parent.ConnectChildren();
            }
        }

        public Leaf GetLeaf(int x, int y)
        {
            int id = GetLeafId(x, y);
            if(id != -1)
            {
                return leaves[id];
            }
            return null;
        }

        public int GetLeafId(int x, int y)
        {
            if (leaves != null)
            {
                for (int i = 0; i < leaves.Count; i++)
                {
                    if (leaves[i].IsPointInsize(x, y))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public Leaf GetRoom(int x, int y)
        {
            int id = GetRoomId(x, y);
            if (id != -1)
            {
                return leaves[id];
            }
            return null;
        }

        public int GetRoomId(int x, int y)
        {
            if (leaves != null)
            {
                for (int i = 0; i < leaves.Count; i++)
                {
                    if (leaves[i].IsPointInsideRoom(x, y))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
