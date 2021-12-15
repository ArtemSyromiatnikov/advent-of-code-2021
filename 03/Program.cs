// TODO: Consider BitArray

string inputRaw = await File.ReadAllTextAsync("input.txt");
List<bool[]> input = inputRaw
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(str => str.Select(ch => ch == '1').ToArray())
    .ToList();

// Puzzle 1
// bool[] gammaRate = new bool[input[0].Length];
// for (var j=0; j<input[0].Length; j++) {
//     int oneCount = 0;
//     for (int i=0; i<input.Length; i++) {
//         if (input[i][j] == true)
//             oneCount++;
//     }
//     gammaRate[j] = oneCount*2 >= input.Length;
// }
// bool[] epsilonRate = gammaRate.Select(bit => !bit).ToArray();


// Console.WriteLine($"Gamma rate: {string.Join("", gammaRate.Select(bit => bit?1:0))}");
// Console.WriteLine($"Epsilon rate: {string.Join("", epsilonRate.Select(bit => bit?1:0))}");

// string gammaRateString = string.Join("", gammaRate.Select(bit => bit?1:0));
// string epsilonRateString = string.Join("", epsilonRate.Select(bit => bit?1:0));

// int gamma = Convert.ToInt32(gammaRateString, 2);
// int epsilon = Convert.ToInt32(epsilonRateString, 2);

// Console.WriteLine($"Gamma: {gamma}, Epsilon: {epsilon}");
// Console.WriteLine($"Result: {gamma*epsilon}");


// Puzzle 2
//List<byte[]> tempArray = input.ToList<byte[]>();

bool[] o2Rate = Filter(input, false);
bool[] co2Rate = Filter(input, true);

string o2String = BoolArrayToString(o2Rate);
string co2String = BoolArrayToString(co2Rate);

int o2 = Convert.ToInt32(o2String, 2);
int co2 = Convert.ToInt32(co2String, 2);

Console.WriteLine($"o2: {o2}, co2: {co2}");
Console.WriteLine($"Result: {o2*co2}");


bool[] Filter(List<bool[]> input, bool co2mode) {
    int col = 0;
    while (input.Count > 1) {
        int onesCount = 0;
        for (int i=0; i<input.Count; i++) {
            if (input[i][col] == true)
                onesCount++;
        }
        bool numsToKeep = onesCount*2 >= input.Count;
        if (co2mode)
            numsToKeep = !numsToKeep;            

        input = input.Where(row => row[col] == numsToKeep).ToList();
        col++;
        //Console.WriteLine($"NumsToKeep: {(numsToKeep?1:0)}, Kept: {input.Count}");
        //Console.WriteLine($"  NUmbers: {string.Join(",", input.Select(BoolArrayToString))}");
    }
    if (input.Count != 1)
        throw new Exception($"Wrong number of outputs: {input.Count}");
    return input[0];
}

string BoolArrayToString(IEnumerable<bool> input) {
    return string.Join("", input.Select(bit => bit?1:0));
}