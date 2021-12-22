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


HashSet<Cube> reactorCubes = new();
foreach (var instruction in instructions)
{
    var cubes = instruction
        .GetCubesInBounds(-50, 50, -50, 50, -50, 50);
    if (instruction.on)
        reactorCubes.UnionWith(cubes);
    else 
        reactorCubes.ExceptWith(cubes);
}

Console.WriteLine($"Cube count: {reactorCubes.Count}");


public record Instruction(bool on, int x1, int x2, int y1, int y2, int z1, int z2)
{
    public IEnumerable<Cube> GetCubesInBounds(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        if (x2 < minX || x1 > maxX ||
            y2 < minY || y1 > maxY ||
            z2 < minZ || z1 > maxZ)
            yield break;

        minX = Math.Max(minX, x1);
        maxX = Math.Min(maxX, x2);
        minY = Math.Max(minY, y1);
        maxY = Math.Min(maxY, y2);
        minZ = Math.Max(minZ, z1);
        maxZ = Math.Min(maxZ, z2);
        for (int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++)
        for (int z = minZ; z <= maxZ; z++)
        {
            yield return new Cube(x, y, z);
        }
    }
    
    public IEnumerable<Cube> GetCubes()
    {
        for (int x = x1; x <= x2; x++)
        for (int y = y1; y <= y2; y++)
        for (int z = z1; z <= z2; z++)
        {
            yield return new Cube(x, y, z);
        }
    }
}
public record Cube(int x, int y, int z);