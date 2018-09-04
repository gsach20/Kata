//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using Castle.Components.DictionaryAdapter;
//using Castle.Core.Internal;
//using NUnit.Framework;

//namespace ConsoleApplication1
//{
//    class Skyscrapers
//    {
//        public static int[][] SolvePuzzle(int[] clues)
//        {
//            int[][][] gridValues = new int[4][][];
//            for (var i = 0; i < gridValues.Length; i++)
//            {
//                gridValues[i] = new int[4][];
//                for (var j = 0; j < gridValues[i].Length; j++)
//                {
//                    gridValues[i][j] = new[] { 1, 2, 3, 4 };
//                }
//            }

//            for (var i = 0; i < clues.Length; i++)
//            {
//                if (clues[i] == 0) continue;
//                EliminateNumbers(clues[i], GetRowGridMapping(i, gridValues));
//            }


//            int[][] grid = new int[4][];
//            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
//            {
//                grid[rowIndex] = new int[4];
//                for (var colIndex = 0; colIndex < 4; colIndex++)
//                {
//                    grid[rowIndex][colIndex] = gridValues[rowIndex][colIndex].Aggregate((a, b) => a + b);
//                }
//            }
//            return grid;
//        }

//        private static void EliminateNumbers(int clue, int[][] lane)
//        {
//            if (clue == 1)
//            {
//                SetCellValue(4, lane[0]);
//                RemoveFromCell(4, lane[1]);
//                RemoveFromCell(4, lane[2]);
//                RemoveFromCell(4, lane[3]);
//            }
//            if (clue == 2)
//            {
//                RemoveFromCell(4, lane[0]);
//            }
//            if (clue == 3)
//            {
//                RemoveFromCell(3, lane[0]);
//                RemoveFromCell(4, lane[0]);
//                RemoveFromCell(4, lane[1]);
//            }
//            if (clue == 4)
//            {
//                SetCellValue(1, lane[0]);
//                SetCellValue(2, lane[1]);
//                SetCellValue(3, lane[2]);
//                SetCellValue(4, lane[3]);
//            }
//        }

//        private static void SetCellValue(int value, int[] cellValues)
//        {
//            for (var i = 0; i < cellValues.Length; i++)
//            {
//                if (i == value - 1) continue;
//                cellValues[i] = 0;
//            }
//        }

//        private static void RemoveFromCell(int value, int[] cellValues)
//        {
//            cellValues[value - 1] = 0;
//        }

//        private static int[][] GetRowGridMapping(int clueIndex, int[][][] gridValues)
//        {
//            int[][] lane = new int[4][];

//            if (clueIndex < 4)
//            {
//                lane[0] = gridValues[0][clueIndex];
//                lane[1] = gridValues[1][clueIndex];
//                lane[2] = gridValues[2][clueIndex];
//                lane[3] = gridValues[3][clueIndex];
//            }
//            else if (clueIndex < 8)
//            {
//                lane[0] = gridValues[clueIndex - 4][3];
//                lane[1] = gridValues[clueIndex - 4][2];
//                lane[2] = gridValues[clueIndex - 4][1];
//                lane[3] = gridValues[clueIndex - 4][0];
//            }
//            else if (clueIndex < 12)
//            {
//                lane[0] = gridValues[3][-(clueIndex - 11)];
//                lane[1] = gridValues[2][-(clueIndex - 11)];
//                lane[2] = gridValues[1][-(clueIndex - 11)];
//                lane[3] = gridValues[0][-(clueIndex - 11)];
//            }
//            else
//            {
//                lane[0] = gridValues[-(clueIndex - 15)][0];
//                lane[1] = gridValues[-(clueIndex - 15)][1];
//                lane[2] = gridValues[-(clueIndex - 15)][2];
//                lane[3] = gridValues[-(clueIndex - 15)][3];
//            }

//            return lane;
//        }
//    }

//    class Skyscrapers1
//    {
//        private const int Size = 4;
//        public static int[][] SolvePuzzle(int[] clues)
//        {
//            int allValues = Convert.ToInt32("1111", 2);
//            int[][] gridValues = new int[Size][];
//            for (var rowIndex = 0; rowIndex < Size; rowIndex++)
//            {
//                gridValues[rowIndex] = new[] { allValues, allValues, allValues, allValues };
//            }

//            for (var i = 0; i < clues.Length; i++)
//            {
//                if (clues[i] == 0) continue;
//                EliminateNumbers(clues[i], GetLaneIndices(i), gridValues);
//            }

//            //2nd Iteration
//            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
//            {

//            }



//            int[][] grid = new int[4][];
//            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
//            {
//                grid[rowIndex] = new int[4];
//                for (var colIndex = 0; colIndex < 4; colIndex++)
//                {
//                    grid[rowIndex][colIndex] = gridValues[rowIndex][colIndex].Aggregate((a, b) => a + b);
//                }
//            }
//            return grid;
//        }

