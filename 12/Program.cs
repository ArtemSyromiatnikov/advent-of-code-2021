using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

const string START = "start";
const string END = "end";

var fileText = System.IO.File.ReadAllText("inputFull.txt");
var regex = new Regex(@"(?<node1>\w+)-(?<node2>\w+)");
var edges = regex.Matches(fileText)
    .Select(m => (Node1: m.Groups["node1"].Value, Node2: m.Groups["node2"].Value));

var nodeNames = edges.Select(e => e.Item1)
    .Concat(edges.Select(e => e.Item2))
    .ToHashSet();

var nodes = nodeNames
    .Select(name => new Node(name))
    .ToDictionary(node => node.Name);


Console.WriteLine($"All node names: {String.Join(", ", nodeNames)}");

// init edges
foreach (var edge in edges)
{
    nodes[edge.Node1].AddEdgeTo(nodes[edge.Node2]);
}

var sw = Stopwatch.StartNew();
IEnumerable<ImmutableList<Node>> CountUniquePathsToEnd(ImmutableList<Node> pathSoFar)
{
    Node currentNode = pathSoFar.Last();
    
    // Puzzle 1 - each small cave can be visited once
    // var alreadyVisitedSmallNodes = pathSoFar.Where(n => !n.IsBigNode);
    // var candidatesToVisitNext = currentNode.Neighbors
    //     .Except(alreadyVisitedSmallNodes)
    //     .ToList();


    // Puzzle 2 - ONE small cave can be visited twice
    var smallNodesVisitCount = pathSoFar.Where(n => !n.IsBigNode)
        .GroupBy(node => node)
        .Select(gr => (node: gr.Key, count: gr.Count()));
    
    List<Node> candidatesToVisitNext;
    bool doubleVisitHappened = smallNodesVisitCount.Any(nvc => nvc.count > 1);
    if (doubleVisitHappened)
    {
        // we can't revisit previously visited small nodes anymore
        var alreadyVisitedSmallNodes = pathSoFar.Where(n => !n.IsBigNode);
        candidatesToVisitNext = currentNode.Neighbors
            .Except(alreadyVisitedSmallNodes)
            .ToList();
    }
    else
    {
        // we CAN revisit previously visited small nodes (except START)
        candidatesToVisitNext = currentNode.Neighbors
            .Where(node => node.Name != START)
            .ToList();
    }
    // End of Puzzle 2 specific code

    var pathsToEnd = new List<ImmutableList<Node>>();
    
    var endNode = candidatesToVisitNext.FirstOrDefault(node => node.Name == END);
    if (endNode != null)
    {
        pathsToEnd.Add(pathSoFar.Add(endNode));
        candidatesToVisitNext.Remove(endNode);
    }

    foreach (var nodeCandidate in candidatesToVisitNext)
    {
        var newPathsToEnd = CountUniquePathsToEnd(pathSoFar.Add(nodeCandidate));
        pathsToEnd.AddRange(newPathsToEnd);
    }

    return pathsToEnd;
}

var start = nodes[START];
var allPaths = CountUniquePathsToEnd(new List<Node> {start}.ToImmutableList());
// foreach (var pathNodes in allPaths)
// {
//     Console.WriteLine(String.Join(",", pathNodes));
// }
Console.WriteLine($"Total paths: {allPaths.Count()}");
Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");






public class Node
{
    public string Name { get; }

    public HashSet<Node> Neighbors { get; } = new();
    
    public bool IsBigNode { get; }
    
    public Node(string name)
    {
        Name = name;
        IsBigNode = name == name.ToUpper();
    }

    public void AddEdgeTo(Node node2)
    {
        Neighbors.Add(node2);
        node2.Neighbors.Add(this);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Node node2)
        {
            return this.Name == node2.Name;
        }

        return false;
    }

    public override string ToString()
    {
        return Name;
    }
}
