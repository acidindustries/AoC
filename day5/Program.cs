using System.Text.RegularExpressions;

internal class Program {

    private static void Main(string[] args) {
        var instances = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(MapInterface).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance);

            Dictionary<string, MapInterface> maps = instances.ToDictionary(x => ((MapInterface)x).MapIdentifier, x => (MapInterface)x);

        var seeds = new List<Seed>();

        ParseFile();
        PartOne();
        seeds.Clear();
        PartTwo();

        
        void PartOne() {
            ParseSeeds();
            MapInterface currentMap = maps[$"seed-to-soil"];
            foreach(var seed in seeds) {
                var value = seed.Identifier;
                while(currentMap.Destination != "location") {
                    value = currentMap.GetDestination(value);
                    currentMap = maps.Values.FirstOrDefault(map => map.Source == currentMap.Destination) ?? throw new Exception($"Could not find map with source {currentMap.Destination}");
                }
                seed.Location = currentMap.GetDestination(value);
                currentMap = maps[$"seed-to-soil"];
                Console.WriteLine($"Seed {seed.Identifier} has location {seed.Location}");
            }

            Console.WriteLine($"Nearest location is {seeds.Min(seed => seed.Location)}");
        }

        void PartTwo() {
            ParseSeedsAsRange();
            MapInterface currentMap = maps[$"seed-to-soil"];
            foreach(var seed in seeds) {
                var value = seed.Identifier;
                while(currentMap.Destination != "location") {
                    value = currentMap.GetDestination(value);
                    currentMap = maps.Values.FirstOrDefault(map => map.Source == currentMap.Destination) ?? throw new Exception($"Could not find map with source {currentMap.Destination}");
                }
                seed.Location = currentMap.GetDestination(value);
                currentMap = maps[$"seed-to-soil"];
                Console.WriteLine($"Seed {seed.Identifier} has location {seed.Location}");
            }

            Console.WriteLine($"Nearest location is {seeds.Min(seed => seed.Location)}");
        }

        void ParseSeeds() {
            foreach(var line in File.ReadLines("./input-demo.txt")) {
                if(line.Contains("seeds:")) {
                    foreach(var seedNumber in line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(1).Select(long.Parse)) {
                        seeds.Add(new Seed(seedNumber));
                    }
                    return;
                }
            }
        }

        void ParseSeedsAsRange() {
            foreach(var line in File.ReadLines("./input-demo.txt")) {
                if(line.Contains("seeds:")) {
                    var seedRanges = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(1).Select(long.Parse).ToArray();
                    
                    for(var i = 0; i < seedRanges.Length; i += 2) {
                        Console.WriteLine($"{seedRanges[i]} generate {seedRanges[i + 1]}");
                        seeds.Add(new Seed(seedRanges[i]));
                        // for(long j = seedRanges[i]; j < seedRanges[i] + seedRanges[i + 1]; j++) {
                        //     seeds.Add(new Seed(j));
                        // }
                    }
                    Console.WriteLine($"Seeds: {string.Join(",", seeds.Select(x=>x.Identifier))}");
                }
            }
        }

        void ParseFile() {
            var bundleName = string.Empty;
            var bundle = new List<MapEntry>();
            var bundleNamePattern = @$"(?<bundleName>[a-zA-X\-]+)( map)?:";
            foreach(var line in File.ReadLines("./input-demo.txt")) {
                switch(line) {
                    case string a when a.Contains("seeds:"): {
                        foreach(var seedNumber in line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(1).Select(long.Parse)) {
                            seeds.Add(new Seed(seedNumber));
                        }
                        break;
                    }
                    case string a when a.Contains(":"): {
                        bundleName = Regex.Match(line, bundleNamePattern).Groups["bundleName"].Value;
                        break;
                    }
                    case string a when string.IsNullOrEmpty(a): {
                        if(string.IsNullOrEmpty(bundleName) || bundle.Count == 0) break;
                        DispatchBundle(bundleName, [.. bundle]);
                        bundleName = string.Empty;
                        bundle.Clear();
                        break;
                    }
                    default: {
                        bundle.Add(
                            line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(long.Parse)
                            .ToArray()
                        );
                        break;
                    }
                }
            }

            // Make sure we send last bundle if any.
            if(string.IsNullOrEmpty(bundleName) || bundle.Count == 0) return;
            DispatchBundle(bundleName, [.. bundle]);
            bundleName = string.Empty;
            bundle.Clear();
        }

        void DispatchBundle(string bundleName, MapEntry[] bundle) {
            maps[bundleName].MapEntries = bundle;
        }
    }
}

public record MapEntry(long Destination, long Source, long Range) {
    public static implicit operator MapEntry(long[] values) {
        if(values.Length < 3) throw new Exception("Invalid array to convert to MapEntry.");
        return new MapEntry(values[0], values[1], values[2]);
    }

    public (long Min, long Max) GetSourceRange() => (Source, Source + Range);
    public (long Min, long Max) GetDestinationRange() => (Destination, Destination + Range);

    public bool ContainsValue(long value, bool checkDestination = false) {
        var range = checkDestination ? GetDestinationRange() : GetSourceRange();
        return value >= range.Min && value <= range.Max;
    }
}

public abstract class MapInterface {
    public MapEntry[] MapEntries {get; set;}
    public abstract string Source {get;}
    public abstract string Destination {get;}
    public string MapIdentifier {
        get {
            if(string.IsNullOrEmpty(Destination)) return Source;
            if(string.IsNullOrEmpty(Source)) return Destination;
            return $"{Source}-to-{Destination}";
        }
    }

    public long GetDestination(long source) {
        // Console.Write($"{Source} value {source} maps to {Destination} value ");
        foreach(var entry in MapEntries){
            (long position, bool _)= (source, source >= entry.Source && source <= entry.Source + entry.Range) 
                switch { 
                    (_, true) => (source - entry.Source, true), 
                    _ => (-1, false) 
            };

            if(position == -1) continue;
            return entry.Destination + position;
        }
        return source;
    }
}

public class Seed(long identifier) {
    public long Identifier { get; init; } = identifier;
    public long Location {get; set;}
}

public class Seed2SoilMap : MapInterface {

    public override string Source => "seed";

    public override string Destination => "soil";
}

public class Soil2FertilizerMap : MapInterface {
    public override string Source => "soil";

    public override string Destination => "fertilizer";
}

public class Fertilizer2WaterMap : MapInterface {
    public override string Source => "fertilizer";

    public override string Destination => "water";
}

public class Water2LightMap : MapInterface {
    public override string Source => "water";

    public override string Destination => "light";
}

public class Light2TemperatureMap : MapInterface {
    public override string Source => "light";

    public override string Destination => "temperature";
}

public class Temperature2HumidityMap : MapInterface {
    public override string Source => "temperature";

    public override string Destination => "humidity";
}

public class Humidity2LocationMap : MapInterface {
    public override string Source => "humidity";

    public override string Destination => "location";
}

