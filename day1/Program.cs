PartTwo();

void PartOne() {
    var sum = File.ReadLines("./input.txt")
        .Select(line => new {
                Number = new Func<int>(() => {
                    return Convert.ToInt32($"{line.Where(char.IsDigit).First()}{line.Where(char.IsDigit).Last()}");
                })
            })
        .Sum(line => line.Number());
    Console.WriteLine($"The total is {sum}");
}

void PartTwo() {
    var digits = new Dictionary<string, int> {
        {"one", 1},
        {"two", 2},
        {"three", 3},
        {"four", 4},
        {"five", 5},
        {"six", 6},
        {"seven", 7},
        {"eight", 8},
        {"nine", 9},
        {"zero", 0},
    };

    var total = 0;
    foreach(var line in File.ReadLines("./input2.txt")) {
        var foundDigits = new List<int>();
        for(var i = 0; i < line.Length; i++) {
            if(char.IsDigit(line[i])) {
                // Console.WriteLine($"Found raw digit {line[i]}");
                foundDigits.Add(int.Parse($"{line[i]}"));
                continue;
            }
            foreach(var digit in digits) {
                if(i + digit.Key.Length > line.Length) continue;
                if(line.Substring(i, digit.Key.Length) == digit.Key) {
                    // Console.WriteLine($"Found {digit.Value} in {line.Substring(i, digit.Key.Length)}");
                    foundDigits.Add(digit.Value);
                }
            }
        }
        Console.WriteLine($"Found digits {string.Join("-", foundDigits)} in {line}");
        Console.WriteLine($"Adding {foundDigits.First()}{foundDigits.Last()} to the total.");
        total += Convert.ToInt32($"{foundDigits.First()}{foundDigits.Last()}");
    }

    Console.WriteLine($"The total is {total}");
}