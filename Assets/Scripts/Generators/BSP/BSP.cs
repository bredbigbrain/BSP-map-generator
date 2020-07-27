using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ugly.MapGenerators.BinarySpacePartitioning
{
    [System.Serializable]
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
        /// <param name="hallsWidht">minimum 1</param>
        public DataBSP(int minLeafSize, int maxLeafSize, int minRoomSize, int maxRoomSize, int mapWidth, int mapHeigh, int hallsWidht)
        {
            this.minLeafSize = minLeafSize < 5 ? 5 : minLeafSize;
            this.maxLeafSize = maxLeafSize < minLeafSize ? minLeafSize : maxLeafSize;

            this.minRoomSize = minRoomSize < 3 ? 3 : minRoomSize;
            this.maxRoomSize = maxRoomSize < minRoomSize ? minRoomSize : maxRoomSize;

            this.mapWidth = mapWidth < minLeafSize * 2 ? minLeafSize * 2 : mapWidth;
            this.mapHeigh = mapHeigh < minLeafSize * 2 ? minLeafSize * 2 : mapHeigh;

            this.hallsWidht = hallsWidht > 0 ? hallsWidht : 1;
        }

        public int mapWidth;
        public int mapHeigh;
        public int minLeafSize;
        public int maxLeafSize;
        public int minRoomSize;
        public int maxRoomSize;
        public int hallsWidht;
    }

    public class BSP
    {
        [HideInInspector, SerializeField]
        private DataBSP dataBSP;

        public Leaf root = null;
        public List<Leaf> allLeaves = new List<Leaf>();
        public List<Leaf> leavesWithRooms = new List<Leaf>();
        public List<Hall> allHalls = new List<Hall>();

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

        public BSP(DataBSP dataBSP)
        {
            DataBSP = dataBSP;
        }

        public void CreateLeaves()
        {
            leavesWithRooms.Clear();

            root = new Leaf(0, 0, dataBSP.mapWidth, dataBSP.mapHeigh, dataBSP.minLeafSize, this);
            allLeaves = new List<Leaf>{ root };

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
                            leavesWithRooms.Add(_leaf);
                        }
                    }
                    else
                    {
                        _leaf.CreateRoom(dataBSP.minRoomSize, dataBSP.maxRoomSize);
                        leavesWithRooms.Add(_leaf);
                    }
                }
            }

            allHalls.Clear();
            for (int i = 0; i < allLeaves.Count; i++)
            {
                allLeaves[i].ConnectChildren(dataBSP.hallsWidht);
            }
        }

        public Leaf GetLeaf(int x, int y)
        {
            int id = GetLeafId(x, y);
            if(id != -1)
            {
                return leavesWithRooms[id];
            }
            return null;
        }

        public int GetLeafId(int x, int y)
        {
            if (leavesWithRooms != null)
            {
                for (int i = 0; i < leavesWithRooms.Count; i++)
                {
                    if (leavesWithRooms[i].IsPointInside(x, y))
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
                return leavesWithRooms[id];
            }
            return null;
        }

        public int GetRoomId(int x, int y)
        {
            if (leavesWithRooms != null)
            {
                for (int i = 0; i < leavesWithRooms.Count; i++)
                {
                    if (leavesWithRooms[i].IsPointInsideRoom(x, y))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void AddHall(Hall hall)
        {
            if (!allHalls.Contains(hall))
            {
                List<Leaf> connectedLeaves = new List<Leaf>();

                foreach (var otherHall in allHalls)
                {
                    if(hall.Connects(otherHall))
                    {
                        foreach(var otherLeaf in otherHall.leaves)
                        {
                            if(!connectedLeaves.Contains(otherLeaf))
                                connectedLeaves.Add(otherLeaf);
                        }
                    }
                }

                allHalls.Add(hall);

                Leaf leaf;
                int leavesCount = leavesWithRooms.Count;
                for (int i = 0; i < leavesCount; i++)
                {
                    leaf = leavesWithRooms[i];
                    if (!connectedLeaves.Contains(leaf) && hall.Connects(leaf))
                    {
                        leaf.connections.AddRange(connectedLeaves);
                        if(!leaf.halls.Contains(hall))
                        {
                            leaf.halls.Add(hall);
                        }
                        for (int j = 0; j < connectedLeaves.Count; j++)
                        {
                            connectedLeaves[j].connections.Add(leaf);
                            if (!connectedLeaves[j].halls.Contains(hall))
                            {
                                connectedLeaves[j].halls.Add(hall);
                            }
                        }
                        connectedLeaves.Add(leaf);
                    }
                }

                hall.leaves.AddRange(connectedLeaves);
            }
        }
    }
}
