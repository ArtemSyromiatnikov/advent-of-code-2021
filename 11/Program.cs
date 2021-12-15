using System;

var lines = System.IO.File.ReadAllLines("input1.txt");
const byte ROWS = 10;
const byte COLS = 10;
var map = new Octopus[ROWS, COLS];

for (byte i = 0; i < ROWS; i++)
for (byte j = 0; j < COLS; j++)
    map[i, j] = new Octopus(map, i, j, (byte)(lines[i][j] - 48)); // parse char digit as byte

Console.WriteLine($"Initial: ");
Output(map);

// Iterate over all octopuses one-by-one, row after row
// When one octopus flashes, immediately evaluate its neighbors (end their neighbors recursively)

const int DAYS = 10000;
int totalFlashes = 0;
for (int i = 0; i < DAYS; i++)
{
    foreach (var octopus in map)
        octopus.IncreaseEnergy();

    var flashes = AsEnumerable(map).Count(oct => oct.HasFlashed);
    totalFlashes += flashes;
    
    
    Console.WriteLine($"Day {i+1}:    {flashes} flashes");
    //Output(map);
    
    
    if (flashes == 100)     // Puzzle 2
        break;
    
    foreach (var octopus in map)
        octopus.Reset();
}
Console.WriteLine($"Total flashes: {totalFlashes}");

IEnumerable<Octopus> AsEnumerable(Octopus[,] map)
{
    foreach (var octopus in map)
    {
        yield return octopus;
    }
}


void Output(Octopus[,] map)
{
    for (int i = 0; i < ROWS; i++)
    {
        for (int j = 0; j < COLS; j++)
            Console.Write(map[i, j]);
        Console.WriteLine();
    }
}

public class Octopus
{
    private readonly Octopus[,] _board;
    
    public byte I { get; }
    public byte J { get; }
    
    public byte Energy { get; private set; }
    public bool HasFlashed => Energy > 9;

    public Octopus(Octopus[,] board, byte i, byte j, byte energy)
    {
        _board = board;
        I = i;
        J = j;
        Energy = energy;
    }

    /// <summary>
    /// Returns neighbors that have not yet flashed.
    /// </summary>
    public IEnumerable<Octopus> GetNeighbors()
    {
        var cells = new (int i, int j)[]
        {
            (I - 1, J - 1),
            (I - 1, J),
            (I - 1, J + 1),
            (I, J - 1),
            (I, J + 1),
            (I + 1, J - 1),
            (I + 1, J),
            (I + 1, J + 1),
        };
        IEnumerable<Octopus> neighbors = cells
            .Where(cell => cell.i is >= 0 and < 10 && cell.j is >= 0 and < 10)
            .Select(cell => _board[cell.i, cell.j])
            .Where(oct => !oct.HasFlashed);
        return neighbors;
    }

    public void IncreaseEnergy()
    {
        if (HasFlashed)
            return;
        this.Energy++;
        if (Energy > 9)
        {
            // Recursively process neighbors
            foreach (var neighbor in GetNeighbors())
            {
                neighbor.IncreaseEnergy();
            }
        }
    }

    public override string ToString()
    {
        return HasFlashed ? "." : Energy.ToString();
    }

    public void Reset()
    {
        if (HasFlashed)
        {
            Energy = 0;
        }
    }
}
