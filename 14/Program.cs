using System.Text.RegularExpressions;


var fileLines = File.ReadAllLines("inputFull.txt");

var regex = new Regex(@"(?<pair>\w\w) -> (?<insert>\w)");
string polymer = fileLines[0];
var rules = fileLines[2..]
    .Select(l =>
    {
        var m = regex.Match(l);
        var pair = m.Groups["pair"].Value;
        var ch = m.Groups["insert"].Value[0];
        //var descendents = new[] { $"{pair[0]}{ch}", $"{ch}{pair[1]}" };
        return new Rule(pair, $"{pair[0]}{ch}", $"{ch}{pair[1]}");
    }).ToList();

Console.WriteLine($"Input:");
Console.WriteLine($"  Polymer: {polymer}    Rules: {rules.Count}");

var allChars = rules.SelectMany(r => r.pair).ToHashSet();


Dictionary<string, List<Dictionary<char, long>>> table = rules
    .ToDictionary(r => r.pair, _ => new List<Dictionary<char, long>>());

// Initialize generation 0
foreach ((string pair, List<Dictionary<char, long>> list) in table)
{
    var dict = allChars.ToDictionary(ch => ch, _ => 0l);
    dict[pair[0]] = 1;
    list.Add(dict);
}

// Generate table of generations
const int STEPS = 41;
for (int generation=1; generation<STEPS; generation++)
{
    foreach (var rule in rules)
    {
        // Character count for each new generation can be calculated based
        // on char count of two previous generations:
        var source1 = table[rule.child1][generation - 1];
        var source2 = table[rule.child2][generation - 1];
        var dict = allChars.ToDictionary(c => c, c => source1[c] + source2[c]);
        table[rule.pair].Add(dict);
    }
}

var finalCounter = allChars.ToDictionary(c => c, _ => 0l);
for (int i = 0; i < polymer.Length - 1; i++)
{
    var pair = polymer.Substring(i, 2);
    var charCountsForGivenPair = table[pair][STEPS - 1];
    foreach (var character in allChars)
    {
        finalCounter[character] += charCountsForGivenPair[character];
    }
}

// And include the last symbol!
finalCounter[polymer.Last()]++;

// Output
foreach (var kv in finalCounter.OrderBy(kv => kv.Value))
{
    Console.WriteLine($"{kv.Key}: {kv.Value}");
}

var max = finalCounter.Select(kv => kv.Value).Max();
var min = finalCounter.Select(kv => kv.Value).Min();
Console.WriteLine($"Diff = {max - min}");


public record Rule(String pair, string child1, string child2);

// Solution 3
/*
for (int i = 0; i < polymer.Length - 1; i++)
{
    Console.WriteLine($"Processing {i+1} pair...");
    Traverse(polymer.Substring(i, 2), STEPS, rules, aggregator);
}
Console.WriteLine();

foreach (var kv in aggregator.OrderBy(kv => kv.Value))
{
    Console.WriteLine($"{kv.Key}: {kv.Value}");
}

var max = aggregator.Select(kv => kv.Value).Max();
var min = aggregator.Select(kv => kv.Value).Min();
Console.WriteLine($"Diff = {max - min}");


void Traverse(string pair, int depth, Dictionary<string, string[]> rules, Dictionary<char, long> aggregator)
{
    if (depth == 0)
    {
        //Console.Write(pair[1]);
        //if (aggregator.ContainsKey(pair[1]))
            aggregator[pair[1]]++;
        //else 
            //aggregator[pair[1]] = 1;
        return;
    }
    
    if (rules.ContainsKey(pair))
    {
        Traverse(rules[pair][0], depth - 1, rules, aggregator);
        Traverse(rules[pair][1], depth - 1, rules, aggregator);
    }
    else
    {
        //Console.Write(pair[1]);
        //if (aggregator.ContainsKey(pair[1]))
            aggregator[pair[1]]++;
        //else 
            //aggregator[pair[1]] = 1;
    }
}
*/






// Naive solution 1
// for (int i = 0; i < 40; i++)
// {
//     polymer = RunPolymerization(polymer, rules);
//     Console.WriteLine($"Iteration {i+1}: {polymer.Length}");
// }

// var sortedChars = polymer
//     .GroupBy(ch => ch)
//     .OrderBy(gr => gr.Count())
//     // .Select(gr =>
//     // {
//     //     Console.WriteLine($"{gr.Key}: {gr.Count()}");
//     //     return gr.Key;
//     // })
//     .ToList();
//
//
// var mostCommon = (character: sortedChars.Last().Key, count: sortedChars.Last().Count());
// var leastCommon = (character: sortedChars.First().Key, count: sortedChars.First().Count());
// Console.WriteLine($"{mostCommon.character}: {mostCommon.count},  {leastCommon.character}: {leastCommon.count}");
// Console.WriteLine($"Diff = {mostCommon.count-leastCommon.count}");

// Naive solution 1
/*string RunPolymerization(string template, Dictionary<string, char> rules)
{
    List<char> polymer = new List<char>(template.Length * 2) { template[0] };
    
    for (int i = 1; i < template.Length; i++)
    {
        string pair = template.Substring(i-1, 2);
        if (rules.ContainsKey(pair))
            polymer.Add(rules[pair]);
        polymer.Add(template[i]);
    }

    return new String(polymer.ToArray());
}
*/