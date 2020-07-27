using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ugly.MapGenerators.BinarySpacePartitioning
{
    public static class BSP_Serializer
    {
        [System.Serializable]
        public class Data
        {
            [System.Serializable]
            public class LeafSerialized
            {
                public int x = -1, y = -1, width = -1, height = -1, minSize = -1;
                public RectInt room = new RectInt(-1, -1, 0, 0);
                public int[] hallsIDs;
                public int leftChildID = -1; //Left or lower.
                public int rightChildID = -1; //Right or higher.
                public int parentID = -1; //Connects child's rooms, if any.
                public int[] connections;

                public LeafSerialized(Leaf leaf, BSP bsp)
                {
                    x = leaf.x;
                    y = leaf.y;
                    width = leaf.width;
                    height = leaf.height;
                    minSize = leaf.minSize;
                    room = leaf.room;

                    leftChildID = bsp.allLeaves.IndexOf(leaf.leftChild);
                    rightChildID = bsp.allLeaves.IndexOf(leaf.rightChild);
                    parentID = bsp.allLeaves.IndexOf(leaf.parent);

                    hallsIDs = new int[leaf.halls.Count];
                    hallsIDs.ForEach((int i, ref int ID) => { ID = bsp.allHalls.IndexOf(leaf.halls[i]); });

                    connections = new int[leaf.connections.Count];
                    connections.ForEach((int i, ref int ID) => { ID = bsp.allLeaves.IndexOf(leaf.connections[i]); });
                }
            }

            [System.Serializable]
            public class HallSerialized
            {
                public HallSerialized(Hall hall, BSP bsp)
                {
                    rects = (RectInt[])hall.rects.Clone();
                    leavesIds = new int[hall.leaves.Count];
                    leavesIds.ForEach((int i, ref int ID) => { ID = bsp.leavesWithRooms.IndexOf(hall.leaves[i]); });
                }

                public RectInt[] rects;
                public int[] leavesIds;
            }

            public DataBSP dataBSP;

            public int rootID = -1;
            public int[] leavesWithRoomsIds;
            public LeafSerialized[] allLeaves;
            public HallSerialized[] allHalls;
        }

        public static Data Serialize(BSP bsp)
        {
            var data = new Data()
            {
                allHalls = new Data.HallSerialized[bsp.allHalls.Count],
                allLeaves = new Data.LeafSerialized[bsp.allLeaves.Count],
                leavesWithRoomsIds = new int[bsp.leavesWithRooms.Count]
            };

            data.allHalls.ForEach((int i, ref Data.HallSerialized hall) => { hall = new Data.HallSerialized(bsp.allHalls[i], bsp); });
            data.allLeaves.ForEach((int i, ref Data.LeafSerialized leaf) => { leaf = new Data.LeafSerialized(bsp.allLeaves[i], bsp); });
            data.leavesWithRoomsIds.ForEach((int i, ref int ID) => { ID = bsp.allLeaves.IndexOf(bsp.leavesWithRooms[i]); });

            data.dataBSP = bsp.DataBSP;
            data.rootID = bsp.allLeaves.IndexOf(bsp.root);

            return data;
        }

        public static BSP Deserialize(Data data)
        {
            var bsp = new BSP(data.dataBSP);

            foreach (var serLeaf in data.allLeaves)
                bsp.allLeaves.Add(new Leaf(serLeaf.x, serLeaf.y, serLeaf.width, serLeaf.height, serLeaf.minSize, bsp));
            foreach (int index in data.leavesWithRoomsIds)
                bsp.leavesWithRooms.Add(bsp.allLeaves[index]);

            foreach (var hallSer in data.allHalls)
            {
                var hall = new Hall(hallSer.rects);
                foreach (var id in hallSer.leavesIds)
                    hall.leaves.Add(bsp.leavesWithRooms[id]);

                bsp.allHalls.Add(hall);
            }

            for (int i = 0; i < data.allLeaves.Length; ++i)
            {
                var leaf = bsp.allLeaves[i];
                var leavSer = data.allLeaves[i];

                leaf.room = leavSer.room;

                if(leavSer.leftChildID > 0)
                    leaf.leftChild = bsp.allLeaves[leavSer.leftChildID];
                if(leavSer.rightChildID > 0)
                    leaf.rightChild = bsp.allLeaves[leavSer.rightChildID];
                if(leavSer.parentID > 0)
                    leaf.parent = bsp.allLeaves[leavSer.parentID];

                foreach (var id in leavSer.hallsIDs)
                    leaf.halls.Add(bsp.allHalls[id]);

                foreach (var id in leavSer.connections)
                    leaf.connections.Add(bsp.allLeaves[id]);
            }

            if(data.rootID > 0)
                bsp.root = bsp.allLeaves[data.rootID];

            return bsp;
        }
    }
}