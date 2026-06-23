using System.Text.RegularExpressions;

namespace ValorantReplayParser;

public static class ValorantSeededPayloadTransform
{
    private const ulong Multiplier = 0x2545f4914f6cdd1dUL;
    private const uint SeedAddendV12_10 = 0x12fd0ee5u;
    private const uint SeedAddendV12_11 = 0x409d36a3u;
    private const uint SeedAddendV13_00 = 0x2949b6efu;
    private const uint InitAOffsetV12_10 = 0x1bu;
    private const uint InitAOffsetV12_11 = 0x23u;
    private const uint InitAOffsetV13_00 = 0x11u;
    private const byte TailXorV12_10 = 0xe5;
    private const byte TailXorV12_11 = 0xa3;
    private const byte TailXorV13_00 = 0xef;

    public static byte[] Apply(byte[] payload, int bitCount, uint seed, string? branch = null) =>
        ResolveVersion(branch) switch
        {
            TransformVersion.V13_00 => ApplyV13_00(payload, bitCount, seed),
            TransformVersion.V12_11 => ApplyV12_11(payload, bitCount, seed),
            _ => ApplyV12_10(payload, bitCount, seed),
        };

    private static TransformVersion ResolveVersion(string? branch)
    {
        if (string.IsNullOrEmpty(branch))
            return TransformVersion.V12_10;

        var match = Regex.Match(branch, @"release-(\d+)\.(\d+)", RegexOptions.IgnoreCase);
        if (!match.Success)
            return TransformVersion.V12_10;

        var major = int.Parse(match.Groups[1].Value);
        var minor = int.Parse(match.Groups[2].Value);
        return (major, minor) switch
        {
            (13, 0) => TransformVersion.V13_00,
            (12, 11) => TransformVersion.V12_11,
            _ => TransformVersion.V12_10,
        };
    }

    private static byte[] ApplyV12_10(byte[] payload, int bitCount, uint seed)
    {
        var output = payload.ToArray();
        var state = seed;
        var streamByte = (byte)seed;
        var prngA = InitialPrngA(seed);
        var prngB = InitialPrngB(seed);
        var byteOffset = 0;
        var bitsRemaining = bitCount;

        while (bitsRemaining > 63)
        {
            var value = BitConverter.ToUInt64(output, byteOffset);
            var constants = GetTransformConstants64(state);
            value = RotateRight(value, constants.Rotate2);
            value = SwapAdjacentBits(value);
            value -= constants.Addend2;
            value = RotateRight(value, constants.Rotate1);
            value = SwapAdjacentBits(value ^ ~(ulong)constants.Addend1);
            WriteUInt64(output, byteOffset, value);
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset += 8;
            bitsRemaining -= 64;
        }

        while (bitsRemaining > 31)
        {
            var value = BitConverter.ToUInt32(output, byteOffset);
            var constants = GetTransformConstants32(state);
            value = RotateRight(value, constants.Rotate2);
            value = SwapAdjacentBits(value);
            value -= constants.Addend2;
            value = RotateRight(value, constants.Rotate1);
            value = SwapAdjacentBits(value ^ constants.Addend1);
            WriteUInt32(output, byteOffset, value);
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset += 4;
            bitsRemaining -= 32;
        }

        while (bitsRemaining > 7)
        {
            var value = output[byteOffset];
            var constants = GetTransformConstants8(state);
            value = RotateRight(value, constants.Rotate2);
            value = SwapAdjacentBits(value);
            value -= constants.Addend2;
            value = RotateRight(value, constants.Rotate1);
            value = SwapAdjacentBits((byte)(value ^ constants.Addend1));
            output[byteOffset] = value;
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset++;
            bitsRemaining -= 8;
        }

        if (bitsRemaining != 0)
        {
            var mask = (byte)(0xff >> (7 - ((bitCount - 1) & 7)));
            output[byteOffset] ^= (byte)(mask & (streamByte ^ TailXorV12_10));
        }

        return output;
    }

