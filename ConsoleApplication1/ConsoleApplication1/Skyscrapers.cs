using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class Skyscrapers
    {
        internal class CellDetails
        {
            public int[] Indices;
            public int[] PossibleValues = {1, 2, 3, 4};
            public int ValueSet = 0;
        }

        public static CellDetails[][] CellDetailsList;

        public static int[][][] PossibleValues;
        public const int Size = 4;

        class LaneDetails
        {
            public int Clue;
            public int Size;
            public int[][] Indices;

            public LaneDetails(int clue, int size, int[][] indices)
            {
                Clue = clue;
                Size = size;
                Indices = indices;
            }

            public override string ToString()
            {
                return Clue+","+Size;
            }
        }

        private static LaneDetails[] laneDetailses;

        public static int[][] SolvePuzzle(int[] clues)
        {
            int[][][] gridValues = new int[Size][][];
            PossibleValues = gridValues;

            CellDetailsList = new CellDetails[Size][];

            for (var i = 0; i < gridValues.Length; i++)
            {
                gridValues[i] = new int[Size][];
                CellDetailsList[i] = new CellDetails[Size];
                for (var j = 0; j < gridValues[i].Length; j++)
                {
                    gridValues[i][j] = new[] {1, 2, 3, 4};
                }
            }

            laneDetailses = new LaneDetails[4*Size];

            for (var i = 0; i < clues.Length; i++)
            {
                laneDetailses[i] = new LaneDetails(clues[i], Size, GetLaneIndices(i));
            }


            PrintValues();
            for (int i = 0; i < 3; i++)
            {
                //Process clues
                foreach (LaneDetails laneDetails in laneDetailses)
                {
                    ProcessClues(laneDetails, gridValues);

                    Debug.WriteLine("Lane first indices: " + string.Join(",",laneDetails.Indices[0]));
                    PrintValues();
                }

                //Eliminate
                EliminateValues(clues, gridValues);

                //Fill remaining

                PrintValues();
            }

            Debug.Print(string.Join(" ", laneDetailses.Select(l => l.Clue.ToString() + "," + l.Size.ToString())));

            int [][] grid = new int[4][];
            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                grid[rowIndex] = new int[4];
                for (var colIndex = 0; colIndex < 4; colIndex++)
                {
                    grid[rowIndex][colIndex] = gridValues[rowIndex][colIndex].Aggregate((a, b) => a + b);
                }
            }
            return grid;
        }

        private static void EliminateValues(int[] clues, int[][][] gridValues)
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                for (int i = 0; i < Size; i++)
                {
                    int uniqueIndex = -1;
                    for (int colIndex = 0; colIndex < Size; colIndex++)
                    {
                        if (gridValues[rowIndex][colIndex][i] != 0)
                        {
                            if (uniqueIndex != -1)
                            {
                                uniqueIndex = -1;
                                break;
                            }

                            uniqueIndex = colIndex;
                        }
                    }

                    if (uniqueIndex != -1) SetCellValue(i + 1, new[] {rowIndex, uniqueIndex}, gridValues);
                }
            }

            for (int colIndex = 0; colIndex < 4; colIndex++)
            {
                for (int i = 0; i < Size; i++)
                {
                    int uniqueIndex = -1;
                    for (int rowIndex = 0; rowIndex < Size; rowIndex++)
                    {
                        if (gridValues[rowIndex][colIndex][i] != 0)
                        {
                            if (uniqueIndex != -1)
                            {
                                uniqueIndex = -1;
                                break;
                            }

                            uniqueIndex = rowIndex;
                        }
                    }

                    if (uniqueIndex != -1) SetCellValue(i + 1, new[] {uniqueIndex, colIndex}, gridValues);
                }
            }

        }

        public static string PrintValues()
        {
            Stack<LaneDetails> orderedClues = new Stack<LaneDetails>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests.GetLaneIndices(clueIndex)))
                .OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => laneDetailses[t.Item1]).Reverse());

            string values = Environment.NewLine + Environment.NewLine
                                                + "      " +
                                                string.Join("      ",
                                                    Enumerable.Range(0, 4).Select(i => orderedClues.Pop())) +
                                                Environment.NewLine
                                                + string.Join(Environment.NewLine,
                                                    Skyscrapers.PossibleValues.Select(r =>
                                                        orderedClues.Pop() + " |" +
                                                        string.Join("|", r.Select(c => string.Join(",", c))) + "| " +
                                                        orderedClues.Pop())) + Environment.NewLine
                                                + "      " +
                                                string.Join("      ",
                                                    Enumerable.Range(0, 4).Select(i => orderedClues.Pop())) +
                                                Environment.NewLine;
            Debug.Print(values);
            return values;
        }

        private static bool ProcessClues(LaneDetails laneDetails, int[][][] gridValues)
        {
            int clue = laneDetails.Clue;
            int size = laneDetails.Size;
            int[][] laneIndices = laneDetails.Indices;
            if (clue == 0) return false;
            for (int value = size; value > size-clue+1; value--)
            {
                for (int i = 0; i < clue + value - size - 1; i++)
                {
                    RemoveFromCell(value, laneIndices[i], gridValues);
                }
            }

            return true;
        }

        private static bool ProcessClues1(LaneDetails laneDetails, int[][][] gridValues)
        {
            int clue = laneDetails.Clue;
            int size = laneDetails.Size;
            int[][] laneIndices = laneDetails.Indices;
            if (clue == 0) return false;
            if (clue == 1)
            {
                if (size > 1)
                {
                    //laneDetails.Clue = 0;
                    SetCellValue(size, laneIndices[0], gridValues);
                }
            }
            else if (clue == size)
            {
                //laneDetails.Clue = 0;
                for (int i = 0; i < size; i++)
                {
                    SetCellValue(i + 1, laneIndices[i], gridValues);
                }
            }
            else
            {

                for (int value = size; value > size-clue+1; value--)
                {
                    for (int i = 0; i < clue+value-size-1; i++)
                    {
                        RemoveFromCell(value, laneIndices[i], gridValues);
                    }
                }
            }

            return true;
        }

        private static void SetCellValue(int value, int[] cellIndices, int[][][] gridValues)
        {
            CellDetails cellDetails = GetCellDetails(cellIndices);



            int[] cellValues = GetCell(cellIndices);

            for (var i = 0; i < 4; i++)
            {
                if (i == value - 1) continue;
                cellValues[i] = 0;
            }

            for (int i = 0; i < 4; i++)
            {
                if(i==cellIndices[1]) continue;
                RemoveFromCell(value, new []{cellIndices[0], i}, gridValues);
            }
            for (int i = 0; i < 4; i++)
            {
                if(i==cellIndices[0]) continue;
                RemoveFromCell(value, new []{i, cellIndices[1]}, gridValues);
            }

            //foreach (int laneIndex in GetClueIndices(cellIndices))
            //{
            //    if(value != laneDetailses[laneIndex].Size) continue;
            //    int[][] laneIndices = laneDetailses[laneIndex].Indices;
            //    int cellPosition = -1;
            //    for (var i = 0; i < laneIndices.Length; i++)
            //    {
            //        if (laneIndices[i].SequenceEqual(cellIndices))
            //        {
            //            cellPosition = i;
            //            break;
            //        }
            //    }

            //    if (cellPosition < laneDetailses[laneIndex].Size)
            //    {
            //        if(laneDetailses[laneIndex].Clue>0)laneDetailses[laneIndex].Clue--;
            //        laneDetailses[laneIndex].Size = cellPosition;
            //    }
            //}
        }

        private static int[] GetClueIndices(int[] cellIndices)
        {
            return new [] {cellIndices[1], cellIndices[0] + Size, 3*Size-cellIndices[1]-1, 4*Size-cellIndices[0]-1};
        }

        private static ref int[] GetCell(int[] cellIndices)
        {
            return ref PossibleValues[cellIndices[0]][cellIndices[1]];
        }

        private static CellDetails GetCellDetails(int[] cellIndices)
        {
            return CellDetailsList[cellIndices[0]][cellIndices[1]];
        }
       
        private static void RemoveFromCell(int value, int[] cellIndices, int[][][] gridValues)
        {
            if(GetCell(cellIndices)[value-1] == 0) return;

            GetCell(cellIndices)[value-1] = 0;

            CellDetails cellDetails = GetCellDetails(cellIndices);
            if(cellDetails.PossibleValues[value-1] == 0) return;
            cellDetails.PossibleValues[value - 1] = 0;

            EliminateValuesSingle(value, cellIndices);
        }

        

        private static void EliminateValuesSingle(int value, int[] cellIndices)
        {
            int uniqueValueInCell = GetUniqueValueInCell(cellIndices);
            if(uniqueValueInCell > 0)
                SetCellValue(uniqueValueInCell, cellIndices, PossibleValues);


            //Iterate over column
            {
                int[] uniqueCellIndices = null;
                for (int i = 0; i < Size; i++)
                {
                    int[] currentCellIndices = {i, cellIndices[1]};
                    if (GetCell(currentCellIndices)[value - 1] != 0)
                    {
                        if (uniqueCellIndices != null)
                        {
                            uniqueCellIndices = null;
                            break;
                        }

                        uniqueCellIndices = currentCellIndices;
                    }
                }

                if(uniqueCellIndices != null) SetCellValue(value, uniqueCellIndices, PossibleValues);
            }
            
            //Iterate over rows
            {
                int[] uniqueCellIndices = null;
                for (int i = 0; i < Size; i++)
                {
                    int[] currentCellIndices = {cellIndices[0], i};
                    if (GetCell(currentCellIndices)[value - 1] != 0)
                    {
                        if (uniqueCellIndices != null)
                        {
                            uniqueCellIndices = null;
                            break;
                        }

                        uniqueCellIndices = currentCellIndices;
                    }
                }

                if(uniqueCellIndices != null) SetCellValue(value, uniqueCellIndices, PossibleValues);
            }
        }

        private static int GetUniqueValueInCell(int[] cellIndices)
        {
            int numberFound = 0;
            foreach (int i in GetCell(cellIndices))
            {
                if (i != 0)
                {
                    if (numberFound > 0) return 0;
                    numberFound = i;
                }
            }

            if (numberFound > 0) return numberFound;

            throw new InvalidOperationException("All values in a cell are zero");
        }

        public static int[][] GetLaneIndices(int clueIndex)
        {
            int[][] laneIndices = new int[Size][];
            if (clueIndex < Size)
            {
                for (var i = 0; i < Size; i++)
                {
                    laneIndices[i] = new[] { i, clueIndex };
                }
            }
            else if (clueIndex < 2 * Size)
            {
                for (var i = 0; i < Size; i++)
                {
                    laneIndices[i] = new[] { clueIndex - Size, Size - i - 1 };
                }
            }
            else if (clueIndex < 3 * Size)
            {
                for (var i = 0; i < Size; i++)
                {
                    laneIndices[i] = new[] { Size - i - 1, 3 * Size - clueIndex - 1 };
                }
            }
            else
            {
                for (var i = 0; i < Size; i++)
                {
                    laneIndices[i] = new[] { 4 * Size - clueIndex - 1, i };
                }
            }

            return laneIndices;
        }

        private static int[][] GetLaneIndices1(int clueIndex)
        {
            int[][] laneIndices = new int[4][];
            
            if (clueIndex < 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    laneIndices[i] = new[] {i, clueIndex};
                }
            }
            else if (clueIndex < 8)
            {
                for (int i = 0; i < 4; i++)
                {
                    laneIndices[i] = new[] {clueIndex-4, 3-i};
                }
            }
            else if (clueIndex < 12)
            {
                for (int i = 0; i < 4; i++)
                {
                    laneIndices[i] = new[] {3-i, -(clueIndex-11)};
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    laneIndices[i] = new[] {-(clueIndex-15), i};
                }
            }

            return laneIndices;
        }
    }

    [TestFixture]
    public class SkyscrapersTests
    {
        private const int Size = Skyscrapers.Size;
        public static int[] GetLaneIndices(int clueIndex)
        {
            if (clueIndex < Size)
            {
                return new[] { -1, clueIndex };
            }
            if (clueIndex < 2 * Size)
            {
                return new[] { clueIndex - Size, 1 };
            }
            if (clueIndex < 3 * Size)
            {
                return new[] { 4, 3 * Size - clueIndex - 1 };
            }
            return new[] { 4 * Size - clueIndex - 1, 0 };
        }


        [Test]
        public void SolveSkyscrapers1()
        {
            var clues = new[]{ 
                2, 2, 1, 3, 
                2, 2, 3, 1, 
                1, 2, 2, 3, 
                3, 2, 1, 3};
                           
            var expected = new []{ new []{1, 3, 4, 2},       
                new []{4, 2, 1, 3},       
                new []{3, 4, 2, 1},
                new []{2, 1, 3, 4 }};

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                Environment.NewLine
                + string.Join(Environment.NewLine, expected.Select(r => string.Join("|", r)))
                + Environment.NewLine + Environment.NewLine
                + "   "+  string.Join(" " , Enumerable.Range(0,4).Select(i=> orderedClues.Pop())) + Environment.NewLine
                + string.Join(Environment.NewLine, Skyscrapers.PossibleValues.Select(r => orderedClues.Pop() + " |" + string.Join("|", r.Select(c => c.Count(v=>v==0)==3 ? c.First(v=>v!=0).ToString() : " ")) + "| " + orderedClues.Pop())) + Environment.NewLine
                + "   " + string.Join(" ", Enumerable.Range(0, 4).Select(i => orderedClues.Pop())) + Environment.NewLine
                + Environment.NewLine + Skyscrapers.PrintValues()
            );
        }

        [Test]
        public void SolveSkyscrapers2()
        {
            var clues = new[]{ 
                0, 0, 1, 2,  
                0, 2, 0, 0,  
                0, 3, 0, 0,
                0, 1, 0, 0};

            var expected = new []{ new []{2, 1, 4, 3}, 
                new []{3, 4, 1, 2}, 
                new []{4, 2, 3, 1}, 
                new []{1, 3, 2, 4}};


            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                Environment.NewLine
                + string.Join(Environment.NewLine, expected.Select(r => string.Join("|", r)))
                + Environment.NewLine + Environment.NewLine
                + "   "+  string.Join(" " , Enumerable.Range(0,4).Select(i=> orderedClues.Pop())) + Environment.NewLine
                + string.Join(Environment.NewLine, Skyscrapers.PossibleValues.Select(r => orderedClues.Pop() + " |" + string.Join("|", r.Select(c => c.Count(v=>v==0)==3 ? c.First(v=>v!=0).ToString() : " ")) + "| " + orderedClues.Pop())) + Environment.NewLine
                + "   " + string.Join(" ", Enumerable.Range(0, 4).Select(i => orderedClues.Pop())) + Environment.NewLine
                + Environment.NewLine + Skyscrapers.PrintValues()
            );
        }
    }

}
