using System.Diagnostics;

string inputRaw = await File.ReadAllTextAsync("input.txt");
int[] input = inputRaw
    .Split(Environment.NewLine, StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
    .Select(str => Convert.ToInt32(str))
    .ToArray();

// Puzzle 1
// int increments = 0;
// for (int i=1; i<input.Length; i++) {
//     if (input[i] > input[i-1])
//         increments++;
// }
// Console.WriteLine($"Total increments: {increments}");

// Puzzle 2

var sw = Stopwatch.StartNew();
var sums = new int[input.Length-2];
sums[0] = input[0];
sums[1] = input[0] + input[1];
for (int i=2; i<input.Length-2; i++) {
    sums[i] = input[i-2] + input[i-1] + input[i];
    // C# 8 Range/Index syntax = much slower in this case:
    //sums[i] = input[(i-2)..i].Sum();
}

int increments = 0;
for (int i=1; i<sums.Length; i++) {
    if (sums[i] > sums[i-1])
        increments++;
}

Console.WriteLine($"Time: {sw.ElapsedTicks} ticks");
Console.WriteLine($"Total increments: {increments}");