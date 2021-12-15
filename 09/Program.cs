using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

var lines = System.IO.File.ReadAllLines("input1.txt");
int ROWS = lines.Length;
int COLS = lines[0].Length;
byte[,] map = new byte[ROWS+2, COLS+2];

for (int i = 0; i < ROWS+2; i++)
    map[i, 0] = map[i, COLS+1] = 9;
for (int j = 0; j < COLS+2; j++)
    map[0, j] = map[ROWS+1, j] = 9;

for (int i = 1; i < ROWS + 1; i++)
for (int j = 1; j < COLS + 1; j++)
    map[i, j] = (byte)(lines[i - 1][j - 1] - 48); // parse char digit as byte

// Puzzle 1
// int sum = 0;
// for (int i = 1; i < ROWS + 1; i++)
// for (int j = 1; j < COLS + 1; j++)
// {
//     byte cell = map[i, j];
//     if (cell < map[i - 1, j] &&
//         cell < map[i + 1, j] &&
//         cell < map[i, j - 1] &&
//         cell < map[i, j + 1])
//     {
//         // this is a low point
//         sum += cell + 1;
//         Console.Write($"{cell},");
//     } 
// }
// Console.WriteLine();
// Console.WriteLine($"Sum of low points: {sum}");


// Puzzle 2

// Start with low points
// expand low points to basins surrounded by 9's
// track each basin

// Assumption: one basin can't have two lowest points
// Assumption: each basin has one well-defined lowest point (e.g. can't have "1001" situation)

List<Basin> basins = new();
for (int i = 1; i < ROWS + 1; i++)
for (int j = 1; j < COLS + 1; j++)
{
    byte height = map[i, j];
    if (height < map[i - 1, j] &&
        height < map[i + 1, j] &&
        height < map[i, j - 1] &&
        height < map[i, j + 1])
    {
        // this is a low point
        basins.Add(new Basin(new Cell(i, j)));
        Console.Write($"{height},");
    } 
}
Console.WriteLine();

foreach (var basin in basins)
{
    while (basin.PotentialCells.Any())
    {
        var cell = basin.PotentialCells.First();
        basin.TryCell(cell, map[cell.i, cell.j]);
    }
}


for (var i = 0; i < basins.Count; i++)
{
    Console.WriteLine($"Basin [{i+1}]: {basins[i].Size}");
}

var biggestBasins = basins.OrderByDescending(b => b.Size).Take(3).ToArray();
Console.WriteLine($"Size of three largest basins: {biggestBasins[0].Size * biggestBasins[1].Size * biggestBasins[2].Size}");


// Output
// for (int i = 1; i < ROWS + 1; i++)
// {
//     for (int j = 1; j < COLS + 1; j++)
//         Console.Write(map[i, j] + " ");
//     Console.WriteLine();
// }


public class Basin
{
    public HashSet<Cell> Cells { get; } = new();
    public HashSet<Cell> PotentialCells { get; } = new();

    public int Size => Cells.Count;

    public Basin(Cell lowestPoint)
    {
        Cells.Add(lowestPoint);
        DiscoverNewPotentialCells(lowestPoint);
    }

    public void TryCell(Cell cell, byte height)
    {
        if (height < 9)
        {
            Cells.Add(cell);
            PotentialCells.Remove(cell);
            DiscoverNewPotentialCells(cell);
        }
        else
        {
            PotentialCells.Remove(cell);
        }
    }

    private void DiscoverNewPotentialCells(Cell cell)
    {
        AddPotentialCell(new Cell(cell.i - 1, cell.j));
        AddPotentialCell(new Cell(cell.i + 1, cell.j));
        AddPotentialCell(new Cell(cell.i, cell.j-1));
        AddPotentialCell(new Cell(cell.i, cell.j+1));
    }

    private void AddPotentialCell(Cell cellToAnalyze)
    {
        if (!Cells.Contains(cellToAnalyze))
            PotentialCells.Add(cellToAnalyze);
    }
}

public record Cell (int i, int j);