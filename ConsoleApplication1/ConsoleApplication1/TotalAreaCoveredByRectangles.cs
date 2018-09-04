using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace ConsoleApplication1
{
    class TotalAreaCoveredByRectangles
    {
        private const int X1 = 0;
        private const int Y1 = 1;
        private const int X2 = 2;
        private const int Y2 = 3;

        public static int Calculate(IEnumerable<int[]> rectangles)
        {
            List<int[]> rectanglesList = rectangles.OrderBy(r => r[X1]).ToList();

            return CheckRemaining(rectanglesList, 0);
        }

        private static int CheckRemaining(List<int[]> rectangles, int currentIndex)
        {
            int remainingCount = rectangles.Count - currentIndex;
            if (remainingCount == 0) return 0;
            int[] rectangle1 = rectangles[currentIndex];
            int rectangle1Area = (rectangle1[X2] - rectangle1[X1]) * (rectangle1[Y2] - rectangle1[Y1]);
            if (remainingCount == 1) return rectangle1Area;
            int nextIndex = currentIndex + 1;
            List<int[]> intersectionRectangles = new List<int[]>();
            foreach (var rectangle2 in rectangles.Skip(nextIndex))
            {
                if (rectangle2[X1] >= rectangle1[X2]) break;
                int y1, y2;
                if (IntersectionCordinates(rectangle1[Y1], rectangle1[Y2], rectangle2[Y1], rectangle2[Y2], out y1, out y2) > 0)
                {
                    intersectionRectangles.Add(new[] { rectangle2[X1], y1, rectangle2[X2] <= rectangle1[X2] ? rectangle2[X2] : rectangle1[X2], y2 });
                }
            }
            int netArea = rectangle1Area - CheckRemaining(intersectionRectangles, 0);
            netArea += CheckRemaining(rectangles, nextIndex);
            return netArea;
        }

        private static int IntersectionCordinates(int bottom1, int top1, int bottom2, int top2, out int bottom, out int top)
        {
            if (bottom2 >= bottom1)
            {
                if (bottom2 >= top1)
                {
                    bottom = 0;
                    top = 0;
                    return 0;
                }
                bottom = bottom2;
                top = top2 <= top1 ? top2 : top1;
            }
            else
            {
                if (top2 <= bottom1)
                {
                    bottom = 0;
                    top = 0;
                    return 0;
                }
                bottom = bottom1;
                top = top1 <= top2 ? top1 : top2;
            }
            return top - bottom;
        }
    }

    class TotalAreaCoveredByRectangles3
    {
        private const int X1 = 0;
        private const int Y1 = 1;
        private const int X2 = 2;
        private const int Y2 = 3;

        public static int Calculate(IEnumerable<int[]> rectangles)
        {
            List<int[]> rectanglesList = rectangles.OrderBy(r => r[X1]).ToList();

            return CheckRemaining(rectanglesList, 0);
        }

        private static int CheckRemaining(List<int[]> rectangles, int currentIndex)
        {
            int remainingCount = rectangles.Count - currentIndex;
            if (remainingCount == 0) return 0;
            int[] rectangle1 = rectangles[currentIndex];
            int rectangle1Area = (rectangle1[X2] - rectangle1[X1]) * (rectangle1[Y2] - rectangle1[Y1]);
            if (remainingCount == 1) return rectangle1Area;
            int nextIndex = currentIndex + 1;
            List<int[]> intersectionRectangles = new List<int[]>();
            foreach (var rectangle2 in rectangles.Skip(nextIndex))
            {
                if (rectangle2[X1] >= rectangle1[X2]) break;
                int y1, y2;
                if (IntersectionCordinates(rectangle1[Y1], rectangle1[Y2], rectangle2[Y1], rectangle2[Y2], out y1, out y2) > 0)
                {
                    intersectionRectangles.Add(new []{ rectangle2[X1], y1, rectangle2[X2] <= rectangle1[X2] ? rectangle2[X2] : rectangle1[X2], y2});
                }
            }
            int netArea = rectangle1Area - CheckRemaining( intersectionRectangles, 0);
            netArea += CheckRemaining(rectangles, nextIndex);
            return netArea;
        }

        private static int IntersectionCordinates(int bottom1, int top1, int bottom2, int top2, out int bottom, out int top)
        {
            if (bottom2 >= bottom1)
            {
                if (bottom2 >= top1)
                {
                    bottom = 0;
                    top = 0;
                    return 0;
                }
                bottom = bottom2;
                top = top2 <= top1 ? top2 : top1;
            }
            else
            {
                if (top2 <= bottom1)
                {
                    bottom = 0;
                    top = 0;
                    return 0;
                }
                bottom = bottom1;
                top = top1 <= top2 ? top1 : top2;
            }
            return top - bottom;
        }
    }

    class TotalAreaCoveredByRectangles2
    {
        private const int X1 = 0;
        private const int Y1 = 1;
        private const int X2 = 2;
        private const int Y2 = 3;

        public static int Calculate(IEnumerable<int[]> rectangles)
        {
            List<int[]> rectanglesList = rectangles.OrderBy(r => r[X1]).ToList();

            return CheckRemaining(rectanglesList, 0);
        }

        private static int CheckRemaining(List<int[]> rectangles, int currentIndex)
        {
            if (currentIndex >= rectangles.Count) return 0;

            int[] rectangle1 = rectangles[currentIndex];
            int nextIndex = currentIndex + 1;
            int intersectionArea = 0;
            foreach (var rectangle2 in rectangles.Skip(nextIndex))
            {
                if (rectangle2[X1] >= rectangle1[X2]) break;
                intersectionArea += ((rectangle2[X2] <= rectangle1[X2] ? rectangle2[X2] : rectangle1[X2]) - rectangle2[X1]) * IntersectionLength(rectangle1[Y1], rectangle1[Y2], rectangle2[Y1], rectangle2[Y2]);
            }
            int netArea = (rectangle1[X2] - rectangle1[X1]) * (rectangle1[Y2] - rectangle1[Y1]) - intersectionArea;
            netArea += CheckRemaining(rectangles, nextIndex);
            return netArea;
        }

        private static int IntersectionLength(int bottom1, int top1, int bottom2, int top2)
        {
            int bottom, top;
            if (bottom2 >= bottom1)
            {
                if (bottom2 >= top1) return 0;
                bottom = bottom2;
                top = top2 <= top1 ? top2 : top1;
            }
            else
            {
                if (top2 <= bottom1) return 0;
                bottom = bottom1;
                top = top1 <= top2 ? top1 : top2;
            }
            return top - bottom;
        }
    }

    class TotalAreaCoveredByRectangles1
    {
        public static int Calculate(IEnumerable<int[]> rectangles)
        {
            List<int[]> rectanglesList = rectangles.ToList();

            return CheckRemaining(rectanglesList, 0);
        }

        private static int CheckRemaining(List<int[]> rectangles, int currentIndex)
        {
            if (currentIndex >= rectangles.Count) return 0;

            int[] c = rectangles[currentIndex++];
            int intersectionArea = 0;
            foreach (var r in rectangles.Skip(currentIndex))
                intersectionArea += IntersectionLength(c[0], c[2], r[0], r[2]) * IntersectionLength(c[1], c[3], r[1], r[3]);
            int netArea = (c[2] - c[0]) * (c[3] - c[1]) - intersectionArea;
            netArea += CheckRemaining(rectangles, currentIndex);
            return netArea;
        }

        private static int IntersectionLength(int a1, int a2, int b1, int b2)
        {
            int x0, x1;
            if (b1 >= a1)
            {
                if (b1 >= a2) return 0;
                x0 = b1;
                x1 = b2 <= a2 ? b2 : a2;
            }
            else
            {
                if (b2 <= a1) return 0;
                x0 = a1;
                x1 = a2 <= b2 ? a2 : b2;
            }
            return x1 - x0;
        }


        private const int X0 = 0;
        private const int Y0 = 1;
        private const int X1 = 2;
        private const int Y1 = 3;

        private static int Intersection1(int[] rectangle1, int[] rectangle2)
        {
            int x0, y0, x1, y1;
            if (rectangle2[X0] >= rectangle1[X0])
            {
                if (rectangle2[X0] >= rectangle1[X1]) return 0;
                x0 = rectangle2[X0];
                x1 = rectangle2[X1] <= rectangle1[X1] ? rectangle2[X1] : rectangle1[X1];
            }
            else
            {
                if (rectangle2[X1] <= rectangle1[X0]) return 0;
                x0 = rectangle1[X0];
                x1 = rectangle1[X1] <= rectangle2[X1] ? rectangle1[X1] : rectangle2[X1];
            }
            if (rectangle2[Y0] >= rectangle1[Y0])
            {
                if (rectangle2[Y0] >= rectangle1[Y1]) return 0;
                y0 = rectangle2[Y0];
                y1 = rectangle2[Y1] <= rectangle1[Y1] ? rectangle2[Y1] : rectangle1[Y1];
            }
            else
            {
                if (rectangle2[Y1] <= rectangle1[Y0]) return 0;
                y0 = rectangle1[Y0];
                y1 = rectangle1[Y1] <= rectangle2[Y1] ? rectangle1[Y1] : rectangle2[Y1];
            }

            return (x1 - x0) * (y1 - y0);
        }

        public static int Calculate3(IEnumerable<int[]> rectangles)
        {
            int x0 = 0;
            int y0 = 1;
            int x1 = 2;
            int y1 = 3;
            Dictionary<int, List<int[]>> squares = new Dictionary<int, List<int[]>>();
            foreach (int[] rectangle in rectangles.OrderBy(r => r[y0]))
            {
                for (int x = rectangle[x0]; x < rectangle[x1]; x++)
                {
                    List<int[]> yPatches;
                    squares.TryGetValue(x, out yPatches);
                    if (yPatches == null)
                    {
                        yPatches = new List<int[]>(new[] {new[] {0, 0}});
                        squares.Add(x, yPatches);
                    }

                    int[] lastPatch = yPatches[yPatches.Count-1];
                    if (rectangle[y1] <= lastPatch[1]) continue;
                    if (rectangle[y0] <= lastPatch[1])
                    {
                        lastPatch[1] = rectangle[y1];
                    }
                    else
                    {
                        yPatches.Add(new[] {rectangle[y0], rectangle[y1]});
                    }
                }
            }

            int area = 0;

            foreach (List<int[]> yPatches in squares.Values)
            {
                foreach (int[] yPatch in yPatches)
                {
                    area += yPatch[1] - yPatch[0];
                }
            }

            return area;
        }


        public static int Calculate2(IEnumerable<int[]> rectangles)
        {
            SortedDictionary<int, LinkedList<int[]>> squares = new SortedDictionary<int, LinkedList<int[]>>();
            foreach (int[] rectangle in rectangles)
            {
                for (int x = rectangle[0]; x < rectangle[2]; x++)
                {
                    LinkedList<int[]> yPatches;
                    squares.TryGetValue(x, out yPatches);
                    if (yPatches == null)
                    {
                        yPatches = new LinkedList<int[]>();
                        squares.Add(x, yPatches);
                    }

                    yPatches.AddFirst(new LinkedListNode<int[]>(new[] {rectangle[1], rectangle[3]}));

                    LinkedListNode<int[]> currentNode = yPatches.First;
                    while (currentNode != null)
                    {
                        LinkedListNode<int[]> nextNode = currentNode.Next;
                        if (nextNode == null)
                        {
                            break;
                        }
                        if (currentNode.Value[1] < nextNode.Value[0])
                        {
                            break;
                        }
                        if (currentNode.Value[1] < nextNode.Value[1])
                        {
                            if (currentNode.Value[0] < nextNode.Value[0])
                            {
                                currentNode.Value[1] = nextNode.Value[1];
                                yPatches.Remove(nextNode);
                            }
                            else
                            {
                                yPatches.Remove(currentNode);
                            }
                            break;
                        }
                        if (currentNode.Value[0] < nextNode.Value[0])
                        {
                            yPatches.Remove(nextNode);
                        }
                        else if (currentNode.Value[0] < nextNode.Value[1])
                        {
                            currentNode.Value[0] = nextNode.Value[0];
                            yPatches.Remove(nextNode);
                        }
                        else
                        {
                            yPatches.AddAfter(nextNode, new LinkedListNode<int[]>(currentNode.Value));
                            yPatches.Remove(currentNode);
                            currentNode = nextNode.Next;
                        }
                    }
                }
            }

            int area = 0;

            foreach (LinkedList<int[]> yPatches in squares.Values)
            {
                foreach (int[] yPatch in yPatches)
                {
                    area += yPatch[1] - yPatch[0];
                }
            }

            return area;
        }


        public static int Calculate1(IEnumerable<int[]> rectangles)
        {
            SortedDictionary<int, SortedSet<int>> squares = new SortedDictionary<int, SortedSet<int>>();
            foreach (int[] rectangle in rectangles)
            {
                for (int x = rectangle[0]; x < rectangle[2]; x++)
                {
                    SortedSet<int> ySet;
                    squares.TryGetValue(x, out ySet);
                    if (ySet == null)
                    {
                        ySet = new SortedSet<int>();
                        squares.Add(x, ySet);
                    }

                    for (int y = rectangle[1]; y < rectangle[3]; y++)
                    {
                        ySet.Add(y);
                    }
                }
            }

            int area = 0;

            foreach (SortedSet<int> ySet in squares.Values)
            {
                area += ySet.Count;
            }

            return area;
        }
    }

    [TestFixture]
    class TotalAreaCoveredByRectanglesTests
    {
        [Test]
        public void ZeroRectangles()
        {
            AreEqual(0, TotalAreaCoveredByRectangles.Calculate(Enumerable.Empty<int[]>()));
        }

        [Test]
        public void OneRectangle()
        {
            AreEqual(1, TotalAreaCoveredByRectangles.Calculate(new[] { new[] { 0, 0, 1, 1 } }));
        }

        [Test]
        public void OneRectangleV2()
        {
            AreEqual(22, TotalAreaCoveredByRectangles.Calculate(new[] { new[] { 0, 4, 11, 6 } }));
        }

        [Test]
        public void TwoRectangles()
        {
            AreEqual(2, TotalAreaCoveredByRectangles.Calculate(new[] { new[] { 0, 0, 1, 1 }, new[] { 1, 1, 2, 2 } }));
        }

        [Test]
        public void TwoRectanglesV2()
        {
            AreEqual(4, TotalAreaCoveredByRectangles.Calculate(new[] { new[] { 0, 0, 1, 1 }, new[] { 0, 0, 2, 2 } }));
        }

        [Test]
        public void ThreeRectangles()
        {
            AreEqual(36, TotalAreaCoveredByRectangles.Calculate(new[] { new[] { 3, 3, 8, 5 }, new[] { 6, 3, 8, 9 }, new[] { 11, 6, 14, 12 } }));
        }

        [Test]
        public void RectanglesWithSimpleIntersections()
        {
            AreEqual(5, TotalAreaCoveredByRectangles.Calculate(new[] {
                    new[] { 1,4,2,7 },
                    new[] { 1,4,2,6  },
                    new[] { 1,4,4,5  }}));
        }

        [Test]
        public void RectanglesWithSimpleIntersections2()
        {
            AreEqual(1, TotalAreaCoveredByRectangles.Calculate(new[] {
                new[] { 1,1,2,2 },
                new[] { 1,1,2,2  },
                new[] { 1,1,2,2  }}));
        }

        [Test]
        public void RectanglesWithSimpleIntersections1()
        {
            AreEqual(21, TotalAreaCoveredByRectangles.Calculate(new[] { new[] { 1,1,2,2 },
                new[] { 1,4,2,7 },
                new[] { 1,4,2,6  },
                new[] { 1,4,4,5  },
                new[] { 2,5,6,7  },
                new[] { 4,3,7,6}}));
        }
    }
}