//        private static void EliminateNumbers(int clue, int[][] laneIndices, int[][] gridValues)
//        {
//            if (clue == 1)
//            {
//                SetCellValue(4, laneIndices[0], gridValues);
//            }
//            if (clue == 2)
//            {
//                RemoveFromCell(4, laneIndices[0], gridValues);
//            }
//            if (clue == 3)
//            {
//                RemoveFromCell(3, laneIndices[0], gridValues);
//                RemoveFromCell(4, laneIndices[0], gridValues);
//                RemoveFromCell(4, laneIndices[1], gridValues);
//            }
//            if (clue == 4)
//            {
//                SetCellValue(1, laneIndices[0], gridValues);
//                SetCellValue(2, laneIndices[1], gridValues);
//                SetCellValue(3, laneIndices[2], gridValues);
//                SetCellValue(4, laneIndices[3], gridValues);
//            }
//        }

//        private static void SetCellValue(int value, int[] cellIndices, int[][] gridValues)
//        {
//            SetCell(cellIndices, gridValues, GetBinary(value));

//            for (int i = 0; i < 4; i++)
//            {
//                if (i == cellIndices[1]) continue;
//                RemoveFromCell(value, new[] { cellIndices[0], i }, gridValues);
//            }
//            for (int i = 0; i < 4; i++)
//            {
//                if (i == cellIndices[0]) continue;
//                RemoveFromCell(value, new[] { i, cellIndices[1] }, gridValues);
//            }
//        }

//        private static void SetCell(int[] cellIndices, int[][] gridValues, int cellValues)
//        {
//            gridValues[cellIndices[0]][cellIndices[1]] = cellValues;
//        }

//        private static int GetBinary(int value)
//        {
//            char[] binaryString = new[] { '0', '0', '0', '0' };
//            binaryString[value-1] = '1';
//            return Convert.ToInt32(string.Concat(binaryString), 2);
//        }

//        private static ref int GetCell(int[] cellIndices, int[][] gridValues)
//        {
//            return ref gridValues[cellIndices[0]][cellIndices[1]];
//        }

//        private static void RemoveFromCell(int value, int[] cellIndices, int[][] gridValues)
//        {
//            GetCell(cellIndices, gridValues)[value - 1] = 0;
//        }

//        private static int[][] GetLaneIndices(int clueIndex)
//        {
//            int[][] laneIndices = new int[4][];

//            if (clueIndex < 4)
//            {
//                for (int i = 0; i < 4; i++)
//                {
//                    laneIndices[i] = new[] { i, clueIndex };
//                }
//            }
//            else if (clueIndex < 8)
//            {
//                for (int i = 0; i < 4; i++)
//                {
//                    laneIndices[i] = new[] { clueIndex - 4, 3 - i };
//                }
//            }
//            else if (clueIndex < 12)
//            {
//                for (int i = 0; i < 4; i++)
//                {
//                    laneIndices[i] = new[] { 3 - i, -(clueIndex - 11) };
//                }
//            }
//            else
//            {
//                for (int i = 0; i < 4; i++)
//                {
//                    laneIndices[i] = new[] { -(clueIndex - 15), i };
//                }
//            }

//            return laneIndices;
//        }
//    }

//    [TestFixture]
//    public class SkyscrapersTests
//    {
//        [Test]
//        public void SolveSkyscrapers1()
//        {
//            var clues = new[]{ 2, 2, 1, 3,
//                2, 2, 3, 1,
//                1, 2, 2, 3,
//                3, 2, 1, 3};

//            var expected = new[]{ new []{1, 3, 4, 2},
//                new []{4, 2, 1, 3},
//                new []{3, 4, 2, 1},
//                new []{2, 1, 3, 4 }};

//            var actual = Skyscrapers1.SolvePuzzle(clues);
//            CollectionAssert.AreEqual(expected, actual,
//                string.Concat(
//                    string.Concat(expected.Select(r => string.Concat(r.Select(c => c.ToString() + " ")) + Environment.NewLine + "  ")),
//                    Environment.NewLine + "  ",
//                    string.Concat(actual.Select(r => string.Concat(r.Select(c => c.ToString() + " ")) + Environment.NewLine + "  "))));
//        }

//        [Test]
//        public void SolveSkyscrapers2()
//        {
//            var clues = new[]{ 0, 0, 1, 2,
//                0, 2, 0, 0,
//                0, 3, 0, 0,
//                0, 1, 0, 0};

//            var expected = new[]{ new []{2, 1, 4, 3},
//                new []{3, 4, 1, 2},
//                new []{4, 2, 3, 1},
//                new []{1, 3, 2, 4}};

//            var actual = Skyscrapers1.SolvePuzzle(clues);
//            CollectionAssert.AreEqual(expected, actual);
//        }
//    }

//}
