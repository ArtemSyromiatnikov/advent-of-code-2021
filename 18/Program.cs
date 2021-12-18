using System.Text;
using System.Text.Json;

//var input = @"[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]";

//var number1 = ParseInputString(@"[[9,[3,8]],[[0,9],6]]");
//var number2 = ParseInputString(@"[[1,9],[8,5]]");
//Console.WriteLine(FishMath.SimpleAdd(number1, number2));



// Explosions Study
// var numbers = new FishNumber[]
// {
//     ParseInputString("[[[[[9,8],1],2],3],4]"),
//     ParseInputString("[7,[6,[5,[4,[3,2]]]]]"),
//     ParseInputString("[[6,[5,[4,[3,2]]]],1]"),
//     ParseInputString("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]"),
//     ParseInputString("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]"),
// };
// foreach (var number in numbers)
// {
//     Console.WriteLine($"Original: {number}");
//     //Console.WriteLine($"Pair to explode: {number.FindNumberToExplode()}");
//     var reduced = FishMath.Reduce(number);
//     Console.WriteLine($"Reduced : {reduced}");
// }


// Reduction study
// var n1 = ParseInputString("[[[[4,3],4],4],[7,[[8,4],9]]]");
// var n2 = ParseInputString("[1,1]");
// var n3 = FishMath.Add(n1, n2);
// Console.WriteLine($"Reduced : {n3}");


// Sum study
// var input = @"
// [[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
// [7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
// [[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
// [[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
// [7,[5,[[3,8],[1,4]]]]
// [[2,[2,2]],[8,[8,1]]]
// [2,9]
// [1,[[[9,3],9],[[9,0],[0,7]]]]
// [[[5,[7,4]],7],1]
// [[[[4,2],2],6],[8,7]]".Trim();
// var numbers = input.Split(Environment.NewLine)
//     .Select(line => ParseInputString(line))
//     .ToList();
//
// var number = numbers[0];
// for (int i = 1; i < numbers.Count; i++)
// {
//     number = FishMath.Add(number, numbers[i]);
//     Console.WriteLine($"Added number {i,2}: {number}");
// }

// Puzzle 1
// var input = @"
// [[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
// [[[5,[2,8]],4],[5,[[9,9],0]]]
// [6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
// [[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
// [[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
// [[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
// [[[[5,4],[7,7]],8],[[8,3],8]]
// [[9,3],[[9,9],[6,[4,9]]]]
// [[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
// [[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]".Trim();

var input = File.ReadAllText("inputFull.txt");
var numbers = input.Split(Environment.NewLine)
    .Select(line => ParseInputString(line))
    .ToList();

// Puzzle 1
// var number = numbers[0];
// for (int i = 1; i < numbers.Count; i++)
// {
//     number = FishMath.Add(number, numbers[i]);
//     Console.WriteLine($"Added number {i,2}: {number}");
// }
//Console.WriteLine($"Result magnitude: {number.CalculateMagnitude()}");

// Puzzle 2
FishNumber bestLeft = null;
FishNumber bestRight = null; 
long maxMagnitude = 0;
for (int i = 0; i < numbers.Count-1; i++)
for (int j = i+1; j < numbers.Count; j++)
{
    {
        var sum = FishMath.Add(numbers[i], numbers[j]);
        var magnitude = sum.CalculateMagnitude();
        if (magnitude > maxMagnitude)
        {
            bestLeft = numbers[i];
            bestRight = numbers[j];
            maxMagnitude = magnitude;
        }
    }
    {
        var sum = FishMath.Add(numbers[j], numbers[i]);
        var magnitude = sum.CalculateMagnitude();
        if (magnitude > maxMagnitude)
        {
            bestLeft = numbers[j];
            bestRight = numbers[i];
            maxMagnitude = magnitude;
        }
    }
}


Console.WriteLine($"Best left: {bestLeft}");
Console.WriteLine($"Best right: {bestRight}");
Console.WriteLine($"Max magnitude: {maxMagnitude}");







#region Parsing
FishNumber ParseInputString(String inputStr)
{
    var json = JsonSerializer.Deserialize<JsonElement>(new MemoryStream(Encoding.UTF8.GetBytes(inputStr)));
    return (FishNumber)JsonToFishNumber(json);
} 
FishNumberValue JsonToFishNumber(JsonElement json)
{
    if (json.ValueKind == JsonValueKind.Number)
    {
        return new FishLiteral(json.GetInt32());
    }
    if (json.ValueKind == JsonValueKind.Array)
    {
        var left = JsonToFishNumber(json[0]);
        var right = JsonToFishNumber(json[1]);
        return new FishNumber(left, right);
    }

    throw new Exception($"Bad Input: {json}");
}
#endregion


static class FishMath
{
    public static FishNumber Add(FishNumber a, FishNumber b)
    {
        var number = new FishNumber(a.Clone(), b.Clone());
        return Reduce(number);
    }

