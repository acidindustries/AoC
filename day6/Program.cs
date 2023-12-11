internal class Program {

    public const string Filename = "./input.txt";
    private static void Main(string[] args) {
        
        var races = new List<Race>();
        var race = new Race();
        PartOne();
        PartTwo();


        
        void PartOne() {
            ParseRaces();
            var total = 1;
            foreach(var race in races) {
                total *= GetWinningCombinations(race);
            }
            Console.WriteLine($"The answer for part one is {total}");
        }

        void PartTwo() {
            ParseRace();
            var quad = GetQuadraticRoots(race);
            Console.WriteLine($"The answer for part two is {(int)(quad.UpperBound - quad.LowerBound)}.");
        }

        int GetWinningCombinations(Race race) {
            var quad = GetQuadraticRoots(race);
            return Enumerable.Range((int)quad.LowerBound, (int)quad.UpperBound + 1).Where(x => race.Time * x - Math.Pow(x, 2) > race.Distance).Count();
        }

        (double LowerBound, double UpperBound) GetQuadraticRoots(Race race) {
            long a = -1, b = race.Time, c = race.Distance * -1;
            var lowerBound = ((b * -1) + Math.Sqrt(Math.Pow(b, 2) - (4 * a * c))) / 2 * a;
            var upperBound = ((b * -1) - Math.Sqrt(Math.Pow(b, 2) - (4 * a * c))) / 2 * a;
            return (lowerBound, upperBound);
        }

        void ParseRaces() {
            foreach(var line in File.ReadLines(Filename)) {
                (string identifier, string[] data) =  line.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) switch { var a => (a[0], a[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) };
                switch(identifier) {
                    case "Time": {
                        for(var i = 0; i < data.Length; i++) {
                            if(!races.Any(race => race.Index == i)) {
                                races.Add(new Race() {
                                    Index = i,
                                    Time = int.Parse(data[i]),
                                });
                                continue;
                            }
                            races[i].Time = int.Parse(data[i]);
                        }
                        break;
                    }
                    case "Distance": {
                        for(var i = 0; i < data.Length; i++) {
                            if(!races.Any(race => race.Index == i)) {
                                races.Add(new Race() {
                                    Index = i,
                                    Distance = int.Parse(data[i]),
                                });
                                continue;
                            }
                            races[i].Distance = int.Parse(data[i]);
                        }
                        break;
                    }
                }

            }
        }

        void ParseRace() {
            foreach(var line in File.ReadLines(Filename)) {
                (string identifier, string data) =  line.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) switch { var a => (a[0], string.Join("", a[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))) };
                switch(identifier) {
                    case "Time": {
                        race.Time = int.Parse(data);
                        break;
                    }
                    case "Distance": {
                        race.Distance = long.Parse(data);
                        break;
                    }
                }

            }
        }
    }
}

public record Race {
    public int Index {get; set;}
    public int Time { get; set; }
    public long Distance { get; set; }
}