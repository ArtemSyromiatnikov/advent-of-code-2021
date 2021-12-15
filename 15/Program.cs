var fileLines = File.ReadAllLines("inputFull.txt");
int SIZE = fileLines.Length;
int FULL_SIZE = SIZE * 5;
byte[,] template = new byte[SIZE, SIZE];

for (int i = 0; i < SIZE; i++)
{
    for (int j = 0; j < SIZE; j++)
    {
        template[i, j] = (byte)(fileLines[i][j] - 48);
    }
}

Node[,] map = new Node[FULL_SIZE, FULL_SIZE];
for (byte a = 0; a < 5; a++)
for (byte b = 0; b < 5; b++)
{
    byte offset = (byte)(a + b);
    for (int i = 0; i < SIZE; i++)
    for (int j = 0; j < SIZE; j++)
    {
        var newRiskLevel = template[i, j] + offset > 9
            ? (template[i, j] + offset) % 10 + 1
            : template[i, j] + offset;
            
        map[a * SIZE + i, b * SIZE + j] = new Node(newRiskLevel, a * SIZE + i, b * SIZE + j);
    }
}

// for (int i = 0; i < FULL_SIZE; i++)
// { 
//     for (int j = 0; j < FULL_SIZE; j++)
//         Console.Write(map[i,j].RiskLevel);
//     Console.WriteLine();
// }

// Initialize Edges
for (int i = 0; i < FULL_SIZE; i++)
{
    for (int j = 0; j < FULL_SIZE; j++)
    {
        if (i > 0)
            map[i, j].AddEdgeTo(map[i - 1, j]);
        if (i < FULL_SIZE-1)
            map[i, j].AddEdgeTo(map[i + 1, j]);
        if (j > 0)
            map[i, j].AddEdgeTo(map[i, j - 1]);
        if (j < FULL_SIZE-1)
            map[i, j].AddEdgeTo(map[i, j + 1]);
    }
}

// Dijkstra shortest path now!
var startNode = map[0, 0];
var endNode = map[FULL_SIZE-1, FULL_SIZE-1];


//var processedNodes = new HashSet<Node>() { startNode };
var processedNodes = new Dictionary<Node, GraphPath>();

var graphPathsSorted = new List<GraphPath>();
graphPathsSorted.Add(new GraphPath(startNode, null, 0));

do
{
    graphPathsSorted.Sort();
    if (processedNodes.Count % 1000 == 0)
        Console.WriteLine($"Processed nodes: {processedNodes.Count}    graphPathsSorted: {graphPathsSorted.Count}");

    var nodePathEntry = graphPathsSorted.First();
    graphPathsSorted.Remove(nodePathEntry);

    foreach (var neighbor in nodePathEntry.Node.Neighbors)
    {
        if (!processedNodes.ContainsKey(neighbor))
        {
            UpsertIfBetter(graphPathsSorted, nodePathEntry.Node, neighbor, nodePathEntry.Distance + neighbor.RiskLevel);
        }
    }
    processedNodes.Add(nodePathEntry.Node, nodePathEntry);
} while (!processedNodes.ContainsKey(endNode));

Console.WriteLine($"Shortest/safest path to the end node has length of {processedNodes[endNode].Distance}");





void UpsertIfBetter(List<GraphPath> sortedSet, Node from, Node to, int distance)
{
    var entry = sortedSet.FirstOrDefault(e => e.Node == to);
    if (entry != null)
    {
        if (distance < entry.Distance)
        {
            entry.FromNode = from;
            entry.Distance = distance;
        }
    }
    else
    {
        sortedSet.Add(new GraphPath(to, from, distance));
    }
}

public class GraphPath : IComparable<GraphPath>, IEquatable<GraphPath>
{
    public Node Node { get; set; }
    public Node FromNode { get; set; }
    public int Distance { get; set; }

    public GraphPath(Node node, Node fromNode, int distance)
    {
        Node = node;
        FromNode = fromNode;
        Distance = distance;
    }

    public int CompareTo(GraphPath? other)
    {
        if (other != null)
            return this.Distance - other.Distance;
        return 1;
    }

    public bool Equals(GraphPath? other)
    {
        return other != null && other.Node == this.Node;
    }
}

public class Node
{
    public int RiskLevel { get; set; }
    public int I { get; }
    public int J { get; }

    public HashSet<Node> Neighbors { get; } = new();
    
    public Node(int riskLevel, int i, int j)
    {
        RiskLevel = riskLevel;
        I = i;
        J = j;
    }

    public void AddEdgeTo(Node node2)
    {
        Neighbors.Add(node2);
    }

    public override string ToString()
    {
        return $"[{I},{J}]: {RiskLevel}";
    }
}
