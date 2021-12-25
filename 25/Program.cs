var masterBoard = Parser.Parse("inputFull.txt");

int iterations = 0;
int movesAtLastStep = Int32.MaxValue;
while (movesAtLastStep > 0)
{
    iterations++;
    if (iterations % 50 == 0)
        Console.WriteLine(iterations);
    
    (masterBoard, movesAtLastStep) = SimulateSingleStep(masterBoard);
}

//PrintBoard(masterBoard);
Console.WriteLine($"Iterations until stop: {iterations}");


(char[,] masterBoard, int) SimulateSingleStep(char[,] board)
{
    (board, int moves1) = SimulateMovementRight(board);
    (board, int moves2) = SimulateMovementDown(board);
    //PrintBoard(board);

    return (board, moves1 + moves2);
}

(char[,] newBoard, int moves) SimulateMovementRight(char[,] board)
{
    int moves = 0;
    
    var ROWS = board.GetLength(0);
    var COLS = board.GetLength(1);
    char[,] newBoard = new char[ROWS, COLS];
    
    for (int i = 0; i < ROWS; i++)
    for (int j = 0; j < COLS; j++)
    {
        if (board[i, j] == '>')
        {
            int col = (j + 1) % COLS;
            if (board[i, col] == '.')
            {
                newBoard[i, j] = '.';
                newBoard[i, col] = '>';
                moves++;
            }
        }
    }
    for (int i = 0; i < ROWS; i++)
    for (int j = 0; j < COLS; j++)
    {
        if (newBoard[i, j] == 0)
            newBoard[i, j] = board[i, j];
    }
    //PrintBoard(newBoard);

    return (newBoard, moves);
}

(char[,] newBoard, int moves) SimulateMovementDown(char[,] board)
{
    int moves = 0;
    
    var ROWS = board.GetLength(0);
    var COLS = board.GetLength(1);
    char[,] newBoard = new char[ROWS, COLS];
    
    for (int i = 0; i < ROWS; i++)
    for (int j = 0; j < COLS; j++)
    {
        if (board[i, j] == 'v')
        {
            int row = (i + 1) % ROWS;
            if (board[row, j] == '.')
            {
                newBoard[i, j] = '.';
                newBoard[row, j] = 'v';
                moves++;
            }
        }
    }
    for (int i = 0; i < ROWS; i++)
    for (int j = 0; j < COLS; j++)
    {
        if (newBoard[i, j] == 0)
            newBoard[i, j] = board[i, j];
    }
    //PrintBoard(newBoard);

    return (newBoard, moves);
}

void PrintBoard(char[,] board)
{
    Console.WriteLine("===");
    for (int i = 0; i < board.GetLength(0); i++)
    {
        for (int j = 0; j < board.GetLength(1); j++)
            Console.Write(board[i, j]);
        Console.WriteLine();
    }
}


public static class Parser
{
    public static char[,] Parse(string fileName)
    {
        var fileLines = File.ReadAllLines(fileName);
        var ROWS = fileLines.Length;
        var COLS = fileLines[0].Length;

        var board = new char[ROWS, COLS];
        for (int i = 0; i < ROWS; i++)
        for (int j = 0; j < COLS; j++)
            board[i, j] = fileLines[i][j];
        return board;
    }
}