    private static FishNumber Reduce(FishNumber number)
    {
        bool reduced = false;
        do
        {
            reduced = false;

            var explosive = number.FindNumberToExplode();
            if (explosive != null)
            {
                number = explosive.Explode();
                reduced = true;
                //Console.WriteLine($"after explode: {number}");
            }
            else
            {
                var splittable = number.FindNumberToSplit();
                if (splittable != null)
                {
                    number = splittable.Split();
                    reduced = true;
                    //Console.WriteLine($"after split:   {number}");
                }
            }
        } while (reduced);


        return number;
    }

}



public class FishNumber: FishNumberValue
{
    public FishNumber(FishNumberValue left, FishNumberValue right)
    {
        Left = left;
        Right = right;
        Left.Parent = this;
        Right.Parent = this;
    }
    
    public FishNumberValue Left { get; set; }
    public FishNumberValue Right { get; set; }

    public bool IsSimplePair =>
        Left is FishLiteral && Right is FishLiteral;

    // Explosion criteria:
    // - Pair should contain 2 simple numbers
    // - Nesting level should be >= 4
    public FishNumber? FindNumberToExplode(int currentDepth = 0)
    {
        if (IsSimplePair && currentDepth >= 4)
            return this;
        
        if (Left is FishNumber leftNumber)
        {
            var candidate = leftNumber.FindNumberToExplode(currentDepth + 1);
            if (candidate != null)
                return candidate;
        }
        
        if (Right is FishNumber rightNumber)
        {
            var candidate = rightNumber.FindNumberToExplode(currentDepth + 1);
            if (candidate != null)
                return candidate;
        }

        return null;
    }
    
    // Criteria for splittable:
    // Literal value is >= 10
    public override FishLiteral? FindNumberToSplit() =>
        Left.FindNumberToSplit() ??
        Right.FindNumberToSplit();


    public FishNumber Explode()
    {
        if (!this.IsSimplePair)
            throw new Exception("Attempting to explode not-so-simple Pair!");
        
        var leftNeighbor = Parent.FindLiteralToTheLeftFrom(this);
        if (leftNeighbor != null)
            leftNeighbor.Value += ((FishLiteral)this.Left).Value;

        var rightNeighbor = Parent.FindLiteralToTheRightFrom(this);
        if (rightNeighbor != null)
            rightNeighbor.Value += ((FishLiteral)this.Right).Value;

        if (Parent.Left == this)
            Parent.Left = new FishLiteral(0, Parent);
        else 
            Parent.Right = new FishLiteral(0, Parent);

        return GetTopParent();
    }

    public FishLiteral? FindLiteralToTheLeftFrom(FishNumber from)
    {
        if (from == Left)
            return Parent?.FindLiteralToTheLeftFrom(this);

        else if (from == Right)
            return Left.GetRightmostLiteral();

        throw new Exception("Exception in FindLiteralToTheLeftFrom");
    }

    public FishLiteral? FindLiteralToTheRightFrom(FishNumber from)
    {
        if (from == Right)
            return Parent?.FindLiteralToTheRightFrom(this);

        else if (from == Left)
            return Right.GetLeftmostLiteral();

        throw new Exception("Exception in FindLiteralToTheRightFrom");
    }


    public override FishLiteral GetRightmostLiteral() => Right.GetRightmostLiteral();

    public override FishLiteral GetLeftmostLiteral() => Left.GetLeftmostLiteral();

    public override string ToString()
    {
        return $"[{Left},{Right}]";
    }

    public override FishNumber Clone() => new FishNumber(Left.Clone(), Right.Clone());

    public override long CalculateMagnitude() =>
        3 * Left.CalculateMagnitude() + 2 * Right.CalculateMagnitude();
}
public abstract class FishNumberValue
{
    public FishNumber? Parent { get; set; }
    
    public FishNumber GetTopParent() => Parent?.GetTopParent() ?? (FishNumber)this;
    
    public abstract FishLiteral GetRightmostLiteral();
    public abstract FishLiteral GetLeftmostLiteral();
    public abstract FishLiteral? FindNumberToSplit();
    public abstract long CalculateMagnitude();
    
    public abstract FishNumberValue Clone();
}
public class FishLiteral : FishNumberValue
{
    public FishLiteral(int value)
    {
        Value = value;
    }
    
    public FishLiteral(int value, FishNumber parent)
    {
        Value = value;
        Parent = parent;
    }

    public int Value { get; set; }

    public override string ToString()
    {
        return Value.ToString();
    }
    
    public override FishLiteral GetRightmostLiteral() => this;
    public override FishLiteral GetLeftmostLiteral() => this;
    public override FishLiteral? FindNumberToSplit() => Value >= 10 ? this : null;

    public FishNumber Split()
    {
        var splitted = new FishNumber(
            new FishLiteral((int)Math.Floor(Value / 2.0)),
            new FishLiteral((int)Math.Ceiling(Value / 2.0)));

        splitted.Parent = this.Parent;
        if (this == Parent.Left)
            Parent.Left = splitted;
        else
            Parent.Right = splitted;

        return splitted.GetTopParent();
    }

    public override long CalculateMagnitude() => this.Value;
    public override FishNumberValue Clone() => new FishLiteral(Value);
}
