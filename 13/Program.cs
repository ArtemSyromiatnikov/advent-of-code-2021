using System.Text.RegularExpressions;

var fileText = System.IO.File.ReadAllText("inputFull.txt");
var regexDots = new Regex(@"(?<x>\d+),(?<y>\d+)");
var regexFolds = new Regex(@"fold along (?<axis>\w)=(?<value>\d+)");
var dots = regexDots.Matches(fileText)
    .Select(m => (x: Convert.ToInt32(m.Groups["x"].Value), y: Convert.ToInt32(m.Groups["y"].Value)))
    .ToHashSet();
var folds = regexFolds.Matches(fileText)
    .Select(m => (xAxis: m.Groups["axis"].Value == "x", value: Convert.ToInt32(m.Groups["value"].Value)))
    .ToList();


//Console.WriteLine($"Dots: {String.Join("; ", dots)}  -  {dots.Count} in total");
//Console.WriteLine($"Folds: {String.Join("; ", folds)}");

Console.WriteLine($"Input:");
Console.WriteLine($"  Dots: {dots.Count}    maxX: {dots.MaxBy(d => d.x).x}   maxY: {dots.MaxBy(d => d.y).y}");
Console.WriteLine($"  Folds: {folds.Count}");

// Do the folding
foreach (var fold in folds)
{
    dots = Fold(dots, fold.xAxis, fold.value);
    //Console.WriteLine($"Dots: {String.Join("; ", dots)}");
    Console.WriteLine($"Fold {(fold.xAxis ? 'x' : 'y')}={fold.value}:  {dots.Count} dots left");
}

// Visualize
var cols = dots.MaxBy(d => d.x).x+1;
var rows = dots.MaxBy(d => d.y).y+1;
for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < cols; j++)
    {
        Console.Write(dots.Contains((x: j, y: i)) ? '#' : '.');
    }
    Console.WriteLine();
}


HashSet<(int x, int y)> Fold(HashSet<(int x, int y)> dots, bool foldAlongXAxis, int axisValue)
    => dots
        .Select(dot => (
            x: foldAlongXAxis 
                ?  dot.x > axisValue 
                    ? 2*axisValue - dot.x
                    : dot.x
                : dot.x,
            y: foldAlongXAxis 
                ? dot.y 
                : dot.y > axisValue
                    ? 2*axisValue - dot.y
                    : dot.y
            ))
        .ToHashSet();

