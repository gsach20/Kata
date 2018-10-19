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
        private static Lane[] AllLanes;

        internal class Cell
        {
            public int X;
            public int Y;
            public readonly List<int> PossibleValues;
            public int ValueSet;

            public Cell(int x, int y)
            {
                X = x;
                Y = y;
                PossibleValues = new List<int>(Enumerable.Range(1, Size));
            }

            public Cell(Cell cell)
            {
                X = cell.X;
                Y = cell.Y;
                PossibleValues = new List<int>(cell.PossibleValues);
                ValueSet = cell.ValueSet;
            }

            public Result RemoveValue(int value)
            {
                if (!PossibleValues.Remove(value)) return Result.NoChange;

                if (PossibleValues.Count == 0) return Result.Failed;

                if (PossibleValues.Count == 1)
                {
                    if (Result.Failed == (SetCellValue(PossibleValues.First()) & Result.Failed)) return Result.Failed;
                }

                if (EliminateValuesSingle(value) == Result.Failed) return Result.Failed;

                return Result.Changed;
            }

            private Result EliminateValuesSingle(int value)
            {
                Result result = Result.NoChange;
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
                if(uniqueCell != null) result = uniqueCell.SetCellValue(value);
                if( result == Result.Failed) return Result.Failed;

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
                if(uniqueCell != null) result = uniqueCell.SetCellValue(value);
                return result;
            }


            public Result SetCellValue(int value)
            {
                if (ValueSet != 0) return Result.NoChange;
                ValueSet = value;

                List<int> toBeRemoved = PossibleValues.Where(v => v != value).ToList();
                if(toBeRemoved.Any(v => RemoveValue(v) == Result.Failed)) return Result.Failed;

                for (int i = 0; i < Size; i++)
                {
                    if (i == Y) continue;
                    if(GetCell(X, i).RemoveValue(value) == Result.Failed) return Result.Failed;
                }
                for (int i = 0; i < Size; i++)
                {
                    if (i == X) continue;
                    if(GetCell(i, Y).RemoveValue(value) == Result.Failed) return Result.Failed;
                }

                foreach (int laneIndex in GetLanesContainingCell(this))
                {
                    Lane lane = AllLanes[laneIndex];

                    if (lane.Clue == 0) continue;

                    //if (this != lane.Cells.Last()) continue;

                    for (var i = lane.Cells.Count - 1; i >= 0; i--)
                    {
                        Cell cell = lane.Cells[i];
                        if(cell.ValueSet == 0) break;
                        lane.Cells.RemoveAt(i);
                        if (!lane.Cells.Any()) lane.Clue = 0;
                        else if (cell.ValueSet > lane.Cells.Select(c => c.PossibleValues.Max()).Max()) lane.Clue--;
                    }

                    //int oldSize = lane.Cells.Count;
                    //int newSize = oldSize;
                    //int numberOfVisible = 0;
                    //for (; newSize > 0; newSize--)
                    //{
                    //    if (lane.Cells[newSize - 1]._valueSet == 0) break;

                    //}



                    //if(lane.Cells[newSize]._valueSet >= lane.HighestPossibleValue())
                    //{
                    //    int numberOfVisible = 1;
                    //    int highestBuildingSize = lane.Cells[newSize]._valueSet;
                    //    for (int j = newSize + 1; j < oldSize; j++)
                    //    {
                    //        int currBuildingSize = lane.Cells[j]._valueSet;
                    //        if (currBuildingSize > highestBuildingSize)
                    //        {
                    //            numberOfVisible++;
                    //            highestBuildingSize = currBuildingSize;
                    //        }
                    //    }

                    //    lane.Clue = lane.Clue > numberOfVisible ? lane.Clue - numberOfVisible : 0;
                    //}

                    //lane.Cells.RemoveRange(newSize, oldSize - newSize);
                }

                return Result.Changed;
            }

            public void CopyFrom(Cell cell)
            {
                PossibleValues.Clear();
                PossibleValues.AddRange(cell.PossibleValues);
                ValueSet = cell.ValueSet;
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

            public Lane(Lane lane)
            {
                Clue = lane.Clue;
                Cells = new List<Cell>(lane.Cells);
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

            public int HighestPossibleValue()
            {
                return Cells.Select(c => c.PossibleValues.Max()).Max();
            }

            public Lane GetOppositeLane()
            {
                if(Cells.Count < 2) throw new InvalidOperationException("Call this method only if there are more than two cells in a lane");
                if (Cells[0].X == Cells[1].X)
                {
                    if(Cells[0].Y > Cells[1].Y)
                        return AllLanes[4 * Size - Cells[0].X - 1];
                    return AllLanes[Size + Cells[0].X];
                }
                else
                {
                    if(Cells[0].X < Cells[1].X)
                        return AllLanes[3 * Size - Cells[0].Y - 1];
                    return AllLanes[Cells[0].Y];
                }
            }
        }

        private class Backup
        {
            private readonly Cell[][] allCells;
            private Lane[] allLanes;

            public Backup()
            {
                allCells = new Cell[Size][];
                for (var i = 0; i < Size; i++)
                {
                    allCells[i] = new Cell[Size];
                    for (int j = 0; j < Size; j++)
                    {
                        allCells[i][j] = new Cell(AllCells[i][j]);
                    }
                }

                allLanes = new Lane[4*Size];
                for (var i = 0; i < allLanes.Length; i++)
                {
                    allLanes[i] = new Lane(AllLanes[i]);
                }
            }

            public void Restore()
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        AllCells[i][j].CopyFrom(allCells[i][j]);
                    }
                }

                AllLanes = allLanes;
            }
        }

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

            AllLanes = new Lane[4 * Size];

            for (var i = 0; i < clues.Length; i++)
            {
                AllLanes[i] = new Lane(clues[i], i);
            }

            if(Result.Failed == (ProcessClues() & Result.Failed)) return null;

            if (!PuzzleSolved())
            {
                FindChoicesAndEliminate(clues);
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

            Console.WriteLine(string.Join(" ", clues));

            //PrintValues();

            return grid;
        }

        private static bool FindChoicesAndEliminate(int[] clues)
        {
            Backup backup = new Backup();
            foreach (Cell cell in GetChoices(out int valueToEliminate))
            {
                if (Result.Failed == (cell.RemoveValue(valueToEliminate) & Result.Failed))
                {
                    backup.Restore();
                    continue;
                }

                //PrintValues();

                bool puzzleSolved = PuzzleSolved();

                //if (!puzzleSolved)
                //{
                //    if (Result.Failed == (ProcessClues() & Result.Failed))
                //    {
                //        backup.Restore();
                //        continue;
                //    }
                //    puzzleSolved = PuzzleSolved();
                //}

                if (puzzleSolved)
                {
                    if (ValidateClues(clues))
                    {
                        return true;
                    }

                    backup.Restore();
                }
                else
                {
                    if (FindChoicesAndEliminate(clues)) return true;                    
                    
                    backup.Restore();
                }
            }

            return false;
        }

        private static bool ValidateClues(int[] clues)
        {
            for (var i = 0; i < clues.Length; i++)
            {
                if(clues[i] == 0) continue;
                Lane lane = new Lane(clues[i], i);
                int numberOfVisible = 0;
                int highestNumber = 0;
                foreach (Cell cell in lane.Cells)
                {
                    if (cell.ValueSet > highestNumber)
                    {
                        numberOfVisible++;
                        highestNumber = cell.ValueSet;
                    }
                }

                if (numberOfVisible != clues[i]) return false;
            }

            return true;
        }

        private static bool PuzzleSolved()
        {
            return !AllCells.Any(r => r.Any(c => c.ValueSet == 0));
        }

        private static Cell[] GetChoices(out int valueToEliminate)
        {
            foreach (Lane lane in AllLanes)
            {
                foreach (int value in Enumerable.Range(1, Size))
                {
                    IEnumerable<Cell> possibleCells = lane.Cells.Where(c => c.PossibleValues.Contains(value));
                    if (possibleCells.Count() == 2)
                    {
                        valueToEliminate = value;
                        return possibleCells.ToArray();
                    }
                }
            }

            valueToEliminate = 0;
            return new Cell[0];
        }

        private static Result ProcessClues()
        {
            Result result = Result.Changed;
            int iterationCount = 1;
            for (; result == Result.Changed; iterationCount++)
            {
                //Process clues
                result = Result.NoChange;
                foreach (var lane in AllLanes)
                {
                    result = ApplySimpleRules(lane) | result;
                    if (Result.Failed == (result & Result.Failed)) return Result.Failed;

                    Debug.WriteLine(Environment.NewLine + "Lane first indices: " + lane.FirstIndices());
                    Console.WriteLine(Environment.NewLine + "Lane first indices: " + lane.FirstIndices());

                    PrintValues();

                }

                //if (result == Result.Changed) continue;

                //foreach (Lane lane in AllLanes)
                //{
                //    result = ApplyAdvancedRules(lane) | result;
                //    if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                //}

                //if (result == Result.Changed) continue;

                ////PrintValues();
                //foreach (var lane in AllLanes)
                //{
                //    result = ApplyMoreAdvancedRules(lane) | result;
                //    if (Result.Failed == (result & Result.Failed)) return Result.Failed;

                //    //Debug.WriteLine(Environment.NewLine + "Lane first indices: " + lane.FirstIndices());
                //    //Console.WriteLine(Environment.NewLine + "Lane first indices: " + lane.FirstIndices());

                //    //PrintValues();
                //}
            }

            return result;
        }

        private static Result ApplyAdvancedRules(Lane lane)
        {
            int clue = lane.Clue;
            int size = lane.Cells.Count;
            if (clue == 2 && size == Size)
            {
                Lane oppositeLane = lane.GetOppositeLane();
                if ( oppositeLane.Clue == 2 && oppositeLane.Cells.Count == Size)
                {
                    if (!lane.Cells[0].PossibleValues.Contains(Size - 1))
                    {
                        return oppositeLane.Cells[0].SetCellValue(Size - 1);
                    }

                    if (!oppositeLane.Cells[0].PossibleValues.Contains(Size - 1))
                    {
                        return lane.Cells[0].SetCellValue(Size - 1);
                    }
                }
            }

            if (clue < 2) return Result.NoChange;

            List<int> possibleValuesInLane = lane.Cells.SelectMany(c => c.PossibleValues).Distinct().ToList();
            possibleValuesInLane.Sort();
            int numberOfVisible = 0;
            int firstPossiblePositionOfLastItem = possibleValuesInLane.Count;
            int i = possibleValuesInLane.Count - 1;
            for (; i >= 0; i--)
            {
                int lastPossiblePosition = lane.Cells.FindLastIndex(c => c.PossibleValues.Contains(possibleValuesInLane[i]));
                if (lastPossiblePosition >= firstPossiblePositionOfLastItem) break;
                numberOfVisible++;
                firstPossiblePositionOfLastItem = lane.Cells.FindIndex(c => c.PossibleValues.Contains(possibleValuesInLane[i]));
            }

            Result result = Result.NoChange;
            if (firstPossiblePositionOfLastItem > 1)
            {
                if (clue - numberOfVisible == 1)
                {
                    result = lane.Cells[0].RemoveValue(possibleValuesInLane.First());
                    if (result == Result.Failed) return Result.Failed;
                    if (lane.Cells.Count > 1)
                        result = lane.Cells[1].RemoveValue(possibleValuesInLane[i]) | result;
                }
                else if (firstPossiblePositionOfLastItem == clue - numberOfVisible)
                {
                    result = lane.Cells[firstPossiblePositionOfLastItem - 1].RemoveValue(lane.Cells.GetRange(0, firstPossiblePositionOfLastItem).Select(c => c.PossibleValues.Min()).Min());
                }
            }

            return result;
        }

        private static Result ApplyMoreAdvancedRules(Lane lane)
        {
            if (lane.Clue < 3) return Result.NoChange;

            int numberOfVisible = 1;
            var i = 1;
            for (; i < lane.Cells.Count; i++)
            {
                if (lane.Cells[i].ValueSet == 0) break;
                if (lane.Cells[i].ValueSet > lane.Cells.GetRange(0, i).Select(c => c.PossibleValues.Max()).Max())
                    numberOfVisible++;
            }

            if (numberOfVisible == 1 && lane.Cells[0].ValueSet == 0) return Result.NoChange;

            if (lane.Clue - numberOfVisible == 1)
                return lane.Cells[i].SetCellValue(lane.Cells[i].PossibleValues.Max());
            if (lane.Clue - numberOfVisible > 1)
                return lane.Cells[i].RemoveValue(lane.Cells[i].PossibleValues.Max());
            return Result.NoChange;
        }

        public static string PrintValues()
        {
            Stack<Lane> orderedClues = new Stack<Lane>(Enumerable.Range(0, 4 * Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex)))
                .OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => AllLanes[t.Item1]).Reverse());

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

        private static Result ApplySimpleRules(Lane lane)
        {
            int clue = lane.Clue;
            int size = lane.Cells.Count;
            if (clue == 0) return Result.NoChange;
            Result result = Result.NoChange;
            if (clue == 1)
            {
                return lane.Cells[0].SetCellValue(lane.Cells[0].PossibleValues.Max());
            }

            if (clue == size)
            {
                for (int i = 0; i < size; i++)
                {
                    Cell cell = lane.Cells.ElementAtOrDefault(i);
                    if (cell != null)
                    {
                        result = cell.SetCellValue(cell.PossibleValues.Min()) | result;
                        if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                    }
                }

                return result;
            }
            
            List<int> possibleValuesInLane = lane.Cells.SelectMany(c => c.PossibleValues).Distinct().ToList();
            possibleValuesInLane.Sort();

            if (clue == 2 && size > 1)
            {
                result = lane.Cells[1].RemoveValue(possibleValuesInLane[size - 2]);
                if (Result.Failed == result) return Result.Failed;
            }

            for (int indexToBeRemoved = size; indexToBeRemoved > size - clue + 1; indexToBeRemoved--)
            {
                for (int i = 0; i < clue + indexToBeRemoved - size - 1; i++)
                {
                    Cell cell = lane.Cells.ElementAtOrDefault(i);
                    if (cell != null)
                    {
                        result = cell.RemoveValue(possibleValuesInLane[indexToBeRemoved-1]) | result;
                        if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                    }
                }
            }

            return result;
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

    [Flags]
    internal enum Result
    {
        NoChange = 0,
        Changed = 1,
        Failed = 2
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
                   + "   " + string.Join("  ", Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) + Environment.NewLine
                   + string.Join(Environment.NewLine, Skyscrapers.AllCells.Select(r => orderedClues.Pop() + " |" + string.Join("|", r.Select(c => c.PossibleValues.Count == 1  ? c.PossibleValues.First() == expected[c.X][c.Y] ? c.PossibleValues.First() + " " : c.PossibleValues.First() + "*" : " *")) + "| " + orderedClues.Pop())) + Environment.NewLine
                   + "   " + string.Join("  ", Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) + Environment.NewLine
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

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests.ErrorMessage(expected, actual, orderedClues));
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

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests.ErrorMessage(expected, actual, orderedClues));
        }

        [Test]
        public void SolvePuzzle4()
        {
            var clues = new[] { 4,4,0,3,0,0,0,0,0,2,2,0,0,0,0,6,3,0,0,4,0,0,0,0};

            var expected = new[]{
                new []{ 3, 1, 6, 2, 4, 5 },
                new []{ 4, 2, 5, 3, 1, 6 },
                new []{ 2, 5, 4, 6, 3, 1 },
                new []{ 5, 6, 3, 1, 2, 4 },
                new []{ 1, 4, 2, 5, 6, 3 },
                new []{ 6, 3, 1, 4, 5, 2 }
            };

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests.ErrorMessage(expected, actual, orderedClues));
        }

        [Test]
        public void SolvePuzzle5()
        {
            var clues = new[] { 1,3,2,3,3,3,5,4,1,2,3,4,4,3,2,5,1,5,2,2,2,2,3,1};

            var expected = new[]{
                new []{ 3, 1, 6, 2, 4, 5 },
                new []{ 4, 2, 5, 3, 1, 6 },
                new []{ 2, 5, 4, 6, 3, 1 },
                new []{ 5, 6, 3, 1, 2, 4 },
                new []{ 1, 4, 2, 5, 6, 3 },
                new []{ 6, 3, 1, 4, 5, 2 }
            };

            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, Skyscrapers.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests.ErrorMessage(expected, actual, orderedClues));
        }
    }

}
