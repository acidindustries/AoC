using System;
using System.Text.RegularExpressions;

PartOne();
PartTwo();

void PartOne() {
    var winningCombo = new Dictionary<string, int>() {
        {"red", 12},
        {"green", 13},
        {"blue", 14}
    };
    var gameResults = new List<(int Game, bool Winning)>();
    foreach(var line in File.ReadLines("./input.txt")) {
        var (game, draws) = line.Split(":") switch {
            var a when a.Length == 2 => (int.Parse(a[0].Split(" ")[1]), a[1]),
        };
        var currentDraw = new Dictionary<string, int>();
        foreach(var draw in draws.Split(";")) {
            string pattern = @"(?<digit>\d+) (?<color>\w+)";
            foreach (Match m in Regex.Matches(draw, pattern)) {
                if(!currentDraw.ContainsKey(m.Groups["color"].Value)) {
                    currentDraw[m.Groups["color"].Value] = int.Parse(m.Groups["digit"].Value);
                    continue;
                }
                if(currentDraw[m.Groups["color"].Value] < int.Parse(m.Groups["digit"].Value)) {
                    currentDraw[m.Groups["color"].Value] = int.Parse(m.Groups["digit"].Value);
                    continue;
                }
            }

        }
        var winning = true;
        foreach(var combo in winningCombo) {
            if(combo.Value < currentDraw[combo.Key]) {
                winning = false;
                break;
            }
        }
        gameResults.Add((game, winning));
    }
    Console.WriteLine($"Total of winning games is {gameResults.Where(game => game.Winning).Sum(game => game.Game)}");
}

void PartTwo() {
    var gameResults = new List<(int Game, int Value)>();
    foreach(var line in File.ReadLines("./input.txt")) {
        var (game, draws) = line.Split(":") switch {
            var a when a.Length == 2 => (int.Parse(a[0].Split(" ")[1]), a[1]),
        };
        var currentDraw = new Dictionary<string, int>();
        foreach(var draw in draws.Split(";")) {
            string pattern = @"(?<digit>\d+) (?<color>\w+)";
            foreach (Match m in Regex.Matches(draw, pattern)) {
                if(!currentDraw.ContainsKey(m.Groups["color"].Value)) {
                    currentDraw[m.Groups["color"].Value] = int.Parse(m.Groups["digit"].Value);
                    continue;
                }
                else if(currentDraw[m.Groups["color"].Value] < int.Parse(m.Groups["digit"].Value)) {
                    currentDraw[m.Groups["color"].Value] = int.Parse(m.Groups["digit"].Value);
                    continue;
                }
            }
        }
        
        Console.WriteLine(currentDraw.Values.Aggregate(1, (x, y) => x * y));
        Console.WriteLine($"Current draw : {string.Join(",", currentDraw)}");
        gameResults.Add((game, currentDraw.Values.Aggregate(1, (x, y) => x * y)));
    }
    Console.WriteLine($"Total of winning games is {gameResults.Sum(game => game.Value)}");
}