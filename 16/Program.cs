const byte TYPE_LITERAL = 4;
//var input = "D2FE28";
//var input = "38006F45291200";
//var input = "EE00D40C823060";
//var input = "8A004A801A8002F478";
//var input = "620080001611562C8802118E34";
//var input = "C0015000016115A2E0802F182340";
//var input = "A0016C880162017C3686B18A3D4780";

//var input = "C200B40A82";
//var input = "04005AC33890";
//var input = "880086C3E88112";
//var input = "CE00C43D881120";
//var input = "D8005AC2A8F0";
//var input = "F600BC2D8F";
//var input = "9C005AC2F8F0";
//var input = "9C0141080250320F1802104A08"; 
var input = File.ReadAllText("inputFull.txt");

bool[] bits = PuzzleConverter.ToBits(input);
Console.WriteLine($"Input: {input}");
//Console.WriteLine($"Bits: {String.Join(null, bits.Select(b => b ? 1 : 0))}");


var span = (Span<bool>)bits;
var packet = ParsePacket(ref span);
packet.Print();

Console.WriteLine($"Sum of all versions: {packet.SumVersions()}");
Console.WriteLine($"Result: {packet.Calculate()}");





Packet ParsePacket(ref Span<bool> bits)
{
    var versionBits = bits.Slice(0, 3);
    var typeBits = bits.Slice(3, 3);

    int version = PuzzleConverter.ConvertBoolArrayToInt32(versionBits);
    int type = PuzzleConverter.ConvertBoolArrayToInt32(typeBits);

    if (type == TYPE_LITERAL)
    {
        // Literal 
        var literalValue = ParseLiteralValue(bits.Slice(6), out var remainingBits);
        bits = remainingBits;
        return new LiteralPacket(version, type, literalValue);
    }
    else
    {
        // Operator
        bool lengthBit = bits[6];
        if (lengthBit == false)
        {
            // next 15 bits = Number of bits
            var numberOfBits = bits.Slice(7, 15);
            int bitsToRead = PuzzleConverter.ConvertBoolArrayToInt32(numberOfBits);
            var subPacketsBits = bits.Slice(7 + 15, bitsToRead);

            var packets = new List<Packet>();
            while (subPacketsBits.Length > 0)
            {
                var packet = ParsePacket(ref subPacketsBits);
                packets.Add(packet);
            }

            bits = bits.Slice(7 + 15 + bitsToRead);
            return new OperatorPacket(version, type, packets);
        }
        else
        {
            // next 11 bits - Number of sub-packets
            var numberOfPackets = bits.Slice(7, 11);
            int packetsToRead = PuzzleConverter.ConvertBoolArrayToInt32(numberOfPackets);
            bits = bits.Slice(7 + 11);

            var packets = new List<Packet>();
            for (int i = 0; i < packetsToRead; i++)
            {
                var packet = ParsePacket(ref bits);
                packets.Add(packet);
            }
            return new OperatorPacket(version, type, packets);
        }
    }
}

long ParseLiteralValue(Span<bool> bits, out Span<bool> remainingBits)
{
    int readFrom = 0;
    
    Span<bool> chunk;
    List<bool> literalBuffer = new List<bool>();
    do
    {
        chunk = bits.Slice(readFrom, 5);
        readFrom += 5;

        literalBuffer.AddRange(chunk.Slice(1, 4).ToArray()); // allocation!
    } while (chunk[0] == true);

    var number = PuzzleConverter.ConvertBoolArrayToInt64(literalBuffer); // allocation!

    remainingBits = bits.Slice(readFrom);
    return number;
}


abstract class Packet
{
    public int Version { get; }
    public int Type { get; }

    protected Packet(int version, int type)
    {
        Version = version;
        Type = type;
    }

    public abstract void Print(string prefix = "");
    public abstract int SumVersions();
    public abstract long Calculate();
}
class LiteralPacket : Packet
{
    public long LiteralValue { get; }
    
    public LiteralPacket(int version, int type, long literalValueValue)
        : base(version, type)
    {
        LiteralValue = literalValueValue;
    }

    public override string ToString()
    {
        return $"Literal: v{Version}, t{Type} == {LiteralValue}";
    }

    public override void Print(string prefix = "")
    {
        Console.WriteLine($"{prefix}Literal: v{Version}, t{Type} == {LiteralValue}");
    }

    public override int SumVersions()
    {
        return Version;
    }

    public override long Calculate()
    {
        return LiteralValue;
    }
}
class OperatorPacket : Packet
{
    List<Packet> Packets { get; } = new();
    
    public OperatorPacket(int version, int type, List<Packet> packets)
        : base(version, type)
    {
        Packets = packets;
    }

    public override string ToString()
    {
        return $"Operator: v{Version}, t{Type}";
    }

    public override void Print(string prefix = "")
    {
        Console.WriteLine($"{prefix}Operator: v{Version}, t{Type} {TypeString}");
        foreach (var packet in Packets)
        {
            packet.Print($"{prefix}  ");
        }
    }

    public string TypeString => Type switch {
        0 => "+",
        1 => "*",
        2 => "Min",
        3 => "Max",
        5 => ">",
        6 => "<",
        7 => "="
    };

    public override int SumVersions()
    {
        return Version + Packets.Select(p => p.SumVersions()).Sum();
    }

    public override long Calculate()
    {
        return Type switch
        {
            0 => Packets.Select(p => p.Calculate()).Sum(),
            1 => Packets.Select(p => p.Calculate()).Aggregate((el1, el2) => el1 * el2),
            2 => Packets.Select(p => p.Calculate()).Min(),
            3 => Packets.Select(p => p.Calculate()).Max(),
            5 => Packets[0].Calculate() > Packets[1].Calculate() ? 1 : 0,
            6 => Packets[0].Calculate() < Packets[1].Calculate() ? 1 : 0,
            7 => Packets[0].Calculate() == Packets[1].Calculate() ? 1 : 0
        };
    }
}



class PuzzleConverter
{
    private static Dictionary<char, bool[]> _hexToBits = new()
    {
        { '0', new[] { false, false, false, false } },
        { '1', new[] { false, false, false, true } },
        { '2', new[] { false, false, true, false } },
        { '3', new[] { false, false, true, true } },
        { '4', new[] { false, true, false, false } },
        { '5', new[] { false, true, false, true } },
        { '6', new[] { false, true, true, false } },
        { '7', new[] { false, true, true, true } },
        { '8', new[] { true, false, false, false } },
        { '9', new[] { true, false, false, true } },
        { 'A', new[] { true, false, true, false } },
        { 'B', new[] { true, false, true, true } },
        { 'C', new[] { true, true, false, false } },
        { 'D', new[] { true, true, false, true } },
        { 'E', new[] { true, true, true, false } },
        { 'F', new[] { true, true, true, true } },
    };
    
    public static bool[] ToBits(string hexString)
    {
        var bits = new List<bool>(hexString.Length * 4);
        foreach (var ch in hexString)
        {
            bits.AddRange(_hexToBits[ch]);
        }
        return bits.ToArray();
    }
    
    public static int ConvertBoolArrayToInt32(Span<bool> source)
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
    
    public static long ConvertBoolArrayToInt64(IEnumerable<bool> source)
    {
        long result = 0;
        // This assumes the array never contains more than 64 elements!
        int index = 64 - source.Count();

        // Loop through the array
        foreach (bool b in source)
        {
            // if the element is 'true' set the bit at that position
            if (b)
                result |= 1L << (63 - index);

            index++;
        }

        return result;
    }
}

