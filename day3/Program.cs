using System.Text;

internal class Program
{
    private record BoundingBox(int StartX, int StartY, int EndX, int EndY) : IComparable {
        public int CompareTo(object? obj) {
            var comparedTo = obj as BoundingBox;
            if(StartX < comparedTo.StartX && StartY < comparedTo.StartY) return -1;
            if(StartX == comparedTo.StartX && StartY == comparedTo.StartY && EndX == comparedTo.EndX && EndY == comparedTo.EndY) return 0;
            return 1;
        }

        public bool Intersects(BoundingBox target) => !(EndX < target.StartX || StartX > target.EndX || EndY < target.StartY || StartY > target.EndY);
    }

    private static void Main(string[] args)
    {
        PartOne();
        PartTwo();

        void PartOne()
        {
            var board = File.ReadAllLines("./input-demo.txt");
            var partNumbers = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                var currentNum = new StringBuilder();
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (char.IsDigit(board[i][j]))
                    {
                        currentNum.Append(board[i][j]);
                        if (j < board[i].Length - 1)
                        {
                            continue;
                        }
                    }
                    if (currentNum.Length > 0)
                    {
                        if (IsPartNumber(board, i, j - 1, currentNum.Length))
                        {
                            partNumbers.Add(int.Parse(currentNum.ToString()));
                        }
                        currentNum.Clear();
                    }
                }
            }
            Console.WriteLine($"Part 1: The total of all part numbers is {partNumbers.Sum()}");
        }

        bool IsPartNumber(string[] board, int line, int index, int length)
        {
            var (lineToCheck, lines) = (line - 1) switch
            {
                < 0 => (0, 2),
                _ => (line - 1, 3),
            };

            if (lineToCheck + lines > board.Length)
            {
                lines = 1;
            }

            for (int i = lineToCheck; i < lineToCheck + lines; i++)
            {
                if (LineHasPartIdentifier(board, i, index, length))
                {
                    return true;
                }
            }
            return false;
        }

        bool LineHasPartIdentifier(string[] board, int lineToCheck, int index, int length)
        {
            var startIndex = index - length;
            if (index - length < 0)
            {
                startIndex = 0;
            }
            var endIndex = index + 1;
            Console.WriteLine($"Line: {lineToCheck}");
            if (endIndex >= board[lineToCheck].Length)
            {
                endIndex = board[lineToCheck].Length;
            }
            for (int i = startIndex; i <= endIndex; i++)
            {
                var currentChar = board[lineToCheck][i];
                if (char.IsDigit(currentChar) || currentChar == '.')
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        BoundingBox? GetGearBoundingBox(string [] board, int lineToCheck, int index, int length) {
            var startIndex = index - length;
            if (index - length < 0)
            {
                startIndex = 0;
            }
            var endIndex = index + 1;
            if (endIndex >= board[lineToCheck].Length)
            {
                endIndex = board[lineToCheck].Length;
            }
            for (int i = startIndex; i <= endIndex; i++)
            {
                var currentChar = board[lineToCheck][i];
                if (currentChar != '*')
                {
                    continue;
                }
                Console.WriteLine($"Found gear at X {lineToCheck} and Y {i}");
                return new(
                    StartX: lineToCheck - 1 < 0 ? 0 : lineToCheck - 1,
                    StartY: i - 1 < 0 ? 0 : i - 1,
                    EndX: lineToCheck + 1 > board[i].Length ? board[i].Length : lineToCheck + 1,
                    EndY: i + 1 > board.Length ? board.Length : i + 1
                );
            }
            return null;
        }

        BoundingBox GetPartNumberGear(string[] board, int line, int index, int length) {
            var (lineToCheck, lines) = (line - 1) switch
            {
                < 0 => (0, 2),
                _ => (line - 1, 3),
            };

            if (lineToCheck + lines > board.Length)
            {
                lines = 1;
            }

            for (int i = lineToCheck; i < lineToCheck + lines; i++)
            {
                BoundingBox? box = GetGearBoundingBox(board, i, index, length);
                if (box is not null)
                {
                    return box;
                }
            }
            return null;
        }


        void PartTwo()
        {
            var board = File.ReadAllLines("./input.txt");
            var partNumbers = new List<(int value, BoundingBox box)>();
            var totals = new List<int>();
            var gears = new List<BoundingBox>();
            for (int i = 0; i < board.Length; i++)
            {
                var currentNum = new StringBuilder();
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (char.IsDigit(board[i][j]))
                    {
                        currentNum.Append(board[i][j]);
                        if (j < board[i].Length - 1)
                        {
                            continue;
                        }
                    }
                    if (currentNum.Length > 0)
                    {
                        var gearBox = GetPartNumberGear(board, i, j - 1, currentNum.Length);
                        if (gearBox is not null)
                        {
                            Console.WriteLine("Adding gear");
                            if(!gears.Contains(gearBox))
                                gears.Add(gearBox);
                            partNumbers.Add((int.Parse(currentNum.ToString()), new (
                                StartX: i < 0 ? 0 : i,
                                StartY: j - currentNum.Length < 0 ? 0 : j - currentNum.Length,
                                EndX: i > board.Length ? board.Length : i,
                                EndY: j - 1 > board[i].Length ? board[i].Length : j - 1 
                                )));
                        }
                        currentNum.Clear();
                    }
                }
            }
            Console.WriteLine($"Found {gears.Count} gears.");
            Console.WriteLine($"Found {partNumbers.Count} parts.");
            foreach(var gear in gears) {
                var intersectingParts = partNumbers.Where(part =>{
                    if(part.box.StartX > gear.EndX || part.box.StartX < gear.StartX) return false;
                    Console.WriteLine($"Part bounding box {part.box}");
                    Console.WriteLine($"Gear bounding box {gear}");
                    if(gear.Intersects(part.box)) Console.WriteLine($"Intersects {part.value}");
                    return gear.Intersects(part.box);
                }).ToList();
                if(intersectingParts.Count != 2) continue;
                totals.Add(intersectingParts.Aggregate(1, (x, y) => x * y.value));
                Console.WriteLine($"Found {intersectingParts}");
            }
            Console.WriteLine($"The total of all part numbers is {totals.Sum()}");
        }

    }
}