    private static byte[] ApplyV12_11(byte[] payload, int bitCount, uint seed)
    {
        var output = payload.ToArray();
        var state = seed;
        var streamByte = (byte)seed;
        var prngA = InitialPrngAV12_11(seed);
        var prngB = InitialPrngB(seed);
        var byteOffset = 0;
        var bitsRemaining = bitCount;

        while (bitsRemaining > 63)
        {
            var value = BitConverter.ToUInt64(output, byteOffset);
            var ror2 = RotateRight(state, 2);
            var ror3 = RotateRight(state, 3);
            var ror4 = RotateRight(state, 4);
            var ror6 = RotateRight(state, 6);
            var ror8 = RotateRight(state, 8);

            value = RotateRight(value, (int)(ror8 % 63) + 1);
            value = SwapAdjacentBits(value);
            value += ror6;
            value = ShuffleBits64V12_11(value);
            value -= ror4;
            value -= ror3;
            value -= ror2;
            value = SwapAdjacentBits(value);

            WriteUInt64(output, byteOffset, value);
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset += 8;
            bitsRemaining -= 64;
        }

        while (bitsRemaining > 31)
        {
            var value = BitConverter.ToUInt32(output, byteOffset);
            var rol2 = RotateLeft(state, 2);
            var rol3 = RotateLeft(state, 3);
            var rol4 = RotateLeft(state, 4);
            var rol6 = RotateLeft(state, 6);
            var rol8 = RotateLeft(state, 8);

            value = RotateRight(value, (int)(rol8 % 31) + 1);
            value = SwapAdjacentBits(value);
            value += rol6;
            value = ReverseBits32(value);
            value -= rol4;
            value -= rol3;
            value -= rol2;
            value = SwapAdjacentBits(value);

            WriteUInt32(output, byteOffset, value);
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset += 4;
            bitsRemaining -= 32;
        }

        while (bitsRemaining > 7)
        {
            var value = output[byteOffset];
            var stateByte = (byte)state;
            var rotate2Input = state * 0x0cc6db61u;

            value = RotateRight(value, (int)(rotate2Input % 7) + 1);
            value = SwapAdjacentBits(value);
            value += (byte)(stateByte * 0x29);
            value = ReverseBits8(value);
            value += (byte)(stateByte * 0x23);
            value = SwapAdjacentBits(value);

            output[byteOffset] = value;
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset++;
            bitsRemaining -= 8;
        }

        if (bitsRemaining != 0)
        {
            var mask = (byte)(0xff >> (7 - ((bitCount - 1) & 7)));
            output[byteOffset] ^= (byte)(mask & (streamByte ^ TailXorV12_11));
        }

        return output;
    }

