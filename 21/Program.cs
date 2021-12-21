#region Puzzle 1
// Puzzle 1
//const int startingTile1 = 4;
//const int startingTile2 = 8;
//const int startingTile1 = 9;
//const int startingTile2 = 3;


/*int p1position = startingTile1;
int p2position = startingTile2;
int p1score = 0;
int p2score = 0;
bool isPlayer1Turn = true;
DeterministicDie die = new();

while (Math.Max(p1score, p2score) < 1000)
{
    var steps = die.ThrowDie() + die.ThrowDie() + die.ThrowDie();
    
    if (isPlayer1Turn)
    {
        p1position = (p1position + steps) % 10;
        if (p1position == 0)
            p1position = 10;

        p1score += p1position;
    }
    else
    {
        p2position = (p2position + steps) % 10;
        if (p2position == 0)
            p2position = 10;

        p2score += p2position;
    }
    
    isPlayer1Turn = !isPlayer1Turn;
}

Console.WriteLine($"Score: {p1score} x {p2score}");
Console.WriteLine($"Score: {Math.Min(p1score, p2score)} x {die.TotalRolls} = {Math.Min(p1score, p2score) * die.TotalRolls}");

public class DeterministicDie
{
    public int Value = 100;
    public int TotalRolls = 0;

    public int ThrowDie()
    {
        TotalRolls++;
        
        Value = (Value + 1) % 100;
        if (Value == 0)
            Value = 100;
        return Value;
    }
}
*/
#endregion

// Puzzle 2

//const int startingTile1 = 4;
//const int startingTile2 = 8;
const int startingTile1 = 9;
const int startingTile2 = 3;

var initialGameStatus = new GameStatus
{
    p1position = startingTile1,
    p2position = startingTile2,
    p1score = 0,
    p2score = 0,
    isPlayer1Turn = true
};
long p1victories = 0;
long p2victories = 0;

var logThreshold = 1_000_000_000;

PlayGame(initialGameStatus);

Console.WriteLine($"Victories: {p1victories} x {p2victories}");
Console.WriteLine($"Max victories: {Math.Max(p1victories, p2victories)}");


void PlayGame(GameStatus game) => PlayNextRound(game, 1);

void PlayNextRound(GameStatus game, long times)
{
    /*if (p1victories > logThreshold)
    {
        Console.WriteLine(logThreshold);
        logThreshold *= 10;
    }*/
    
    if (Math.Max(game.p1score, game.p2score) >= 21)
    {
        if (game.IsP1Winning)
            p1victories += times;
        else
            p2victories += times;
        return;
    }

    if (game.isPlayer1Turn)
    {
        var outcomes = GenerateTurnOutcomes(game.p1position, game.p1score);
        foreach (var turnOutcome in outcomes)
        {
            GameStatus gameStatus = game with
            {
                p1position = turnOutcome.pos,
                p1score = turnOutcome.score,
                isPlayer1Turn = !game.isPlayer1Turn
            };
            PlayNextRound(gameStatus, times * turnOutcome.times);
        }
    }
    else
    {
        var outcomes = GenerateTurnOutcomes(game.p2position, game.p2score);
        foreach (var turnOutcome in outcomes)
        {
            var gameStatus = game with
            {
                p2position = turnOutcome.pos,
                p2score = turnOutcome.score,
                isPlayer1Turn = !game.isPlayer1Turn
            };
            PlayNextRound(gameStatus, times * turnOutcome.times);
        }
    }
}

IEnumerable<(int pos, int score, long times)> GenerateTurnOutcomes(int playerPosition, int playerScore)
{
    (int p3, int s3) = UpdatePosition(playerPosition, playerScore, 3);
    (int p4, int s4) = UpdatePosition(playerPosition, playerScore, 4);
    (int p5, int s5) = UpdatePosition(playerPosition, playerScore, 5);
    (int p6, int s6) = UpdatePosition(playerPosition, playerScore, 6);
    (int p7, int s7) = UpdatePosition(playerPosition, playerScore, 7);
    (int p8, int s8) = UpdatePosition(playerPosition, playerScore, 8);
    (int p9, int s9) = UpdatePosition(playerPosition, playerScore, 9);
    return new[]    // Precalculated results of three die throws
    {
        (p3, s3, 1L),
        (p4, s4, 3L),
        (p5, s5, 6L),
        (p6, s6, 7L),
        (p7, s7, 6L),
        (p8, s8, 3L),
        (p9, s9, 1L),
    };
}

(int pos, int score) UpdatePosition(int position, int score, int steps)
{
    position = (position + steps) % 10;
    if (position == 0)
        position = 10;

    score += position;
    return (position, score);
}


public record GameStatus
{
    public int p1position { get; init; }
    public int p2position { get; init; }
    public int p1score { get; init; }
    public int p2score { get; init; }
    public bool isPlayer1Turn { get; init; }
    
    public bool IsP1Winning => p1score > p2score;
}
