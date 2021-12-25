using System.Diagnostics;

var alu = new AluProgram();

Stopwatch sw = Stopwatch.StartNew();
long iteration = 1;


//var serial = new[] { 9,9,9,9,9,8,4,9,3,3,7,5,5,5 };
//alu.Execute(serial, true);



for(int d1 = 9; d1 >= 1; d1--)
for(int d2 = 4; d2 >= 1; d2--)
for(int d4 = 9; d4 >= 3; d4--)
for(int d5 = 2; d5 >= 1; d5--)
for(int d7 = 9; d7 >= 8; d7--)
for(int d10 = 9; d10 >= 4; d10--)
{
    int d3 = 9;
    int d6 = d5 + 7;
    int d8 = d7 - 7;
    int d9 = d4 - 2;
    int d11 = d10 - 3;
    int d12 = 1;
    int d13 = d2 + 5;
    int d14 = d1;
    iteration++;
    if (iteration % 10_000_000 == 0)
        Console.WriteLine($"Iteration: {iteration}, time passed: {sw.Elapsed}");
    
    var serial = new[] { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14 };
    alu.Reset();
    alu.Execute(serial, true);
    //Console.WriteLine(alu.z);
    if (alu.z == 0)
    {
        Console.WriteLine(String.Join(null, serial));
        return;
    }
}


public class AluProgram
{
    public int z;

    public void Reset()
    {
        z = 0;
    } 

    public void Execute(int[] input, bool verbose)
    {
        ProgramStep(input[0], 1, 13,  6);
        ProgramStep(input[1], 1, 11,  11);
        ProgramStep(input[2], 1, 12,  5); 
        ProgramStep(input[3], 1, 10,  6);
        
        ProgramStep(input[4], 1, 14,  8);
        ProgramStep(input[5], 26,  -1,  14);
        ProgramStep(input[6], 1, 14,  9);
        ProgramStep(input[7], 26,  -16, 4);
        
        ProgramStep(input[8], 26, -8,  7);
        ProgramStep(input[9], 1, 12,  13);
        ProgramStep(input[10],26, -16, 11);
        ProgramStep(input[11],26, -13, 11);
        
        ProgramStep(input[12], 26, -6,  6);
        ProgramStep(input[13], 26, -6,  1);
    }
    /*
    public void ProgramStepGeneric(int input, int arg5, int arg6, int arg16)
    {
        // inp w
        int w = input;
        
        // mul x 0
        // add x z
        // mod x 26
        int x = z % 26;
        
        // div z [arg5]   // arg5 is either 1 or 26
        z = (int)Math.Truncate(1.0 * z / arg5);
        // add x [arg6]   // arg6 can be both positive or negative integers
        x = x + arg6;
        
        // eql x w
        // eql x 0
        x = (x == w) ? 0 : 1;
        
        // mul y 0
        // add y 25
        // mul y x
        // add y 1
        y = 25 * x + 1;      // y = either 1 or 26
        // mul z y
        z = z * y;
        
        // mul y 0
        // add y w
        // add y [arg16]     // positive integers
        // mul y x
        y = (w + arg16) * x; // y = either 0 or (w + arg16)
        
        // add z y
        z = z + y;
    }
    */
    
    public void ProgramStep(int w,  int a, int b, int c)
    {
        int x = z % 26 + b;
        
        z = (int)Math.Truncate(1.0 * z / a);       // Z CAN BECOME 0 HERE!
        
        if (x != w)
        {
            z = z * 26 + w + c;
        }
    }

}