    private static byte[] ApplyV13_00(byte[] payload, int bitCount, uint seed)
    {
        var output = payload.ToArray();
        var state = seed;
        var streamByte = (byte)seed;
        var prngA = InitialPrngAV13_00(seed);
        var prngB = InitialPrngB(seed);
        var byteOffset = 0;
        var bitsRemaining = bitCount;

        while (bitsRemaining > 63)
        {
            var value = BitConverter.ToUInt64(output, byteOffset);
            var ror1 = RotateRight(state, 1);
            var ror3 = RotateRight(state, 3);
            var ror6 = RotateRight(state, 6);
            var ror8 = RotateRight(state, 8);

            value += ror8;
            value = ShuffleBits64V12_11(value);
            value = (value + ror6) ^ ror3;
            value = SubstituteBytes(value, SubstituteTable64V13_00);
            value = RotateRight(value, (int)(ror1 % 63) + 1);

            WriteUInt64(output, byteOffset, value);
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset += 8;
            bitsRemaining -= 64;
        }

        while (bitsRemaining > 31)
        {
            var value = BitConverter.ToUInt32(output, byteOffset);
            var rol1 = RotateLeft(state, 1);
            var rol3 = RotateLeft(state, 3);
            var rol6 = RotateLeft(state, 6);
            var rol8 = RotateLeft(state, 8);

            value += rol8;
            value = ReverseBits32(value);
            value = ~(value + rol6) ^ rol3;
            value = SubstituteBytes(value, SubstituteTable32V13_00);
            value = RotateRight(value, (int)(rol1 % 31) + 1);

            WriteUInt32(output, byteOffset, value);
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset += 4;
            bitsRemaining -= 32;
        }

        while (bitsRemaining > 7)
        {
            var value = output[byteOffset];
            var mix = state * 0x533u;

            value = (byte)(value + (byte)mix * 0x1b);
            value = ReverseBits8(value);
            value = (byte)(~(value + (byte)mix * 0x33) ^ (byte)mix);
            value = SubstituteTable8V13_00[value];
            value = RotateRight(value, (int)(state * 0x0bu % 7) + 1);

            output[byteOffset] = value;
            AdvanceTransformState(ref state, ref prngA, ref prngB, out streamByte);
            byteOffset++;
            bitsRemaining -= 8;
        }

        if (bitsRemaining != 0)
        {
            var mask = (byte)(0xff >> (7 - ((bitCount - 1) & 7)));
            output[byteOffset] ^= (byte)(mask & (streamByte ^ TailXorV13_00));
        }

        return output;
    }

    private static ulong InitialPrngA(uint seed)
    {
        var seedPlus = seed + SeedAddendV12_10;
        var mixed = ((seedPlus >> 15) ^ seedPlus) >> 12 ^ ((seed - InitAOffsetV12_10) * 0x02000000u) ^ seedPlus;
        return mixed * Multiplier;
    }

    private static ulong InitialPrngAV12_11(uint seed)
    {
        var seedPlus = seed + SeedAddendV12_11;
        var mixed = ((seedPlus >> 15) ^ seedPlus) >> 12 ^ ((seed + InitAOffsetV12_11) * 0x02000000u) ^ seedPlus;
        return mixed * Multiplier;
    }

    private static ulong InitialPrngAV13_00(uint seed)
    {
        var seedPlus = seed + SeedAddendV13_00;
        var mixed = ((seedPlus >> 15) ^ seedPlus) >> 12 ^ ((seed - InitAOffsetV13_00) * 0x02000000u) ^ seedPlus;
        return mixed * Multiplier;
    }

    private static ulong InitialPrngB(uint seed)
    {
        var mixed = ((seed >> 15) ^ seed) >> 12 ^ (seed << 25) ^ seed;
        return mixed * Multiplier;
    }

    private static void AdvanceTransformState(ref uint state, ref ulong prngA, ref ulong prngB, out byte streamByte)
    {
        var sum = prngB + prngA;
        prngB ^= prngA;
        prngA = RotateRight(prngA, 9) ^ (prngB << 14) ^ prngB;
        prngB = RotateLeft(prngB, 36);
        state = (uint)(sum >> 32);
        streamByte = (byte)state;
    }

    private static TransformConstants64 GetTransformConstants64(uint state)
    {
        var ror1 = (state >> 1) | (state << 31);
        var ror2 = (ror1 >> 1) | (ror1 << 31);
        var ror3 = (ror2 >> 1) | (ror2 << 31);
        var ror4 = (ror3 >> 1) | (ror3 << 31);
        var ror5 = (ror4 >> 1) | (ror4 << 31);
        var ror6 = (ror5 >> 1) | (ror5 << 31);
        var ror7 = (ror6 >> 1) | (ror6 << 31);
        var ror8 = (ror7 >> 1) | (ror7 << 31);

        return new TransformConstants64(ror4, ror6, (int)(ror5 % 63) + 1, (int)(ror8 % 63) + 1);
    }

