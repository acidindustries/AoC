internal class Program {

    private static void Main(string[] args) {

        Console.WriteLine("Hello, World!");
        PartOne();
        PartTwo();

        
        void PartOne() {

            var games = ParseGames();
            foreach(var game in games) {
                Console.WriteLine(game.Value.First().GetScore());
            }
            Console.WriteLine($"The total is {games.Sum(x => x.Value.First().GetScore())}");
        }

        void PartTwo() {
            var games = ParseGames();
            foreach(var game in games) {
                for(var i = 0; i< game.Value.Count; i++) {
                    var ticket = game.Value[i];
                    var winningNumbers = ticket.GetWinningNumbers();
                    Console.WriteLine($"Game {game.Key} has {winningNumbers} winning numbers.");
                    if(winningNumbers == 0) continue;
                    for(var j = game.Key + 1; j <= (game.Key + winningNumbers); j++) {
                        Console.WriteLine($"Adding new ticket to game #{j}");
                        games[j].Add(games[j].First());
                    }
                }
            }

            Console.WriteLine($"Jesus christ, you got {games.Sum(game => game.Value.Count())} scratch cards!");
        }

        Dictionary<int, List<Ticket>> ParseGames() {
            Dictionary<int, List<Ticket>> games = new();
            foreach(var card in File.ReadLines("./input.txt")) {
                var details = card.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var gameNumber = int.Parse(details[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]);
                if(!games.ContainsKey(gameNumber)) {
                    games[gameNumber] = new();
                }
                games[gameNumber].Add(new Ticket(details[1]));
            }
            return games;
        }
    }
}

public class Ticket {
    public int[] WinningNumbers {get; set;}
    public int[] CardNumbers {get; set;}

    public Ticket(string cardSummary) {
        var summary = cardSummary.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        WinningNumbers = summary[0].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        CardNumbers = summary[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
    }

    public int GetScore() {
        return WinningNumbers.Where(x => CardNumbers.Contains(x)).Aggregate(0, (x, y) => x == 0 ? 1 : x * 2);
    }

    public int GetWinningNumbers() {
        return WinningNumbers.Where(x => CardNumbers.Contains(x)).Count();
    }
}