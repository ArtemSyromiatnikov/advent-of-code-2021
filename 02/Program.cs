string inputRaw = await File.ReadAllTextAsync("input.txt");
string[] input = inputRaw
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .ToArray();



// Puzzle 1
// foreach (var entry in input) {
//     var spaceIx = entry.IndexOf(' ');
//     string dir = entry.Substring(0, spaceIx);
//     int stepLength = Convert.ToInt32(entry.Substring(spaceIx+1));
//     //Console.WriteLine($"{dir}-{stepLength}");
//     switch (dir) {
//         case "forward":
//             distance += stepLength;
//             break;
//         case "down":
//             depth += stepLength;
//             break;
//         case "up":
//             depth -= stepLength;
//             break;
//     }
// }

// Console.WriteLine($"Depth: {depth}, Distance: {distance}, Multiplied: {depth*distance}");

int depth = 0;
int distance = 0;
int aim = 0;

foreach (var entry in input) {
    var spaceIx = entry.IndexOf(' ');
    string dir = entry.Substring(0, spaceIx);
    int stepLength = Convert.ToInt32(entry.Substring(spaceIx+1));
    //Console.WriteLine($"{dir}-{stepLength}");
    switch (dir) {
        case "forward":
            distance += stepLength;
            depth += aim * stepLength;
            break;
        case "down":
            //depth += stepLength;
            aim += stepLength;
            break;
        case "up":
            //depth -= stepLength;
            aim -= stepLength;
            break;
    }
    Console.WriteLine($"{entry}\t\t=> Depth: {depth}, Distance: {distance}, Aim: {aim}");
}

Console.WriteLine($"Depth: {depth}, Distance: {distance}, Aim: {aim}, Multiplied: {depth*distance}");