    private static TransformConstants32 GetTransformConstants32(uint state)
    {
        var rot1 = (state << 1) | (state >> 31);
        var rot2 = (rot1 << 1) | (rot1 >> 31);
        var rot3 = (rot2 << 1) | (rot2 >> 31);
        var rot4 = (rot3 << 1) | (rot3 >> 31);
        var rot5 = (rot4 << 1) | (rot4 >> 31);
        var rot6 = (rot5 << 1) | (rot5 >> 31);
        var rot7 = (rot6 << 1) | (rot6 >> 31);
        var rot8 = (rot7 << 1) | (rot7 >> 31);

        return new TransformConstants32(rot4, rot6, (int)(rot5 % 31) + 1, (int)(rot8 % 31) + 1);
    }

    private static TransformConstants8 GetTransformConstants8(uint state) =>
        new((byte)(state * 0x31), (byte)(state * 0x29), (int)(state * 0x2751b % 7) + 1, (int)(state * 0xcc6db61 % 7) + 1);

    private static ulong SwapAdjacentBits(ulong value) =>
        ((value & 0x5555555555555555UL) << 1) | ((value >> 1) & 0x5555555555555555UL);

    private static uint SwapAdjacentBits(uint value) =>
        ((value & 0x55555555u) << 1) | ((value >> 1) & 0x55555555u);

    private static byte SwapAdjacentBits(byte value) =>
        (byte)(((value & 0x55) << 1) | ((value >> 1) & 0x55));

    private static ulong ShuffleBits64V12_11(ulong value)
    {
        value = ((value & 0x5555555555555555UL) << 1) | ((value >> 1) & 0x5555555555555555UL);
        value = ((value & 0x3333333333333333UL) << 2) | ((value >> 2) & 0x3333333333333333UL);
        value = ((value & 0x0F0F0F0F0F0F0F0FUL) << 4) | ((value >> 4) & 0x0F0F0F0F0F0F0F0FUL);
        value = ((value & 0x00FF00FF00FF00FFUL) << 8) | ((value >> 8) & 0x00FF00FF00FF00FFUL);
        // Native 12.11 swaps the 32-bit halves here, but does not perform the usual 16-bit swap.
        value = (value << 32) | (value >> 32);
        return value;
    }

    private static uint ReverseBits32(uint value)
    {
        value = ((value & 0x55555555u) << 1) | ((value >> 1) & 0x55555555u);
        value = ((value & 0x33333333u) << 2) | ((value >> 2) & 0x33333333u);
        value = ((value & 0x0F0F0F0Fu) << 4) | ((value >> 4) & 0x0F0F0F0Fu);
        value = ((value & 0x00FF00FFu) << 8) | ((value >> 8) & 0x00FF00FFu);
        value = (value << 16) | (value >> 16);
        return value;
    }

    private static byte ReverseBits8(byte value)
    {
        value = (byte)(((value & 0x55) << 1) | ((value >> 1) & 0x55));
        value = (byte)(((value & 0x33) << 2) | ((value >> 2) & 0x33));
        value = (byte)(((value & 0x0F) << 4) | ((value >> 4) & 0x0F));
        return value;
    }

    private static ulong SubstituteBytes(ulong value, byte[] table)
    {
        var result = 0UL;
        for (var i = 0; i < sizeof(ulong); i++)
        {
            result |= (ulong)table[(byte)(value >> (i * 8))] << (i * 8);
        }

        return result;
    }

    private static uint SubstituteBytes(uint value, byte[] table)
    {
        var result = 0u;
        for (var i = 0; i < sizeof(uint); i++)
        {
            result |= (uint)table[(byte)(value >> (i * 8))] << (i * 8);
        }

        return result;
    }

    private static ulong RotateLeft(ulong value, int count) =>
        (value << count) | (value >> (64 - count));

    private static uint RotateLeft(uint value, int count) =>
        (value << count) | (value >> (32 - count));

    private static ulong RotateRight(ulong value, int count) =>
        (value >> count) | (value << (64 - count));

    private static uint RotateRight(uint value, int count) =>
        (value >> count) | (value << (32 - count));

