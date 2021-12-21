

const bool WHITE = true;
const bool BLACK = false;

var lines = File.ReadAllLines("inputFull.txt");


var symbols = lines[0].Select(ch => ch == '#' ? true : false).ToArray();
var image = ParseImage(lines[2..]);
//PrintImage(BLACK, image);

bool bgColor = BLACK; // false = 0 = black

// in beginning, expand it twice
image = ExpandCanvas(image, bgColor);
//PrintImage(bgColor, image);

int times = 0;
while (times++ < 50)
{
    Console.WriteLine(times);
    image = ExpandCanvas(image, bgColor);
    (image, bgColor) = EnhanceImage(image, symbols, bgColor);
    //PrintImage(bgColor, image);
}

var whitePixels = AsEnumerable(image).Count(px => px == WHITE);
Console.WriteLine($"White pixels: {whitePixels}");





















static (bool[,] image, bool bgColor) EnhanceImage(bool[,] image, bool[] symbols, bool bgColor)
{
    bool blackBecomesWhite = symbols[0] == WHITE;
    bool whiteBecomesBlack = symbols[511] == BLACK;
    bool newBgColor = bgColor;
    if (bgColor == WHITE && whiteBecomesBlack)
        newBgColor = BLACK;
    else if (bgColor == BLACK && blackBecomesWhite)
        newBgColor = WHITE;
    
    // image comes with guaranteed double-padding!
    var size = image.GetLength(0);
    var canvas = new bool[size, size];
    
    for (int i = 1; i < size-1; i++)
    for (int j = 1; j < size-1; j++)
    {
        bool[] bits = {
            image[i-1, j-1], image[i-1, j], image[i-1, j+1],
            image[i,   j-1], image[i,   j], image[i,   j+1],
            image[i+1, j-1], image[i+1, j], image[i+1, j+1]
        };
        var number = ConvertBoolArrayToInt32(bits);
        canvas[i, j] = symbols[number];
    }
    
    // Fill padding with bg color
    for (int i = 0; i < size; i++)
    {
        canvas[i, 0] = newBgColor;
        canvas[i, size-1] = newBgColor;
        canvas[0, i] = newBgColor;
        canvas[size-1, i] = newBgColor;
    }

    return (canvas, newBgColor);
}



static bool[,] ExpandCanvas(bool[,] image, bool backgroundColor)
{
    var origSize = image.GetLength(0);
    var size = origSize + 2;
    var canvas = new bool[size, size];
    
    // Copy image
    for (int i = 0; i < origSize; i++)
    for (int j = 0; j < origSize; j++)
    {
        canvas[i+1, j+1] = image[i,j];
    }

    // Fill padding with bg color
    for (int i = 0; i < size; i++)
    {
        canvas[i, 0] = backgroundColor;
        canvas[i, size-1] = backgroundColor;
        canvas[0, i] = backgroundColor;
        canvas[size-1, i] = backgroundColor;
    }

    return canvas;
}

bool[,] ParseImage(string[] lines)
{
    int originalSize = lines.Length;
    var image = new bool[originalSize, originalSize];
    for (int i = 0; i < originalSize; i++)
    for (int j = 0; j < originalSize; j++)
    {
        image[i, j] = lines[i][j] == '#';
    }

    return image;
}
static void PrintImage(bool bgColor, bool[,] image)
{
    int size = image.GetLength(0);
    
    Console.WriteLine($"Background: {(bgColor ? "White [#]" : "Black [.]")}");
    for (int i = 0; i < size; i++)
    {
        for (int j = 0; j < size; j++)
        {
            Console.Write(image[i, j] ? '#' : '.');
        }
        Console.WriteLine();
    }
}

static int ConvertBoolArrayToInt32(bool[] source)
{
    int result = 0;
    // This assumes the array never contains more than 32 elements!
    int index = 32 - source.Length;

    // Loop through the array
    foreach (bool b in source)
    {
        // if the element is 'true' set the bit at that position
        if (b)
            result |= (int)(1 << (31 - index));

        index++;
    }

    return result;
}

static IEnumerable<bool> AsEnumerable(bool[,] array)
{
    for (int i = 0; i < array.GetLength(0); i++)
    for (int j = 0; j < array.GetLength(1); j++)
        yield return array[i, j];
}