 


// Test input:
// #############
// #...........#
//  01234567890
// ###B#C#B#D###
//   #A#D#C#A#
//   #########


using System.Diagnostics;

var input = new[] {
    new[] { 'A', 'B' }, // down -> up
    new[] { 'D', 'C' },
    new[] { 'C', 'B' },
    new[] { 'A', 'D' }
};


// #############
// #...........#
// ###C#A#D#D###
//   #B#A#B#C#
//   #########
char[][] inputFull = new[] {
    new[] { 'B', 'C' }, // down -> up
    new[] { 'A', 'A' },
    new[] { 'B', 'D' },
    new[] { 'C', 'D' }
};


Stopwatch sw = Stopwatch.StartNew();
// processed Burrow hash - best score achieved
Dictionary<String, int> processedHashes = new();

var initialBurrow = new Burrow(inputFull);
Burrow? bestBurrow = null;


PerformNextMove(initialBurrow);

Console.WriteLine();
Console.WriteLine($"The best score: {bestBurrow.EnergyCost}");
Console.WriteLine($"Time taken: {sw.Elapsed.TotalSeconds}s");
// Replay:
var replayBurrow = initialBurrow;
replayBurrow.Print();
foreach (var action in bestBurrow.History)
{
    Console.WriteLine(action);
    replayBurrow = replayBurrow.ExecuteAction(action);
    replayBurrow.Print();
}




void PerformNextMove(Burrow burrow, int depth = 1)
{
    // log
    if (burrow.Id % 1_000_000 == 0)
        Console.WriteLine($"iteration {burrow.Id}, best cost so far: {bestBurrow?.EnergyCost}");
    
    
    //burrow.Print();
    if (burrow.IsBurrowDone)
    {
        //Console.WriteLine("Success!");
        if (bestBurrow == null || burrow.EnergyCost < bestBurrow.EnergyCost)
            bestBurrow = burrow;
        return;
    }

    if (bestBurrow != null && burrow.EnergyCost >= bestBurrow.EnergyCost)
    {
        //Console.WriteLine("too expensive - skipping further effort");
        return;
    }

    var hash = burrow.GenerateHash();
    int bestCostForthisHashSoFar = processedHashes.ContainsKey(hash)
        ? processedHashes[hash]
        : Int32.MaxValue;
    if (bestCostForthisHashSoFar <= burrow.EnergyCost)
    {
        //Console.WriteLine("We already analyzed this case - and it was faster");
        return;
    }
    processedHashes[hash] = burrow.EnergyCost;

    var actions = burrow.GeneratePossibleActions();
    //int bestCostInThisBranch = Int32.MaxValue;;
    foreach (var action in actions)
    {
        var newBurrow = burrow.ExecuteAction(action);
        //Console.WriteLine($"{burrow.Id} => {newBurrow.Id}, depth: {depth}");
        PerformNextMove(newBurrow, depth+1);
    }
}


public class Burrow
{
    public static int Count = 1;
//    public readonly Shrimp[] AllShrimps;
    public readonly int Id;
    
    public readonly Shrimp?[] CorridorPlaces = new Shrimp[11]; // 11 nulls in beginning

    public readonly List<List<Shrimp>> Rooms = new();

    public readonly List<Action> History = new();
    public int EnergyCost = 0;


    public Burrow(char[][] input)
    {
        Id = Count++;
        for (int i = 0; i < 4; i++)
        {
            var roomShrimps = new List<Shrimp>();
            for (int j = 0; j < 2; j++)
            {
                roomShrimps.Add(new Shrimp(input[i][j]));
            }
            Rooms.Add(roomShrimps);
        }
//        AllShrimps = Rooms.SelectMany(r => r).ToArray();
    }

    // Cloning ctor
    private Burrow(Burrow input)
    {
        Id = Count++;
//        AllShrimps = input.AllShrimps;  // no need to clone
        CorridorPlaces = (Shrimp[])input.CorridorPlaces.Clone(); // clone
        History = new List<Action>(input.History);
        EnergyCost = input.EnergyCost;
        
        Rooms = new List<List<Shrimp>>();
        Rooms.Add(new List<Shrimp>(input.Rooms[0]));
        Rooms.Add(new List<Shrimp>(input.Rooms[1]));
        Rooms.Add(new List<Shrimp>(input.Rooms[2]));
        Rooms.Add(new List<Shrimp>(input.Rooms[3]));
    }