    private static byte RotateRight(byte value, int count) =>
        (byte)((value >> count) | (value << (8 - count)));

    private static void WriteUInt64(byte[] output, int byteOffset, ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, output, byteOffset, sizeof(ulong));
    }

    private static void WriteUInt32(byte[] output, int byteOffset, uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, output, byteOffset, sizeof(uint));
    }

    private static readonly byte[] SubstituteTable32V13_00 = Convert.FromHexString(
        "2167b396313fbad3d5062b16f1b651a79c7b419584251536a4703546b05fa6c3" +
        "bb8638f62ea2a994831b6239f3d228149e9af2c9decc26a1d8d0748d69127189" +
        "f758cd4db7114809b968c77cf42042f56b54756da81d6a07d7c50ea066db" +
        "f899ad1004ff8fb1ef986c29e201183d371e654b4a6e24d9bd90fe135693" +
        "34aa8b0d79e74992f98eca43cbc6da022d8c0fb2c08a4785aee0d477c40b" +
        "5c617e335745e62ffd6f915b9fcf3c4fe33aede480087372ea63fbfcb8" +
        "7a23a51f815952875dfa78c1b5beb4a3641c3253f07fdc3b7640ec309755" +
        "4c00bc880c05e1df197d22c25a9be52a50bf1ac8035e2cd1abdd44ee82" +
        "ce27afebd64e0ae9173e9de8ac60");

    private static readonly byte[] SubstituteTable64V13_00 = Convert.FromHexString(
        "77b9042feb7d27c944739a3f36f565ddf7e0302da9985dde69a394a05e170678" +
        "a4f6ab0343c828e56a8e1cf270cf5305d30dffa7a23a32255a1f48c1" +
        "b7e16e85996047bbe48acbc01bea6164f0c2d88bcdfdadb819b5bf0e9181" +
        "839d45d249e9c731bd20bec66680d179d7e6fca15b5fdff1d0506752fe" +
        "7b3513f846b3758de33e2ef4dc342a0823e20c094beec30f248f544c" +
        "5539cc1d1e3b2272da296b41aaa6122c93ca9c970a56a87a9eb462923" +
        "d9f38f3408437b2d4af7633fa21effb716f9082511ac574f95907ba11" +
        "b1acd6ede702ae9610167c4f881426bc1501684a2b0b7fa54ee86dec4d" +
        "b05cc4009558b6d57e42db5718866cced99b89873c8c63");

    private static readonly byte[] SubstituteTable8V13_00 = Convert.FromHexString(
        "0a6c6996cadc5a08b38339a0f9adf4560e6e4c85649982d4885c8736239a" +
        "112db8c4341866136f59e07422faa665e2d7954e94b0779e1aeee705a" +
        "2c830900d9bd219c93a471512a9291f53acaf4352aef54dbfbee34a06" +
        "d5d0a378a7d61c7a6b81d8dee568fb267ebcbae8cce4727f2cfcf0" +
        "ec28716048ef3e038f1ef16a8df2461b9c86f7b476628a10fd6d0b" +
        "3f9f2f555fc3c6921627d344840fe1808cb7738945db332550ea0414" +
        "c50c32415e79a41d3d5b4037c1cffe2b54eb9d4991f307173cda57" +
        "8bcd61f6ce702eff2193972a7d67abb57c5d0042a5d92051eddd0209" +
        "c2d1f8bdbbe93524985838aab9a8b27501cbc063df3b8ec731b1a1" +
        "b6e67b4b4f");

    private enum TransformVersion
    {
        V12_10,
        V12_11,
        V13_00
    }

    private readonly record struct TransformConstants64(uint Addend1, uint Addend2, int Rotate1, int Rotate2);

    private readonly record struct TransformConstants32(uint Addend1, uint Addend2, int Rotate1, int Rotate2);

    private readonly record struct TransformConstants8(byte Addend1, byte Addend2, int Rotate1, int Rotate2);
}
