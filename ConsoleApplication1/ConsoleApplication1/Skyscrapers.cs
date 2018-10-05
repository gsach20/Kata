using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class Skyscrapers
    {
        public const int Size = 6;

        public static Cell[][] AllCells;

        internal class Cell
        {
            public int X;
            public int Y;
            public readonly List<int> PossibleValues;
            private int _valueSet;

            public Cell(int x, int y)
            {
                X = x;
                Y = y;
                PossibleValues = new List<int>(Enumerable.Range(1, Size));
            }

            public void RemoveValue(int value)
            {
                if (!PossibleValues.Remove(value)) return;

                if (PossibleValues.Count == 1)
                {
                    SetCellValue(PossibleValues.First());
                }

                EliminateValuesSingle(value);
            }

            private void EliminateValuesSingle(int value)
            {
                //Iterate over column and find if this value is present only in one cell then set value for that cell.
                Cell uniqueCell = null;
                for (int x = 0, y = Y; x < Size; x++)
                {
                    Cell currCell = AllCells[x][y];
                    if (currCell.PossibleValues.Contains(value))
                    {
                        if (uniqueCell != null)
                        {
                            uniqueCell = null;
                            break;
                        }

                        uniqueCell = currCell;
                    }
                }
                uniqueCell?.SetCellValue(value);

                //Iterate over rows
                uniqueCell = null;
                for (int x = X, y = 0; y < Size; y++)
                {
                    Cell currCell = AllCells[x][y];
                    if (currCell.PossibleValues.Contains(value))
                    {
                        if (uniqueCell != null)
                        {
                            uniqueCell = null;
                            break;
                        }

                        uniqueCell = currCell;
                    }
                }
                uniqueCell?.SetCellValue(value);
            }


            public void SetCellValue(int value)
            {
                if (_valueSet != 0) return;
                _valueSet = value;

                List<int> toBeRemoved = PossibleValues.Where(v => v != value).ToList();
                toBeRemoved.ForEach(RemoveValue);

                for (int i = 0; i < Size; i++)
                {
                    if (i == Y) continue;
                    GetCell(X, i).RemoveValue(value);
                }
                for (int i = 0; i < Size; i++)
                {
                    if (i == X) continue;
                    GetCell(i, Y).RemoveValue(value);
                }

                foreach (int laneIndex in GetLanesContainingCell(this))
                {
                    Lane lane = _lanesList[laneIndex];

                    if (lane.Clue == 0) continue;

                    if (this != lane.Cells.Last()) continue;

                    int oldSize = lane.Cells.Count;
                    int newSize = oldSize;
                    for (; newSize > 0; newSize--)
                    {
                        if (lane.Cells[newSize - 1]._valueSet == 0) break;
                    }

                    if (!lane.Cells.Where((c, i) => i > newSize - 1 && c._valueSet == oldSize).Any()) continue;


                    int numberOfVisible = 1;
                    int highestBuildingSize = lane.Cells[newSize]._valueSet;
                    for (int j = newSize + 1; j < oldSize; j++)
                    {
                        int currBuildingSize = lane.Cells[j]._valueSet;
                        if (currBuildingSize > highestBuildingSize)
                        {
                            numberOfVisible++;
                            highestBuildingSize = currBuildingSize;
                        }
                    }

                    lane.Clue = lane.Clue > numberOfVisible ? lane.Clue - numberOfVisible : 0;

                    lane.Cells.RemoveRange(newSize, oldSize - newSize);
                }
            }
        }

        private class Lane
        {
            public int Clue;
            public readonly List<Cell> Cells;

            public Lane(int clue, int index)
            {
                Clue = clue;
                Cells = GetLaneCells(index);
            }

            private static List<Cell> GetLaneCells(int index)
            {
                List<Cell> laneCells = new List<Cell>(Size);
                if (index < Size)
                {
                    for (var i = 0; i < Size; i++)
                    {
                        laneCells.Add(GetCell(i, index));
                    }
                }
                else if (index < 2 * Size)
                {
                    for (var i = 0; i < Size; i++)
                    {
                        laneCells.Add(GetCell(index - Size, Size - i - 1));
                    }
                }
                else if (index < 3 * Size)
                {
                    for (var i = 0; i < Size; i++)
                    {
                        laneCells.Add(GetCell(Size - i - 1, 3 * Size - index - 1));
                    }
                }
                else
                {
                    for (var i = 0; i < Size; i++)
                    {
                        laneCells.Add(GetCell(4 * Size - index - 1, i));
                    }
                }

                return laneCells;
            }

            public override string ToString()
            {
                return Clue + "," + Cells.Count;
            }

            public string FirstIndices()
            {
                if (Cells.Any()) return Cells.First().X + "," + Cells.First().Y;
                return "S00";
            }
        }

        private static Lane[] _lanesList;

        public static int[][] SolvePuzzle(int[] clues)
        {
            AllCells = new Cell[Size][];

            for (var i = 0; i < Size; i++)
            {
                AllCells[i] = new Cell[Size];
                for (var j = 0; j < Size; j++)
                {
                    AllCells[i][j] = new Cell(i, j);
                }
            }

            _lanesList = new Lane[4 * Size];

            for (var i = 0; i < clues.Length; i++)
            {
                _lanesList[i] = new Lane(clues[i], i);
            }


            PrintValues();
            for (int i = 0; i < 10; i++)
            {
                //Process clues
                //foreach (Lane lane in LanesList)
                for (var i1 = 0; i1 < _lanesList.Length; i1++)
                {
                    Lane lane = _lanesList[i1];
                    ProcessClues(lane);

                    Debug.WriteLine(Environment.NewLine + "Lane first indices: " + lane.FirstIndices());
                    Console.WriteLine(Environment.NewLine + "Lane first indices: " + lane.FirstIndices());

                    PrintValues();
                }

                if (AllCells.Count(r => r.Count(c => c.PossibleValues.Count != 1) != 0) == 0) break;

                //PrintValues();
            }

            //Debug.Print(string.Join(" ", laneDetailses.Select(l => l.Clue.ToString() + "," + l.Size.ToString())));

            int[][] grid = new int[Size][];
            for (var rowIndex = 0; rowIndex < Size; rowIndex++)
            {
                grid[rowIndex] = new int[Size];
                for (var colIndex = 0; colIndex < Size; colIndex++)
                {
                    grid[rowIndex][colIndex] = AllCells[rowIndex][colIndex].PossibleValues.Aggregate((a, b) => a + b);
                }
            }

            PrintValues();

            return grid;
        }

        public static string PrintValues()
        {
            Stack<Lane> orderedClues = new Stack<Lane>(Enumerable.Range(0, 4 * Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex)))
                .OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => _lanesList[t.Item1]).Reverse());

            string values = Environment.NewLine
                                                + "      " +
                                                string.Join(string.Concat(Enumerable.Range(0,2*Size-2).Select(i => " ")),
                                                    Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) +
                                                Environment.NewLine
                                                + string.Join(Environment.NewLine,
                                                    AllCells.Select(r =>
                                                        orderedClues.Pop() + " |" +
                                                        string.Join("|", r.Select(c => string.Join(",", c.PossibleValues) + string.Concat(Enumerable.Range(0, Size - c.PossibleValues.Count).Select(i => "  ")))) + "| " +
                                                        orderedClues.Pop())) + Environment.NewLine
                                                + "      " +
                                                string.Join(string.Concat(Enumerable.Range(0,2*Size-2).Select(i => " ")),
                                                    Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) +
                                                Environment.NewLine;
            Debug.Print(values);
            Console.Write(values);
            return values;
        }

        private static bool ProcessClues(Lane lane)
        {
            int clue = lane.Clue;
            int size = lane.Cells.Count;
            if (clue == 0) return false;
            if (clue == 1)
            {
                lane.Cells[0].SetCellValue(lane.Cells[0].PossibleValues.Max());
            }
            else if (clue == size)
            {
                for (int i = 0; i < size; i++)
                {
                    lane.Cells.ElementAtOrDefault(i)?.SetCellValue(i + 1);
                }
            }
            else
            {
                if (clue == 2 && size > 1)
                {
                    lane.Cells[1].RemoveValue(size - 1);
                }
                for (int value = size; value > size - clue + 1; value--)
                {
                    for (int i = 0; i < clue + value - size - 1; i++)
                    {
                        lane.Cells.ElementAtOrDefault(i)?.RemoveValue(value);
                    }
                }
            }

            return true;
        }

        private static int[] GetLanesContainingCell(Cell cell)
        {
            return new[] { cell.Y, cell.X + Size, 3 * Size - cell.Y - 1, 4 * Size - cell.X - 1 };
        }

        private static Cell GetCell(int x, int y)
        {
            return AllCells[x][y];
        }

        public static int[] GetLaneIndices2(int clueIndex)
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
                return new[] { Size, 3 * Size - clueIndex - 1 };
            }
            return new[] { 4 * Size - clueIndex - 1, 0 };
        }
    }

    [TestFixture]
    public class SkyscrapersTests
    {
        private const int Size = Skyscrapers.Size;



        [Test]
        public void SolveSkyscrapers1()
        {
            var clues = new[]{
                2, 2, 1, 3,
                2, 2, 3, 1,
                1, 2, 2, 3,
                3, 2, 1, 3};

            var expected = new[]{ new []{1, 3, 4, 2},
                new []{4, 2, 1, 3},
                new []{3, 4, 2, 1},
                new []{2, 1, 3, 4 }};

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
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

            var expected = new[]{ new []{2, 1, 4, 3},
                new []{3, 4, 1, 2},
                new []{4, 2, 3, 1},
                new []{1, 3, 2, 4}};


            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
            );
        }

        [Test]
        public void SolveSkyscrapers3()
        {
            var clues = new[]{
                1, 2, 4, 2,
                2, 1, 3, 2,
                3, 1, 2, 3,
                3, 2, 2, 1};

            var expected = new[]{
                new []{ 4, 2, 1, 3},
                new []{ 3, 1, 2, 4},
                new []{ 1, 4, 3, 2},
                new []{ 2, 3, 4, 1}};


            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
            );
        }

        [Test]
        public void SolveSkyscrapers4()
        {
            var clues = new[]{
                2, 2, 1, 3,
                2, 2, 3, 1,
                1, 2, 2, 3,
                3, 2, 1, 3};

            var expected = new[]{
                new []{ 1, 3, 4, 2},
                new []{ 4, 2, 1, 3},
                new []{ 3, 4, 2, 1},
                new []{ 2, 1, 3, 4}};


            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
            );
        }

        [Test]
        public void SolveSkyscrapers5()
        {
            var clues = new[]{
                0, 2, 0, 0,
                0, 3, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 2};

            var expected = new[]{
                new []{ 3, 2, 1, 4},
                new []{ 4, 1, 3, 2},
                new []{ 1, 4, 2, 3},
                new []{ 2, 3, 4, 1}};


            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
            );
        }

        [Test]
        public void SolveSkyscrapers6()
        {
            var clues = new[]{
                2, 2, 3, 1,
                1, 2, 2, 3,
                3, 2, 1, 3,
                2, 2, 1, 3};

            var expected = new[]{
                new []{ 2, 3, 1, 4},
                new []{ 4, 1, 2, 3},
                new []{ 3, 2, 4, 1},
                new []{ 1, 4, 3, 2}};


            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
            );
        }

        public static string ErrorMessage(int[][] expected, int[][] actual, Stack<int> orderedClues)
        {
            return Environment.NewLine
                   + string.Join(Environment.NewLine, expected.Select(r => string.Join("|", r)))
                   + Environment.NewLine + Environment.NewLine
                   + "   " + string.Join(" ", Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) + Environment.NewLine
                   + string.Join(Environment.NewLine, Skyscrapers.AllCells.Select(r => orderedClues.Pop() + " |" + string.Join("|", r.Select(c => c.PossibleValues.Count == 1 ? c.PossibleValues.FirstOrDefault(v => v != 0).ToString() : " ")) + "| " + orderedClues.Pop())) + Environment.NewLine
                   + "   " + string.Join(" ", Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) + Environment.NewLine
                   + Environment.NewLine + Skyscrapers.PrintValues()
                   + Environment.NewLine + "Difference index: " + DifferenceIndex(expected, actual)
                   + Environment.NewLine;

        }

        private static string DifferenceIndex(int[][] expected, int[][] actual)
        {
            for (int i = 0; i < Skyscrapers.Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (expected[i][j] != actual[i][j]) return i + "," + j;
                }
            }

            return string.Empty;
        }
    }

    [TestFixture]
    public class SkyscrapersTests2
    {

        [Test]
        public void SolvePuzzle1()
        {
            var clues = new[]{ 3, 2, 2, 3, 2, 1,
                           1, 2, 3, 3, 2, 2,
                           5, 1, 2, 2, 4, 3,
                           3, 2, 1, 2, 2, 4};

            var expected = new[]{new []{ 2, 1, 4, 3, 5, 6},
                             new []{ 1, 6, 3, 2, 4, 5},
                             new []{ 4, 3, 6, 5, 1, 2},
                             new []{ 6, 5, 2, 1, 3, 4},
                             new []{ 5, 4, 1, 6, 2, 3},
                             new []{ 3, 2, 5, 4, 6, 1 }};

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests.ErrorMessage(expected, actual, orderedClues));
        }

        [Test]
        public void SolvePuzzle2()
        {
            var clues = new[]{ 0, 0, 0, 2, 2, 0,
                            0, 0, 0, 6, 3, 0,
                            0, 4, 0, 0, 0, 0,
                            4, 4, 0, 3, 0, 0};

            var expected = new[]{new []{ 5, 6, 1, 4, 3, 2 },
                             new []{ 4, 1, 3, 2, 6, 5 },
                             new []{ 2, 3, 6, 1, 5, 4 },
                             new []{ 6, 5, 4, 3, 2, 1 },
                             new []{ 1, 2, 5, 6, 4, 3 },
                             new []{ 3, 4, 2, 5, 1, 6 }};

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SolvePuzzle3()
        {
            var clues = new[] { 0, 3, 0, 5, 3, 4,
                            0, 0, 0, 0, 0, 1,
                            0, 3, 0, 3, 2, 3,
                            3, 2, 0, 3, 1, 0};

            var expected = new[]{new []{ 5, 2, 6, 1, 4, 3 },
                             new []{ 6, 4, 3, 2, 5, 1 },
                             new []{ 3, 1, 5, 4, 6, 2 },
                             new []{ 2, 6, 1, 5, 3, 4 },
                             new []{ 4, 3, 2, 6, 1, 5 },
                             new []{ 1, 5, 4, 3, 2, 6 }};

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }
    }

}