    public IEnumerable<Shrimp> DoneShrimps 
    {
        get
        {
            var dones = new List<Shrimp>();
            dones.AddRange(GetDoneResindentsInRoom(Rooms[0], Shrimp.A));
            dones.AddRange(GetDoneResindentsInRoom(Rooms[1], Shrimp.B));
            dones.AddRange(GetDoneResindentsInRoom(Rooms[2], Shrimp.C));
            dones.AddRange(GetDoneResindentsInRoom(Rooms[3], Shrimp.D));
            return dones;
        }
    }

    private IEnumerable<Shrimp> GetDoneResindentsInRoom(List<Shrimp> roomResidents, char roomClass)
    {
        if (roomResidents.Count > 0 && roomResidents[0].Name == roomClass)
        {
            yield return roomResidents[0];
            if (roomResidents.Count > 1 && roomResidents[1].Name == roomClass)
                yield return roomResidents[1];
        }
    }

    public IEnumerable<Shrimp> ShrimpsInCorridors => CorridorPlaces.Where(p => p != null);
    public IEnumerable<Shrimp> OutermostShrimpsInRooms => Rooms.Where(r => r.Any()).Select(s => s.Last());

    public IEnumerable<Shrimp> PotentiallyActionableShrimps => OutermostShrimpsInRooms.Union(ShrimpsInCorridors).Except(DoneShrimps);

    public bool IsBurrowDone => DoneShrimps.Count() == 8;
    public static int[] HabitableCorridorPositions = new[] { 0, 1, 3, 5, 7, 9, 10 };

    public List<Action> GeneratePossibleActions()
    {
        List<Action> actions = new();
        foreach (var shrimp in PotentiallyActionableShrimps)
        {
            if (IsShrimpInRoom(shrimp))
            {
                // generate moves into corridor
                var initialPosition = GetRoomedShrimpPosition(shrimp);
                foreach (var targetPosition in HabitableCorridorPositions)
                {
                    if (CanShrimpGoInCorridor(shrimp, initialPosition, targetPosition))
                    {
                        int stepsInCorridor = Math.Abs(initialPosition - targetPosition);
                        int stepsToLeaveRoom = CountStepsToLeaveRoom(shrimp);
                        actions.Add(new GoToCorridor()
                        {
                            Shrimp = shrimp,
                            PositionInCorridor = targetPosition,
                            EnergyPrice = (stepsInCorridor + stepsToLeaveRoom) * shrimp.StepCost
                        });
                    }
                }
            }
            else
            {
                // generate moves into room
                var initialPosition = GetCorridorShrimpPosition(shrimp);
                var targetPosition = GetTargetRoomPositionFor(shrimp);
                if (RoomCanAcceptItsShrimp(shrimp) &&
                    CanShrimpGoInCorridor(shrimp, initialPosition, targetPosition))
                {
                    int stepsInCorridor = Math.Abs(initialPosition - targetPosition);
                    var targetRoomIndex = GetTargetRoomIndex(shrimp);
                    int stepsToEnterRoom = CountStepsToEnterRoom(targetRoomIndex);
                    actions.Add(new GoToRoom()
                    {
                        Shrimp = shrimp,
                        TargetRoomIndex = targetRoomIndex,
                        EnergyPrice = (stepsInCorridor + stepsToEnterRoom) * shrimp.StepCost
                    });
                }
            }
        }

        return actions;
    }

    private bool RoomCanAcceptItsShrimp(Shrimp shrimp)
    {
        var roomIndex = GetTargetRoomIndex(shrimp);
        var alienCount = Rooms[roomIndex].Count(sh => sh.Name != shrimp.Name);
        return Rooms[roomIndex].Count < 2 && alienCount == 0;

    }

    private bool CanShrimpGoInCorridor(Shrimp shrimp, int initialPosition, int targetPosition)
    {
        var from = Math.Min(initialPosition, targetPosition);
        var to = Math.Max(initialPosition, targetPosition);
        return CorridorPlaces[from..to].Count(cell => cell != null && cell != shrimp) == 0;
    }

    private int CountStepsToLeaveRoom(Shrimp shrimp)
    {
        var roomIndex = GetCurrentRoomIndex(shrimp);
        var indexInRoom = Rooms[roomIndex].FindIndex(sh => sh == shrimp);
        return 2 - indexInRoom;
    }

    private int CountStepsToEnterRoom(int roomIndex)
    {
        return 2 - Rooms[roomIndex].Count;
    }

    // from which corridor positions does one enter this room?
    private int GetRoomedShrimpPosition(Shrimp shrimp)
    {
        var roomIndex = GetCurrentRoomIndex(shrimp);
        return roomIndex switch
        {
            0 => 2,
            1 => 4,
            2 => 6,
            3 => 8
        };
    }

