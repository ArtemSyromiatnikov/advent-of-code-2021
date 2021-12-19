using System.Diagnostics;

const int MINIMAL_INTERSECTION = 12;


// Point p = new Point(1, 2, 0);
// Console.WriteLine($" {Rotate.ByXAxis(p, 0)}");
// Console.WriteLine($" {Rotate.ByXAxis(p, 1)}");
// Console.WriteLine($" {Rotate.ByXAxis(p, 2)}");
// Console.WriteLine($" {Rotate.ByXAxis(p, 3)}");
//
// Console.WriteLine($" {Rotate.ByYAxis(p, 0)}");
// Console.WriteLine($" {Rotate.ByYAxis(p, 1)}");
// Console.WriteLine($" {Rotate.ByYAxis(p, 2)}");
// Console.WriteLine($" {Rotate.ByYAxis(p, 3)}");
//
// Console.WriteLine($" {Rotate.ByZAxis(p, 0)}");
// Console.WriteLine($" {Rotate.ByZAxis(p, 1)}");
// Console.WriteLine($" {Rotate.ByZAxis(p, 2)}");
// Console.WriteLine($" {Rotate.ByZAxis(p, 3)}");



// Puzzle 1
var allScanners = ReadInput("inputFull.txt");

Stopwatch sw = Stopwatch.StartNew();
var masterScanner = allScanners[0];
masterScanner.Location = new Point(0, 0, 0);

var unprocessedScanners = allScanners.Skip(1).ToList();

while (unprocessedScanners.Any())
{
    Console.WriteLine($"Scanners left: {unprocessedScanners.Count}");
    var masterOffsets = masterScanner.CalculateOffsetArrays();
    
    for (int i = 0; i < unprocessedScanners.Count; i++)
    {
        Console.WriteLine($"  Trying scanner: {i+1}...");
        var currentScanner = unprocessedScanners[i];

        var currentScannerOffsets = currentScanner.CalculateOffsetArraysWithRotations();
        if (AreOffsetsIntersecting(masterOffsets, currentScannerOffsets, out var masterPoint, out var matchingOffsets))
        {
            masterScanner.AddPoints(masterPoint, matchingOffsets.Offsets);
            Console.WriteLine($"Match found! Master point {masterPoint} corresponds to scanner point {matchingOffsets.ReferencePoint}");
            currentScanner.Location = matchingOffsets.ScannerLocation + masterPoint;
            Console.WriteLine($"bad scanner location: {matchingOffsets.ReferencePoint - masterPoint} ???");
            Console.WriteLine($"Good scanner location: {currentScanner.Location} ???");
            unprocessedScanners.Remove(currentScanner);
            
            break;
        }
    }
}

Console.WriteLine("Full area: ");
Console.WriteLine($"Total points: {masterScanner.Points.Count}");
Console.WriteLine($"Time elapsed: {sw.Elapsed.TotalSeconds}s");
//PrintPoints(masterScanner.Points.OrderBy(p=>p.x).ThenBy(p=>p.y));
// end of puzzle 1



var scannerLocations = allScanners.Select(s => s.Location).ToList();
Console.WriteLine("Scanner Locations:");
PrintPoints(scannerLocations);

int maxDistance = 0; 
for (int i=0; i<scannerLocations.Count-1; i++)
for (int j = i+1; j < scannerLocations.Count; j++)
{
    var dist = CalculateManhattanDistance(scannerLocations[i], scannerLocations[j]);
    if (dist > maxDistance)
        maxDistance = dist;
}

Console.WriteLine($"Max manhattan distance: {maxDistance}");




int CalculateManhattanDistance(Point left, Point right)
{
    return Math.Abs(right.x - left.x) +
           Math.Abs(right.y - left.y) +
           Math.Abs(right.z - left.z);
}



bool AreOffsetsIntersecting(IEnumerable<OffsetCollection> masterOffsets, IEnumerable<OffsetCollection> currentScannerOffsets,
    out Point masterPoint, out OffsetCollection scannerOffset)
{
    foreach (var area1offsets in masterOffsets)
    foreach (var area2offsets in currentScannerOffsets)
    {
        if (area1offsets.Offsets.Intersect(area2offsets.Offsets).Count() >= MINIMAL_INTERSECTION)
        {
            // Console.WriteLine("Offsets 1:");
            // PrintPoints(area1offsets.Offsets);
            //
            // Console.WriteLine("Offsets 2:");
            // PrintPoints(area2offsets.Offsets);
            //
            // Console.WriteLine("Intersection:");
            // PrintPoints(area1offsets.Offsets.Intersect(area2offsets.Offsets));
            
            masterPoint = area1offsets.ReferencePoint;
            scannerOffset = area2offsets;

            return true;
        }
    }

    masterPoint = null;
    scannerOffset = null;
    return false;
}











