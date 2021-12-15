using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var fileStream = File.OpenText("input.txt");

// Line 1: Random numbers
var numbersRaw = fileStream.ReadLine();
var randomNumbers = numbersRaw
    .Split(',', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
    .Select(str => Convert.ToInt32(str))
    .ToList();

// Lines 2+: Boards
List<Board> boards = new List<Board>();
while (!fileStream.EndOfStream) {
    fileStream.ReadLine(); // Empty Line

    Board board = new Board();
    for (int i=0; i<5; i++)
    {
        var row = fileStream.ReadLine();
        int[] rowNumbers = row
            .Split(' ', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
            .Select(str => Convert.ToInt32(str))
            .ToArray();
        board.SetRow(i, rowNumbers);
    }
    boards.Add(board);
    
}
fileStream.Close();


// Console.WriteLine($"Numbers: {string.Join(',', randomNumbers)}");
// Console.WriteLine($"Boards: {boards.Count}");
// foreach(var board in boards) {
//     board.Print();
// }

// Puzzle 1
// bool done = false;
// foreach (int drawnNumber in randomNumbers) {
//     if (done)
//         break;
//     foreach (var board in boards) {
//         bool hasWon = board.MarkNumber(drawnNumber);
//         if (hasWon) {
//             int sumUnmarked = board.GetSumOfUnmarkedNumbers();

//             Console.WriteLine($"Drawn number: {drawnNumber}");
//             Console.WriteLine($"Sum unmarked: {sumUnmarked}");
//             Console.WriteLine($"Result: {drawnNumber * sumUnmarked}");

//             board.Print();
//             done = true;
//             break;
//         }
//     }
// }

// Puzzle 2
randomNumbers.Reverse();
Stack<int> inputs = new(randomNumbers);

while (boards.Count > 0)
{
    int drawnNumber = inputs.Pop();
    var remainingBoards = new List<Board>();
    foreach (var board in boards) {
        bool hasWon = board.MarkNumber(drawnNumber);
        if (!hasWon)
            remainingBoards.Add(board);

        if (hasWon && boards.Count == 1)
        {
            // This is the last board!
            int sumUnmarked = board.GetSumOfUnmarkedNumbers();

             Console.WriteLine($"Drawn number: {drawnNumber}");
             Console.WriteLine($"Sum unmarked: {sumUnmarked}");
             Console.WriteLine($"Result: {drawnNumber * sumUnmarked}");

             board.Print();
             return;
        }
    }

    boards = remainingBoards;
}

//
// if (hasWon) {
//     int sumUnmarked = board.GetSumOfUnmarkedNumbers();
//
//     Console.WriteLine($"Drawn number: {drawnNumber}");
//     Console.WriteLine($"Sum unmarked: {sumUnmarked}");
//     Console.WriteLine($"Result: {drawnNumber * sumUnmarked}");
//
//     board.Print();
//     done = true;
//     break;
// }





// Console.WriteLine($"Numbers: {string.Join(',', randomNumbers)}");
// Console.WriteLine($"Boards: {boards.Count}");
// foreach(var board in boards) {
//     board.Print();
// }



class Board {
    int[,] Numbers = new int[5,5];
    bool[,] Marked = new bool[5,5];

    public bool MarkNumber(int drawnNumber) {
        bool marked = false;
        int thisRow = -1;
        int thisCol = -1;
        for (int i=0; i<5 && !marked; i++)
            for (int j=0; j<5 && !marked; j++)
                if (Numbers[i,j] == drawnNumber) {
                    Marked[i,j] = true;
                    thisRow = i;
                    thisCol = j;
                    marked = true;
                }
        
        if (marked) {
            // check if this is a win
            bool rowWon = true;
            bool colWon = true;
            for (int i=0; i<5; i++) {
                rowWon = rowWon && Marked[thisRow, i];
                colWon = colWon && Marked[i, thisCol];
            }
            return rowWon || colWon;
        }
        return false; // Not won
    }

    public int GetSumOfUnmarkedNumbers() {
        int sum = 0;
        for (int i=0; i<5; i++)
            for (int j=0; j<5; j++)
                if (Marked[i,j] == false)
                    sum += Numbers[i,j];
        return sum;
    }

    

    public void SetRow(int i, int[] numbers)
    {
        for (int j = 0; j < 5; j++)
        {
            Numbers[i, j] = numbers[j];
        }
    }
    
    public void Print() {
        for (int i=0; i<5; i++) {
            for (int j=0; j<5; j++) 
                Console.Write("\t" + (Marked[i,j] ? $"[{Numbers[i,j]}]" : Numbers[i,j]));
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}