using System.Text.RegularExpressions;

namespace ValorantReplayParser;

public static class ValorantSeededPayloadTransform
{
    private const ulong Multiplier = 0x2545f4914f6cdd1dUL;
    private const uint SeedAddendV12_10 = 0x12fd0ee5u;
    private const uint SeedAddendV12_11 = 0x409d36a3u;
    private const uint InitAOffsetV12_10 = 0x1bu;
    private const uint InitAOffsetV12_11 = 0x23u;
    private const byte TailXorV12_10 = 0xe5;
    private const byte TailXorV12_11 = 0xa3;

    public static byte[] Apply(byte[] payload, int bitCount, uint seed, string? branch = null) =>
        ResolveVersion(branch) == TransformVersion.V12_11
            ? ApplyV12_11(payload, bitCount, seed)
            : ApplyV12_10(payload, bitCount, seed);

    private static TransformVersion ResolveVersion(string? branch)
    {
        if (string.IsNullOrEmpty(branch))
            return TransformVersion.V12_10;

        var match = Regex.Match(branch, @"release-(\d+)\.(\d+)", RegexOptions.IgnoreCase);
        if (!match.Success)
            return TransformVersion.V12_10;

        var major = int.Parse(match.Groups[1].Value);
        var minor = int.Parse(match.Groups[2].Value);
        return major == 12 && minor == 11 ? TransformVersion.V12_11 : TransformVersion.V12_10;
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

    private enum TransformVersion
    {
        V12_10,
        V12_11
    }

    private readonly record struct TransformConstants64(uint Addend1, uint Addend2, int Rotate1, int Rotate2);

    private readonly record struct TransformConstants32(uint Addend1, uint Addend2, int Rotate1, int Rotate2);

    private readonly record struct TransformConstants8(byte Addend1, byte Addend2, int Rotate1, int Rotate2);
}
