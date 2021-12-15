using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var inputStr = await File.ReadAllTextAsync("input.txt");
var regex = new Regex(@"(?<x1>\d+),(?<y1>\d+) -> (?<x2>\d+),(?<y2>\d+)");
var lines = regex.Matches(inputStr).Select(match => new Line()
{
    Point1 = new (Convert.ToInt32(match.Groups["x1"].Value), Convert.ToInt32(match.Groups["y1"].Value)),
    Point2 = new (Convert.ToInt32(match.Groups["x2"].Value), Convert.ToInt32(match.Groups["y2"].Value)),
}).ToArray();

// foreach (var l in lines)
// {
//     Console.WriteLine($"{l.Point1.X},{l.Point1.Y} => {l.Point2.X},{l.Point2.Y}");
// };

int SIZE = 1 + lines.Max(l => new[] { l.Point1.X, l.Point1.Y, l.Point2.X, l.Point1.Y }.Max());
Console.WriteLine($"Max dimension: {SIZE}");
int[,] board = new int[SIZE, SIZE];

// Puzzle 1
// foreach (var line in lines)
// {
//     if (line.IsDiagonal is not true)
//     {
//         foreach ((int x, int y) in line.EnumeratePoints)
//         {
//             board[x, y]++;
//         }
//     }
// }

// int result = 0;
// foreach (var cell in board)
// {
//     if (cell > 1)
//         result++;
// }
// Console.WriteLine($"Result: {result}");


// Puzzle 2
foreach (var line in lines)
{
    foreach ((int x, int y) in line.EnumeratePoints)
    {
        board[x, y]++;
    }
}

int result = 0;
foreach (var cell in board)
{
    if (cell > 1)
        result++;
}
Console.WriteLine($"Result: {result}");



// Output
// for (int i = 0; i < SIZE; i++)
// {
//     for (int j = 0; j < SIZE; j++)
//     {
//         Console.Write(board[j,i] > 0 ? board[j,i] : "." );
//     }
//     Console.WriteLine();
// }
// Console.WriteLine();



public struct Line
{
    public (int X, int Y) Point1 { get; set; }
    public (int X, int Y) Point2 { get; set; }

    public bool IsDiagonal => !(Point1.X == Point2.X || Point1.Y == Point2.Y);

    public IEnumerable<(int X, int Y)> EnumeratePoints
    {
        get
        {
            int xStep = Math.Sign(Point2.X - Point1.X);
             int yStep = Math.Sign(Point2.Y - Point1.Y);;

            int steps = Math.Max(Math.Abs(Point1.X - Point2.X), Math.Abs(Point1.Y - Point2.Y));

            var point = (Point1.X, Point1.Y);
            for (int i = 0; i <= steps; i++)
            {
                yield return point;
                point.X += xStep;
                point.Y += yStep;
            }
        }
    }
}