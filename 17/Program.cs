//var input = new { xLeft = 20, xRight = 30, yBottom = -10, yTop = -5 };
var input = new { xLeft = 201, xRight = 230, yBottom = -99, yTop = -65 };

// Shoot up
var maxYSpeed = Math.Abs(input.yBottom) - 1;
var maxYHeight = 0;
for (int i = 1; i <= maxYSpeed; i++)
     maxYHeight += i;

// Shoot down
var minYSpeed = input.yBottom;

// Min speed - should reach the target after slowing down to 0
var minXSpeed = 0;
var xDistance = 0;
while (xDistance < input.xLeft)
{
     minXSpeed++;
     xDistance += minXSpeed;
}

var maxXSpeed = input.xRight;


Console.WriteLine($"Min Y speed: {minYSpeed},   max Y speed: {maxYSpeed}");
Console.WriteLine($"Min X speed: {minXSpeed},   max X speed: {maxXSpeed}");

// Optimization: Find speeds that guarantee target hit on one coordinate
var xSpeedCandidates = FindXSpeedCandidates(minXSpeed, maxXSpeed).ToList();
var ySpeedCandidates = FindYSpeedCandidates(minYSpeed, maxYSpeed).ToList();

Console.WriteLine($"Viable X speeds: {String.Join(", ", xSpeedCandidates)}");
Console.WriteLine($"Viable Y speeds: {String.Join(", ", ySpeedCandidates)}");

// Run simulation on the speed candidates.
var speeds = new List<(int xv, int yv)>();
foreach (var xSpeed in xSpeedCandidates)
foreach (var ySpeed in ySpeedCandidates)
{
     
     var currentSpeed = (x: xSpeed, y: ySpeed);
     var currentPos = (x: 0, y: 0);

     do
     {
          currentPos.x += currentSpeed.x;
          currentPos.y += currentSpeed.y;
          
          currentSpeed.x = currentSpeed.x > 0
               ? currentSpeed.x - 1
               : 0;
          currentSpeed.y--;

          if (currentPos.x >= input.xLeft && currentPos.x <= input.xRight &&
              currentPos.y >= input.yBottom && currentPos.y <= input.yTop)
          {
               speeds.Add((xSpeed, ySpeed));
               break;
          }
     } while (currentPos.x <= input.xRight && currentPos.y >= input.yBottom);
}

Console.WriteLine($"Speeds: {speeds.Count}");
foreach(var speed in speeds)
     Console.Write($"[{speed.xv,3},{speed.yv,3}]  ");
Console.WriteLine();


IEnumerable<int> FindXSpeedCandidates(int minXSpeed, int maxXSpeed)
{
     for (var xv = minXSpeed; xv <= maxXSpeed; xv++)
     {
          int distance = 0;
          for (int currentSpeed = xv; currentSpeed > 0; currentSpeed--)
          {
               distance += currentSpeed;
               if (distance >= input.xLeft && distance <= input.xRight)
               {
                    yield return xv;
                    break;
               }
          }
     }
}
IEnumerable<int> FindYSpeedCandidates(int minYSpeed, int maxYSpeed)
{
     for (var yv = minYSpeed; yv <= maxYSpeed; yv++)
     {
          int currentHeight = 0;
          int currentYSpeed = yv;
          do
          {
               currentHeight += currentYSpeed;
               if (currentHeight <= input.yTop && currentHeight >= input.yBottom)
               {
                    yield return yv;
                    break;
               }

               currentYSpeed--;
          } while (currentHeight >= input.yBottom);
     }
}
