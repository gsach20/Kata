using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Core.Internal;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class Skyscrapers
    {
        public const int Size = 7;

        private static bool _processingClues;

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

                if (!_processingClues) return Result.Changed;

                foreach (int laneIndex in GetLanesContainingCell(this))
                {
                    Lane lane = AllLanes[laneIndex];

                    if (lane.Clue == 0) continue;

                    for (var i = lane.Cells.Count - 1; i >= 0; i--)
                    {
                        Cell cell = lane.Cells[i];
                        if(cell.ValueSet == 0) break;
                        lane.Cells.RemoveAt(i);
                        if (!lane.Cells.Any()) lane.Clue = 0;
                        else if (cell.ValueSet >= lane.Cells.Select(c => c.PossibleValues.Max()).Max()) lane.Clue--;
                    }
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

        internal class Lane
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

        internal class Backup
        {
            public readonly Cell[][] _allCells;
            public readonly Lane[] _allLanes;

            public Backup()
            {
                _allCells = new Cell[Size][];
                for (var i = 0; i < Size; i++)
                {
                    _allCells[i] = new Cell[Size];
                    for (int j = 0; j < Size; j++)
                    {
                        _allCells[i][j] = new Cell(AllCells[i][j]);
                    }
                }

                _allLanes = new Lane[4*Size];
                for (var i = 0; i < _allLanes.Length; i++)
                {
                    _allLanes[i] = new Lane(AllLanes[i]);
                }
            }

            public void Restore()
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        AllCells[i][j].CopyFrom(_allCells[i][j]);
                    }
                }

                AllLanes = _allLanes;

                AllLanes = new Lane[4*Size];
                for (var i = 0; i < _allLanes.Length; i++)
                {
                    AllLanes[i] = new Lane(_allLanes[i]);
                }
            }
        }

        public static int[][] Expected { get; set; }

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

            _processingClues = true;
            if(Result.Failed == (ProcessClues() & Result.Failed)) return null;
            //_processingClues = false;

            if (!PuzzleSolved())
            {
                FindChoicesAndEliminate(clues);
            }

            int[][] grid = new int[Size][];
            for (var rowIndex = 0; rowIndex < Size; rowIndex++)
            {
                grid[rowIndex] = new int[Size];
                for (var colIndex = 0; colIndex < Size; colIndex++)
                {
                    grid[rowIndex][colIndex] = AllCells[rowIndex][colIndex].PossibleValues.Aggregate((a, b) => a + b);
                }
            }

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

                bool puzzleSolved = PuzzleSolved();
                if (!puzzleSolved)
                {
                    if (Result.Failed == (ProcessClues() & Result.Failed))
                    {
                        backup.Restore();
                        continue;
                    }
                    puzzleSolved = PuzzleSolved();
                }

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
            SortedDictionary<int, Tuple<int, Cell[]>> options = new SortedDictionary<int, Tuple<int, Cell[]>>();
            foreach (Lane lane in AllLanes)
            {
                foreach (int value in Enumerable.Range(1, Size))
                {
                    Cell[] possibleCells = lane.Cells.Where(c => c.PossibleValues.Contains(value)).ToArray();
                    if (possibleCells.Length == 2)
                    {
                        valueToEliminate = value;
                        return possibleCells.ToArray();
                    }

                    if (possibleCells.Length > 2 && !options.ContainsKey(possibleCells.Length))
                    {
                        options[possibleCells.Length] = new Tuple<int, Cell[]>(value, possibleCells);
                    }
                }
            }

            valueToEliminate = 0;
            if(!options.Any()) return new Cell[0];
            var firstOption = options.First();
            valueToEliminate = firstOption.Value.Item1;
            return firstOption.Value.Item2;
        }

        private static Result ProcessClues()
        {
            Result result = Result.Changed;
            while (result == Result.Changed)
            {
                //Process clues
                result = Result.NoChange;
                for (var index = 0; index < AllLanes.Length; index++)
                {
                    var lane = AllLanes[index];
                    result = ApplySimpleRules(lane) | result;
                    if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                }
            }

            result = Result.Changed;
            while (result == Result.Changed)
            {
                //Process clues
                result = Result.NoChange;
                for (var index = 0; index < AllLanes.Length; index++)
                {
                    var lane = AllLanes[index];
                    result = ApplySpecialRules(lane) | result;
                    if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                }
            }

            return result;
        }

        private static Result ApplySpecialRules(Lane lane)
        {
            if (lane.Cells.Count < 3) return Result.NoChange;
            int visible = 0;
            int highestHeight = 0;
            var i = 0;
            for (; i < lane.Cells.Count; i++)
            {
                Cell cell = lane.Cells[i];
                if (cell.ValueSet == 0) break;
                if (cell.ValueSet > highestHeight)
                {
                    highestHeight = cell.ValueSet;
                    visible++;
                }
            }

            int max = lane.Cells.Select(c => c.PossibleValues.Max()).Max();
            if (highestHeight == max) return Result.NoChange;

            int maxIndex = lane.Cells.FindIndex(i, c => c.ValueSet == max);

            Result result = Result.NoChange;

            if (lane.Clue - visible - 1 == maxIndex - i)
            {
                for (int j = i; j < maxIndex; j++)
                {
                    int[] valuesToBeRemoved = lane.Cells[j].PossibleValues.Where(p => p < highestHeight).ToArray();
                    foreach (var v in valuesToBeRemoved)
                    {
                        int cellsCount = lane.Cells.Count;
                        result = lane.Cells[j].RemoveValue(v) | result;
                        if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                        if (cellsCount != lane.Cells.Count) return result;
                    }
                }
            }

            return result;
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
            if (possibleValuesInLane.Count != size) return Result.Failed;
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
                        int cellCount = lane.Cells.Count;
                        result = cell.RemoveValue(possibleValuesInLane[indexToBeRemoved-1]) | result;
                        if (Result.Failed == (result & Result.Failed)) return Result.Failed;
                        if (cellCount != lane.Cells.Count) return result;
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

        public static string PrintValues(Backup backup = null)
        {
            Lane[] allLanes = AllLanes;
            Cell[][] allCells = AllCells;
            if (backup != null)
            {
                allLanes = backup._allLanes;
                allCells = backup._allCells;
            }

            Stack<Lane> orderedClues = new Stack<Lane>(Enumerable.Range(0, 4 * Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex)))
                .OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => allLanes[t.Item1]).Reverse());

            string values = Environment.NewLine
                            + "      " +
                            string.Join(string.Concat(Enumerable.Range(0,2*Size-2).Select(i => " ")),
                                Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) +
                            Environment.NewLine
                            + string.Join(Environment.NewLine,
                                allCells.Select(r =>
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

    }

    [Flags]
    internal enum Result
    {
        NoChange = 0,
        Changed = 1,
        Failed = 2
    }

    [TestFixture]
    public class SkyscrapersTests4by4
    {
        private const int Size = Skyscrapers.Size;

        public static int[] GetLaneIndices2(int clueIndex)
        {
            if (clueIndex < Skyscrapers.Size)
            {
                return new[] { -1, clueIndex };
            }
            if (clueIndex < 2 * Skyscrapers.Size)
            {
                return new[] { clueIndex - Skyscrapers.Size, 1 };
            }
            if (clueIndex < 3 * Skyscrapers.Size)
            {
                return new[] { Skyscrapers.Size, 3 * Skyscrapers.Size - clueIndex - 1 };
            }
            return new[] { 4 * Skyscrapers.Size - clueIndex - 1, 0 };
        }

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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                ErrorMessage(expected, actual, orderedClues)
            );
        }

        public static string ErrorMessage(int[][] expected, int[][] actual, Stack<int> orderedClues)
        {
            return Environment.NewLine
                   + String.Join(Environment.NewLine, expected.Select(r => String.Join("|", r)))
                   + Environment.NewLine + Environment.NewLine
                   + "   " + String.Join("  ", Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) + Environment.NewLine
                   + String.Join(Environment.NewLine, Skyscrapers.AllCells.Select(r => orderedClues.Pop() + " |" + String.Join("|", r.Select(c => c.PossibleValues.Count == 1  ? c.PossibleValues.First() == expected[c.X][c.Y] ? c.PossibleValues.First() + " " : c.PossibleValues.First() + "*" : " *")) + "| " + orderedClues.Pop())) + Environment.NewLine
                   + "   " + String.Join("  ", Enumerable.Range(0, Size).Select(i => orderedClues.Pop())) + Environment.NewLine
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

            return String.Empty;
        }
    }

    [TestFixture]
    public class SkyscrapersTests6by6
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests4by4.ErrorMessage(expected, actual, orderedClues));
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests4by4.ErrorMessage(expected, actual, orderedClues));
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests4by4.ErrorMessage(expected, actual, orderedClues));
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
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual,
                SkyscrapersTests4by4.ErrorMessage(expected, actual, orderedClues));
        }

        
    }

    [TestFixture]
    public class SkyscrapersTests7by7
    {
        static int [][] clues = new[]
        {
            new [] { 7, 0, 0, 0, 2, 2, 3,
                0, 0, 3, 0, 0, 0, 0,
                3, 0, 3, 0, 0, 5, 0,
                0, 0, 0, 0, 5, 0, 4 },
            new [] { 0, 2, 3, 0, 2, 0, 0,
                5, 0, 4, 5, 0, 4, 0,
                0, 4, 2, 0, 0, 0, 6,
                5, 2, 2, 2, 2, 4, 1 }
        };

        static int [][][] expected = new[]
        {
            new[] { new[] { 1, 5, 6, 7, 4, 3, 2 },
                new[] { 2, 7, 4, 5, 3, 1, 6 },
                new[] { 3, 4, 5, 6, 7, 2, 1 },
                new[] { 4, 6, 3, 1, 2, 7, 5 },
                new[] { 5, 3, 1, 2, 6, 4, 7 },
                new[] { 6, 2, 7, 3, 1, 5, 4 },
                new[] { 7, 1, 2, 4, 5, 6, 3 } },
            new[] { new[] { 7, 6, 2, 1, 5, 4, 3 },
                new[] { 1, 3, 5, 4, 2, 7, 6 },
                new[] { 6, 5, 4, 7, 3, 2, 1 },
                new[] { 5, 1, 7, 6, 4, 3, 2 },
                new[] { 4, 2, 1, 3, 7, 6, 5 },
                new[] { 3, 7, 6, 2, 1, 5, 4 },
                new[] { 2, 4, 3, 5, 6, 1, 7 } }
        };


        [Test]
        public void Test_1_Medium()
        {
            int index = 0;
            int[] clues1 = clues[index];
            int[][] expected1 = expected[index];
            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues1[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues1);
            CollectionAssert.AreEqual(expected1, actual,
                SkyscrapersTests4by4.ErrorMessage(expected1, actual, orderedClues));
        }

        [Test]
        public void Test_2_VeryHard()
        {
            int index = 1;
            int[] clues1 = clues[index];
            int[][] expected1 = expected[index];
            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues1[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues1);
            CollectionAssert.AreEqual(expected1, actual,
                SkyscrapersTests4by4.ErrorMessage(expected1, actual, orderedClues));
        }

        [Test]
        public void Test_3_VeryHard()
        {
            int index = 1;
            int[] clues1 = {6,4,0,2,0,0,3,0,3,3,3,0,0,4,0,5,0,5,0,2,0,0,0,0,4,0,0,3};
            int[][] expected1 = { 
                new[] { 2, 1, 6, 4, 3, 7, 5 },
                new[] { 3, 2, 5, 7, 4, 6, 1 },
                new[] { 4, 6, 7, 5, 1, 2, 3 },
                new[] { 1, 3, 2, 6, 7, 5, 4 },
                new[] { 5, 7, 1, 3, 2, 4, 6 },
                new[] { 6, 4, 3, 2, 5, 1, 7 },
                new[] { 7, 5, 4, 1, 6, 3, 2 } };
            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues1[t.Item1]).Reverse());

            var actual = Skyscrapers.SolvePuzzle(clues1);
            CollectionAssert.AreEqual(expected1, actual,
                SkyscrapersTests4by4.ErrorMessage(expected1, actual, orderedClues));
        }

        [Test]
        public void Test_4_VeryHard()
        {
            int index = 1;
            int[] clues1 = {0,0,0,5,0,0,3,0,6,3,4,0,0,0,3,0,0,0,2,4,0,2,6,2,2,2,0,0};
            int[][] expected1 = { 
                new[] { 3, 5, 6, 1, 7, 2, 4 },
                new[] { 7, 6, 5, 2, 4, 3, 1 },
                new[] { 2, 7, 1, 3, 6, 4, 5 },
                new[] { 4, 3, 7, 6, 1, 5, 2 },
                new[] { 6, 4, 2, 5, 3, 1, 7 },
                new[] { 1, 2, 3, 4, 5, 7, 6 },
                new[] { 5, 1, 4, 7, 2, 6, 3 } };
            Stack<int> orderedClues = new Stack<int>(Enumerable.Range(0, 4 * Skyscrapers.Size)
                .Select(clueIndex => new Tuple<int, int[]>(clueIndex, SkyscrapersTests4by4.GetLaneIndices2(clueIndex))).OrderBy(t => t.Item2[0])
                .ThenBy(t => t.Item2[1]).Select(t => clues1[t.Item1]).Reverse());
            Skyscrapers.Expected = expected1;
            var actual = Skyscrapers.SolvePuzzle(clues1);
            CollectionAssert.AreEqual(expected1, actual,
                SkyscrapersTests4by4.ErrorMessage(expected1, actual, orderedClues));
        }
    }
}
