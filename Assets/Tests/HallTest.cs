using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Ugly.MapGenerators.BinarySpacePartitioning;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class HallTest
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test]
        public void ConnectSimple_SamePosition(int i)
        {
            var hallOne = new Hall(new RectInt(0, 0, 2, 2));
            var hallTwo = new Hall(new RectInt(0, 0, i, i));

            Assert.IsTrue(hallOne.Connects(hallTwo));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test]
        public void ConnectSimple_RightEdgePositions(int size)
        {
            var hallOne = new Hall(new RectInt(0, 0, 2, 2));

            for (int y = hallOne.rects[0].yMin; y < hallOne.rects[0].height; ++y)
            {
                var hallTwo = new Hall(new RectInt(2, 0, size, size));
                Assert.IsTrue(hallOne.Connects(hallTwo));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test]
        public void ConnectSimple_EdgePositions(int size)
        {
            var hallOne = new Hall(new RectInt(0, 0, size, size));

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    var hallTwo = new Hall(new RectInt(x * size, y * size, size, size));
                    if(Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                        Assert.IsFalse(hallOne.Connects(hallTwo), $"{x}:{y}");
                    else
                        Assert.IsTrue(hallOne.Connects(hallTwo), $"{x}:{y}");
                }
            }
        }
    }
}
