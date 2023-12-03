using System.Text;

PartOne();
PartTwo();

void PartOne() {
    var board = File.ReadAllLines("./input.txt");
    var partNumbers = new List<int>();
    for(int i = 0; i < board.Length; i++) {
        var currentNum = new StringBuilder();
        for(int j = 0; j < board[i].Length; j++) {
            if(char.IsDigit(board[i][j])) {
                currentNum.Append(board[i][j]);
                if(j < board[i].Length - 1) {
                    continue;
                }
            }
            if(currentNum.Length > 0) {
                if(IsPartNumber(board, i, j - 1, currentNum.Length)){
                    partNumbers.Add(int.Parse(currentNum.ToString()));
                }
                currentNum.Clear();
            }
        }
    }
    Console.WriteLine($"Numbers: {string.Join(",", partNumbers)}");
    Console.WriteLine($"The total of all part numbers is {partNumbers.Sum()}");
}

bool IsPartNumber(string[] board, int line, int index, int length)
{
    var (lineToCheck, lines) = (line - 1) switch {
        < 0 => (0, 2),
        _ => (line - 1, 3),
    };

    if(lineToCheck + lines > board.Length) {
        lines = 1;
    }

    for(int i = lineToCheck; i < lineToCheck + lines; i++) {
        if(LineHasPartIdentifier(board, i, index, length)) {
            return true;
        }
    }
    return false;
}

bool LineHasPartIdentifier(string[] board, int lineToCheck, int index, int length) {
    var startIndex = index - length;
    if(index - length < 0) {
        startIndex = 0;
    }
    var endIndex = index + 1;
    Console.WriteLine($"Line: {lineToCheck}");
    if(endIndex >= board[lineToCheck].Length) {
        endIndex = board[lineToCheck].Length;
    }
    for(int i = startIndex; i <= endIndex; i++) {
        var currentChar = board[lineToCheck][i]; 
        if(char.IsDigit(currentChar) || currentChar == '.') {
            continue;
        }
        return true;
    }
    return false;
}

void PartTwo() {

}