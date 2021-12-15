var lines = File.ReadAllLines("input1.txt");

int syntaxScore = 0;
var completionScores = new List<long>();
foreach (var line in lines)
{
    bool isCorrupted = false;

    // Puzzle 1
    var state = new Stack<ChunkType>();
    for (int i = 0; i < line.Length; i++)
    {
        switch (line[i])
        {
            case '(':
                state.Push(ChunkType.Round);
                break;
            case '[':
                state.Push(ChunkType.Square);
                break;
            case '{':
                state.Push(ChunkType.Curly);
                break;
            case '<':
                state.Push(ChunkType.Angle);
                break;
            
            case ')':
                var token1 = state.Pop();
                if (token1 != ChunkType.Round)
                {
                    isCorrupted = true;
                    syntaxScore += 3;
                }

                break;
            case ']':
                var token2 = state.Pop();
                if (token2 != ChunkType.Square)
                {
                    isCorrupted = true;
                    syntaxScore += 57;
                }

                break;
            case '}':
                var token3 = state.Pop();
                if (token3 != ChunkType.Curly)
                {
                    isCorrupted = true;
                    syntaxScore += 1197;
                }

                break;
            case '>':
                var token4 = state.Pop();
                if (token4 != ChunkType.Angle)
                {
                    isCorrupted = true;
                    syntaxScore += 25137;
                }

                break;
        }

        if (isCorrupted)
            break;
    }

    // Puzzle 2
    if (!isCorrupted)
    {
        // so this is an incomplete line.
        long completionScore = 0;
        while (state.TryPop(out var chunk))
        {
            completionScore = completionScore * 5 + chunk switch
            {
                ChunkType.Round => 1,
                ChunkType.Square => 2,
                ChunkType.Curly => 3,
                ChunkType.Angle => 4,
            };
        } 
        completionScores.Add(completionScore);
    }
}

Console.WriteLine($"Syntax score: {syntaxScore}");

completionScores = completionScores.OrderBy(s => s).ToList();
Console.WriteLine(String.Join(", ", completionScores));

var result = completionScores.Skip(completionScores.Count / 2).First();
Console.WriteLine($"Completion score: {result}");


public enum ChunkType
{
    Round,
    Square,
    Curly,
    Angle
}