    private int GetCorridorShrimpPosition(Shrimp shrimp)
    {
        for (int i = 0; i < CorridorPlaces.Length; i++)
        {
            if (CorridorPlaces[i] == shrimp)
                return i;
        }

        throw new Exception("This shrimp is not in the corridor!");
    }

    private int GetTargetRoomPositionFor(Shrimp shrimp)
    {
        return shrimp.Name switch
        {
            Shrimp.A => 2,
            Shrimp.B => 4,
            Shrimp.C => 6,
            Shrimp.D => 8
        };
    }
    // assuming the shrimp is in a room, what room index is that?

    private int GetTargetRoomIndex(Shrimp shrimp)
    {
        return shrimp.Name switch
        {
            Shrimp.A => 0,
            Shrimp.B => 1,
            Shrimp.C => 2,
            Shrimp.D => 3
        };
    }
    private int GetCurrentRoomIndex(Shrimp shrimp)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Rooms[i].Contains(shrimp))
                return i;
        }

        throw new Exception("This shrimp is NOT in any room!");
    }

    public bool IsShrimpInRoom(Shrimp s) => CorridorPlaces.Contains(s) == false;

    public Burrow Clone()
    {
        return new Burrow(this);
    }
    
    public Burrow ExecuteAction(Action action)
    {
        var burrow = Clone();
        burrow.ExecuteActionMutating(action);
        return burrow;
    }
    
    private void ExecuteActionMutating(Action action)
    {
        History.Add(action);
        EnergyCost += action.EnergyPrice;
        if (action is GoToCorridor goToCorridor)
        {
            ExecuteGoToCorridor(goToCorridor);
        }
        else
        {
            ExecuteGoToRoom((GoToRoom)action);
        }
    }

    private void ExecuteGoToCorridor(GoToCorridor action)
    {
        CorridorPlaces[action.PositionInCorridor] = action.Shrimp;
        var roomIndex = GetCurrentRoomIndex(action.Shrimp);
        Rooms[roomIndex].Remove(action.Shrimp);
    }

    private void ExecuteGoToRoom(GoToRoom action)
    {
        for (int i = 0; i < CorridorPlaces.Length; i++)
        {
            if (CorridorPlaces[i] == action.Shrimp)
            {
                CorridorPlaces[i] = null;
                break;
            }
        }
        Rooms[action.TargetRoomIndex].Add(action.Shrimp);
    }

// #############
// #...........#
// ###C#A#D#D###
//   #B#A#B#C#
//   #########
    public void Print()
    {
        var corridorLine = String.Join(null, CorridorPlaces.Select(p => p?.Name ?? '.'));
        
        Console.WriteLine($"#############");
        Console.WriteLine($"#{corridorLine}#");
        Console.WriteLine($"###{room(0,1)}#{room(1,1)}#{room(2,1)}#{room(3,1)}###");
        Console.WriteLine($"  #{room(0,0)}#{room(1,0)}#{room(2,0)}#{room(3,0)}#  ");
        Console.WriteLine($"  #########  ");

        char room(int roomIndex, int indexInRoom)
        {
            var room = Rooms[roomIndex];
            if (room.Count > indexInRoom)
                return room[indexInRoom].Name;
            return '.';
        }
    }

    public string GenerateHash()
    {
        var corridorLine = String.Join(null, CorridorPlaces.Select(p => p?.Name ?? '.'));
        foreach (var room in Rooms)
        {
            if (room.Count == 0)
                corridorLine += "..";
            else if (room.Count == 1)
                corridorLine += room[0].Name + ".";
            else
                corridorLine += room[0].Name + room[1].Name;
        }

        return corridorLine;
    }
}

public record Shrimp
{
    public static int Counter = 1;
    
    public const char A = 'A';
    public const char B = 'B';
    public const char C = 'C';
    public const char D = 'D';

    
    public int Id { get; }
    public char Name { get; init; }
    
    public Shrimp(char name)
    {
        Id = Counter++;
        Name = name;
    }

    public int StepCost => Name switch
    {
        A => 1,
        B => 10,
        C => 100,
        D => 1000
    };
}

public class Action
{
    public Shrimp Shrimp { get; set; }
    public int EnergyPrice { get; set; }
}

public class GoToCorridor: Action
{
    //0..10
    public int PositionInCorridor { get; set; }

    public override string ToString()
    {
        return $"Shrimp {Shrimp.Name}{Shrimp.Id} => corridor {PositionInCorridor} [cost: {EnergyPrice}]";
    }
}

public class GoToRoom : Action
{
    // 0..3
    public int TargetRoomIndex { get; set; }
    
    public override string ToString()
    {
        return $"Shrimp {Shrimp.Name}{Shrimp.Id} => room {TargetRoomIndex}     [cost: {EnergyPrice}]";
    }
}