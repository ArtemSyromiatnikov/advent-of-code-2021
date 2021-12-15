// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//var lines = File.ReadAllLines("input.txt");
var lines = File.ReadAllLines("inputFull.txt");
var inputs = lines.Select(l =>
{
    var inputsRaw = l.Substring(0, l.IndexOf('|'));
    var outputsRaw = l.Substring(l.IndexOf('|')+1);
    return new Signals()
    {
        Inputs = inputsRaw
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToArray(),
        Outputs = outputsRaw
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToArray()
    };
}).ToArray();

// 0 - abcefg
// 1 - cf      <- 2!
// 2 - acdeg
// 3 - cdefg
// 4 - bcdf    <- 4!
// 5 - abdfg
// 6 - abdefg
// 7 - acf     <- 3!
// 8 - abcdefg <- 7!
// 9 - abcdfg

// 2 segments: 1
// 3 segments: 7
// 4 segments: 4
// 5 segments: 2, 3, 5
// 6 segments: 0, 6, 9
// 7 segments: 8 

// var simpleDigits = inputs
//     .Select(i => i.Outputs.Count(str => str.Length == 2 || str.Length == 3 || str.Length == 4 || str.Length == 7))
//     .Sum();
// Console.WriteLine($"Simple digits: {simpleDigits}");

List<int> outputs = new List<int>();
foreach (var line in inputs)
{
    var one = line.Inputs.Single(str => str.Length == 2).ToHashSet();
    var seven = line.Inputs.Single(str => str.Length == 3).ToHashSet();
    var four = line.Inputs.Single(str => str.Length == 4).ToHashSet();
    var eight = line.Inputs.Single(str => str.Length == 7).ToHashSet();
    var digitsWith5Segments = line.Inputs.Where(str => str.Length == 5).Select(str => str.ToHashSet()).ToList();
    var digitsWith6Segments = line.Inputs.Where(str => str.Length == 6).Select(str => str.ToHashSet()).ToList();

    var three = digitsWith5Segments.Single(digit => digit.Intersect(one).Count() == one.Count());
    digitsWith5Segments.Remove(three);
    var nine = digitsWith6Segments.Single(digit => digit.Intersect(four).Count() == four.Count());
    digitsWith6Segments.Remove(nine);
    var zero = digitsWith6Segments.Single(digit => digit.Intersect(seven).Count() == seven.Count());
    digitsWith6Segments.Remove(zero);
    var six = digitsWith6Segments.Single();
    var five = digitsWith5Segments.Single(digit => nine.Intersect(digit).Count() == digit.Count());
    digitsWith5Segments.Remove(five);
    var two = digitsWith5Segments.Single();

    var digitsSorted = new Dictionary<string, int>(){ 
        {String.Join(null, zero.OrderBy(a => a)), 0},
        {String.Join(null, one.OrderBy(a => a)), 1},
        {String.Join(null, two.OrderBy(a => a)), 2},
        {String.Join(null, three.OrderBy(a => a)), 3},
        {String.Join(null, four.OrderBy(a => a)), 4},
        {String.Join(null, five.OrderBy(a => a)), 5},
        {String.Join(null, six.OrderBy(a => a)), 6},
        {String.Join(null, seven.OrderBy(a => a)), 7},
        {String.Join(null, eight.OrderBy(a => a)), 8},
        {String.Join(null, nine.OrderBy(a => a)), 9}
    };
    var outputStr = String.Join(null, line.Outputs.Select(str => digitsSorted[String.Join(null, str.OrderBy(a => a))]));
    outputs.Add(Convert.ToInt32(outputStr));
    //Console.WriteLine($"{String.Join(null, line.Outputs)} : {outputStr}");
}

Console.WriteLine($"Result: {outputs.Sum()}");



public struct Signals
{
    public string[] Inputs { get; set; }
    public string[] Outputs { get; set; }
}

//public string StringToHash