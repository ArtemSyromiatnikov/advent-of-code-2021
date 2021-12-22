using System.Text.RegularExpressions;

var file = File.ReadAllText("inputFull.txt");
var regex = new Regex(@"(?<action>on|off) x=(?<x1>-?\d+)..(?<x2>-?\d+),y=(?<y1>-?\d+)..(?<y2>-?\d+),z=(?<z1>-?\d+)..(?<z2>-?\d+)");
var instructions = regex.Matches(file).Select(m => new Instruction(
        m.Groups["action"].Value == "on",
        Convert.ToInt32(m.Groups["x1"].Value),
        Convert.ToInt32(m.Groups["x2"].Value),
        Convert.ToInt32(m.Groups["y1"].Value),
        Convert.ToInt32(m.Groups["y2"].Value),
        Convert.ToInt32(m.Groups["z1"].Value),
        Convert.ToInt32(m.Groups["z2"].Value)))
    .ToList();

// foreach(var i in instructions)
//     Console.WriteLine(i);

var xEdges = instructions.Select(i=>i.AsCuboid()).SelectMany(i => new[] {i.x1, i.x2})
    .Distinct().OrderBy(i=>i).ToList();
var yEdges = instructions.Select(i=>i.AsCuboid()).SelectMany(i => new[] {i.y1, i.y2})
    .Distinct().OrderBy(i=>i).ToList();
var zEdges = instructions.Select(i=>i.AsCuboid()).SelectMany(i => new[] {i.z1, i.z2})
    .Distinct().OrderBy(i=>i).ToList();

Console.WriteLine($"x: {xEdges.Count}, y: {yEdges.Count}, z: {zEdges.Count}");

var reactor = new Reactor(xEdges, yEdges, zEdges);
var i = 0;
foreach (var instruction in instructions)
{
    Console.WriteLine(i++);
    if (instruction.on)
        reactor.TurnOn(instruction.AsCuboid());
    else 
        reactor.TurnOff(instruction.AsCuboid());
}

Console.WriteLine($"Total cubes ON: {reactor.GetVolume()}");




public class Reactor
{
    List<int> xEdges { get; }
    List<int> yEdges { get; }
    List<int> zEdges { get; }

    public readonly HashSet<Cuboid> Cuboids = new();

    public Reactor(List<int> xEdges, List<int> yEdges, List<int> zEdges)
    {
        this.xEdges = xEdges;
        this.yEdges = yEdges;
        this.zEdges = zEdges;
    }

    public void TurnOn(Cuboid bigCuboid)
    {
        var cuboids = SplitToPrimitiveCuboids(bigCuboid);
        Cuboids.UnionWith(cuboids);
    }
    
    public void TurnOff(Cuboid bigCuboid)
    {
        var cuboids = SplitToPrimitiveCuboids(bigCuboid);
        Cuboids.ExceptWith(cuboids);
    }

    private IEnumerable<Cuboid> SplitToPrimitiveCuboids(Cuboid bigCuboid)
    {
        var xSplits = xEdges.Where(x => x >= bigCuboid.xMin && x <= bigCuboid.xMax).ToList();
        var ySplits = yEdges.Where(y => y >= bigCuboid.yMin && y <= bigCuboid.yMax).ToList();
        var zSplits = zEdges.Where(z => z >= bigCuboid.zMin && z <= bigCuboid.zMax).ToList();
        
        if (!xSplits.Any() || !ySplits.Any() || !zSplits.Any())
            yield break;

        for (int xi = 0; xi < xSplits.Count-1; xi++)
        for (int yi = 0; yi < ySplits.Count-1; yi++)
        for (int zi = 0; zi < zSplits.Count-1; zi++)
        {
            yield return new Cuboid(
                xSplits[xi], xSplits[xi + 1],
                ySplits[yi], ySplits[yi + 1],
                zSplits[zi], zSplits[zi + 1]
            );
        }
    }

    public long GetVolume() => Cuboids.Select(c => c.Volume).Sum();
}








public record Instruction(bool on, int x1, int x2, int y1, int y2, int z1, int z2)
{
    // Instruction stores coordinates in CELLS
    // Cuboid stores coordinates in EDGES
    public Cuboid AsCuboid() => new Cuboid(x1-1, x2, y1-1, y2, z1-1, z2);
    
    // public IEnumerable<Cube> GetCubesInBounds(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    // {
    //     if (x2 < minX || x1 > maxX ||
    //         y2 < minY || y1 > maxY ||
    //         z2 < minZ || z1 > maxZ)
    //         yield break;
    //
    //     minX = Math.Max(minX, x1);
    //     maxX = Math.Min(maxX, x2);
    //     minY = Math.Max(minY, y1);
    //     maxY = Math.Min(maxY, y2);
    //     minZ = Math.Max(minZ, z1);
    //     maxZ = Math.Min(maxZ, z2);
    //     for (int x = minX; x <= maxX; x++)
    //     for (int y = minY; y <= maxY; y++)
    //     for (int z = minZ; z <= maxZ; z++)
    //     {
    //         yield return new Cube(x, y, z);
    //     }
    // }
    //
    // public IEnumerable<Cube> GetCubes()
    // {
    //     for (int x = x1; x <= x2; x++)
    //     for (int y = y1; y <= y2; y++)
    //     for (int z = z1; z <= z2; z++)
    //     {
    //         yield return new Cube(x, y, z);
    //     }
    // }
}

public record Cuboid(int x1, int x2, int y1, int y2, int z1, int z2)
{
    public int xMin => Math.Min(x1, x2);
    public int xMax => Math.Max(x1, x2);
    public int yMin => Math.Min(y1, y2);
    public int yMax => Math.Max(y1, y2);
    public int zMin => Math.Min(z1, z2);
    public int zMax => Math.Max(z1, z2);

    public int Length => xMax - xMin;
    public int Width => yMax - yMin;
    public int Height => zMax - zMin;
    
    public long Volume => 1L * (xMax - xMin) * (yMax - yMin) * (zMax - zMin);
}


// Part 1 - naive solution...
/*HashSet<Cube> reactorCubes = new();
var reactor
foreach (var instruction in instructions)
{
    var cubes = instruction
        .GetCubesInBounds(-50, 50, -50, 50, -50, 50);
    if (instruction.on)
        reactorCubes.UnionWith(cubes);
    else 
        reactorCubes.ExceptWith(cubes);
}

Console.WriteLine($"Cube count: {reactorCubes.Count}");*/

//public record Cube(int x, int y, int z);