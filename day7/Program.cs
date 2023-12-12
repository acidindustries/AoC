using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

internal class Program {

    public const string Filename = "./input.txt";
    private static void Main(string[] args) {
        var hands = new List<Hand>();
        PartOne();
        PartTwo();

        void PartOne() {
           ParseGames();
           string.Join(", ", hands);
        }

        void PartTwo() {
            
        }

        void ParseGames() {
            foreach(var line in File.ReadLines(Filename)) {
                (string hand, int bid) =  line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) switch { var a => (a[0], int.Parse(a[1])) };
                var cards = new List<Card>();
                foreach(var character in hand) {
                    var card = Card.Create(character);
                    Console.WriteLine(card.Value);
                    cards.Add(card);
                }
                var handObj = new Hand(cards, bid);
                Console.WriteLine(handObj.HandValue); 
                hands.Add(handObj);
            }
            
            hands.Sort();
            var currentValue = hands.Count;
            foreach(var handPlayed in hands) {
                Console.WriteLine($"{string.Join("", handPlayed.Cards.Select(x => x.Face))} {handPlayed.HandValue} Rank: {currentValue} Score: {(currentValue-- * handPlayed.Bid)}"); 
            }
            currentValue = hands.Count;
            Console.WriteLine($"Total winning for part one is {hands.Sum(hand => hand.Bid * currentValue--)}");
        }
    }
}

public class Hand : IComparable {
    public List<Card> Cards {get; init;}  = new();
    public int Bid {get; init;}
    public HandValue HandValue { 
        get {
            if(Cards.GroupBy(card => card.Face).Count() == 1) return HandValue.FiveOfAKind;
            if(Cards.GroupBy(card => card.Face).Count() == 2) {
                var groups = Cards.GroupBy(x => x.Face);
                if(groups.Any(group => group.Count() == 4)) return HandValue.FourOfAKind;
                return HandValue.FullHouse;
            }
            if(Cards.GroupBy(card => card.Face).Count() == 3) {
                var groups = Cards.GroupBy(x => x.Face);
                if(groups.Any(group => group.Count() == 3)) return HandValue.ThreeOfAKind;
                if(groups.Count(group => group.Count() == 2) == 2) return HandValue.TwoPair;
            }
            if(Cards.GroupBy(card => card.Face).Count() == 4) {
                var groups = Cards.GroupBy(x => x.Face);
                if(groups.Any(group => group.Count() == 1)) return HandValue.OnePair;
            }
            return HandValue.HighCard;
        }
        private set {
            HandValue = value;
        }
    }

    public Hand(List<Card> cards, int bid)
    {  
        Cards = cards;
        Bid = bid;
    }

    public int CompareTo(object? obj) {
        var hand = obj as Hand;
        if(hand is null) {
            return -1;
        }

        if(HandValue == hand.HandValue) {
            for(int i = 0; i < Cards.Count; i++) {
                if(hand.Cards[i].Value == Cards[i].Value) continue;
                return hand.Cards[i].Value.CompareTo(Cards[i].Value);
            }
            return 0;
        }

        return hand.HandValue.CompareTo(HandValue);
    }
}

public class Card {
    public char Face {get; private set;}
    public int Value {get; private set;}

    public static Card Create(char face) {
        var value = face switch {
            >= '2' and <= '9' => int.Parse(face.ToString()),
            'T' => 10,
            'J' => 11,
            'Q' => 12,
            'K' => 13,
            'A' => 14,
            _ => throw new Exception("Bad card.")
        };
        return new Card() {
            Face = face,
            Value = value
        };
    }
}

public enum HandValue {
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
}