List<Scanner> ReadInput(string fileName)
{
    var lines = File.ReadAllLines(fileName);
    int scannerIndex = 0;

    List<Scanner> list = new();
    foreach (var line in lines)
    {
        if (String.IsNullOrEmpty(line))
            continue;

        if (line.StartsWith("--- scanner"))
        {
            list.Add(new Scanner(scannerIndex));
            scannerIndex++;
        }
        else
        {
            var a = line.Split(',');
            var point = new Point(Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), Convert.ToInt32(a[2]));
            list.Last().Points.Add(point);
        }
    }

    return list;
}

void PrintPoints(IEnumerable<Point> points)
{
    foreach ((int x, int y, int z) in points)
    {
        Console.WriteLine($"  {x,4},{y,4},{z,4}");
    }
}


public record Point(int x, int y, int z)
{
    public static Point operator +(Point left, Point right)
    {
        return new Point(left.x + right.x, left.y + right.y, left.z + right.z);
    }
    public static Point operator -(Point left, Point right)
    {
        return new Point(left.x - right.x, left.y - right.y, left.z - right.z);
    }

    public override string ToString()
    {
        return $"{x,4},{y,4},{z,4}";
    }
};

public class Scanner
{
    public int ScannerIndex { get; }
    public HashSet<Point> Points { get; } = new();
    public Point Location { get; set; }

    public Scanner(int scannerIndex)
    {
        ScannerIndex = scannerIndex;
    }

    public IEnumerable<OffsetCollection> CalculateOffsetArrays()
    {
        // Chose frame of reference
        // calculate offset for each beacon relative to the Point 0
        foreach (var reference in Points)
        {
            var scannerLocation = new Point(0, 0, 0) - reference;
            var offsetArray = Points.Select(p =>
                new Point(p.x - reference.x, p.y - reference.y, p.z - reference.z))
                .ToHashSet();
            yield return new OffsetCollection(reference, scannerLocation, offsetArray);
        }
    }
    
    public IEnumerable<OffsetCollection> CalculateOffsetArraysWithRotations()
    {
        foreach (var transformation in Rotate.AllRotations)
        {
            var offsetArraySet = CalculateOffsetArrays();
            foreach (var offsetArray in offsetArraySet)
            {
                var scannerLocation = transformation(offsetArray.ScannerLocation); 
                var rotatedArray = offsetArray.Offsets
                    .Select(offset => transformation(offset))
                    .ToHashSet();
                yield return new OffsetCollection(offsetArray.ReferencePoint, scannerLocation, rotatedArray);
            }
        }
    }

    public void AddPoints(Point masterPoint, HashSet<Point> offsets)
    {
        foreach (var offset in offsets)
        {
            Points.Add(masterPoint + offset);
        }
    }
}

public class OffsetCollection
{
    public Point ReferencePoint { get; }
    public Point ScannerLocation { get; }
    public HashSet<Point> Offsets { get; }

    public OffsetCollection(Point referencePoint, Point scannerLocation, HashSet<Point> offsets)
    {
        ReferencePoint = referencePoint;
        Offsets = offsets;
        ScannerLocation = scannerLocation;
    }
}

/// <summary>
/// https://stackoverflow.com/questions/14607640/rotating-a-vector-in-3d-space
/// </summary>
public static class Rotate
{
    public static Point ByXAxis(Point p, int quarters)
    {
        var angle = Math.PI / 2.0 * quarters;
        return new Point(
            p.x,
            (int)Math.Round(p.y * Math.Cos(angle) - p.z * Math.Sin(angle)),
            (int)Math.Round(p.y * Math.Sin(angle) + p.z * Math.Cos(angle)));
    }

    public static Point ByYAxis(Point p, int quarters)
    {
        var angle = Math.PI / 2.0 * quarters;
        return new Point(
            (int)Math.Round(p.x * Math.Cos(angle) + p.z * Math.Sin(angle)),
            p.y,
            (int)Math.Round(-p.x * Math.Sin(angle) + p.z * Math.Cos(angle)));
    }

    public static Point ByZAxis(Point p, int quarters)
    {
        var angle = Math.PI / 2.0 * quarters;
        var p1 = new Point(
            (int)Math.Round(p.x * Math.Cos(angle) - p.y * Math.Sin(angle)),
            (int)Math.Round(p.x * Math.Sin(angle) + p.y * Math.Cos(angle)),
            p.z);

        return p1;
    }
    
    public static Func<Point, Point>[] AllRotations =
    {
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 0), 0),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 0), 1),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 0), 2),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 0), 3),
            
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 1), 0),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 1), 1),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 1), 2),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 1), 3),
            
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 2), 0),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 2), 1),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 2), 2),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 2), 3),
            
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 3), 0),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 3), 1),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 3), 2),
        p => Rotate.ByXAxis(Rotate.ByYAxis(p, 3), 3),
            
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 1), 0),
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 1), 1),
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 1), 2),
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 1), 3),
            
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 3), 0),
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 3), 1),
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 3), 2),
        p => Rotate.ByXAxis(Rotate.ByZAxis(p, 3), 3),
    };
}