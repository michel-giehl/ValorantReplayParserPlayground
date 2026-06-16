using System.Collections.Generic;
using System.IO;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

public class ComponentDataStream : IProperty
{
    private const byte MovementMagic = 0x52;
    private const double FixedVectorScale = 1.0 / 65536.0;
    private const double OptionalByteScale = 1.0;
    private const double AngleScale = 360.0 / 65536.0;
    private const int MaxMovementPaddingBits = 31;

    public bool HasMovementSection { get; private set; }
    public bool HasValidMovementMagic { get; private set; }
    public ushort MovementBitCount { get; private set; }
    public int TrailingComponentBitCount { get; private set; }
    public string? MovementParseError { get; private set; }
    public List<MovementMove> Moves { get; } = [];

    public void Serialize(NetBitReader reader)
    {
        if (TryReadPayloadBytes(reader, out var payloadBytes))
        {
            var payloadReader = new NetBitReader(payloadBytes, payloadBytes.Length * 8)
            {
                EngineNetworkVersion = reader.EngineNetworkVersion,
                NetworkVersion = reader.NetworkVersion,
                NetworkReplayVersion = reader.NetworkReplayVersion,
                ReplayHeaderFlags = reader.ReplayHeaderFlags,
                ReplayVersion = reader.ReplayVersion,
            };

            ParseComponentPayload(payloadReader);
            return;
        }

        ParseComponentPayload(reader);
    }

    private void ParseComponentPayload(NetBitReader reader)
    {
        reader.Mark();

        if (!reader.CanRead(16))
        {
            return;
        }

        var movementBitCount = reader.ReadUInt16();
        if (movementBitCount == 0)
        {
            HasMovementSection = true;
            MovementBitCount = (ushort)Math.Min(reader.GetBitsLeft(), ushort.MaxValue);
            ParseMovementSection(reader);
            return;
        }

        if (movementBitCount > reader.GetBitsLeft())
        {
            reader.Pop();
            HasMovementSection = true;
            MovementBitCount = (ushort)Math.Min(reader.GetBitsLeft(), ushort.MaxValue);
            ParseMovementSection(reader);
            return;
        }

        HasMovementSection = true;
        MovementBitCount = movementBitCount;

        reader.SetTempEnd(movementBitCount, FBitArchiveEndIndex.FIELD_HEADER_PAYLOAD);
        try
        {
            ParseMovementSection(reader);
        }
        finally
        {
            reader.RestoreTempEnd(FBitArchiveEndIndex.FIELD_HEADER_PAYLOAD);
        }

        if (reader.GetBitsLeft() > 0)
        {
            TrailingComponentBitCount = reader.GetBitsLeft();
            reader.Seek(TrailingComponentBitCount, SeekOrigin.Current);
        }
    }

    private static bool TryReadPayloadBytes(NetBitReader reader, out byte[] payloadBytes)
    {
        payloadBytes = [];
        reader.Mark();

        if (!TryReadUInt16(reader, out var byteCount) || byteCount == 0 || !reader.CanRead(byteCount * 8))
        {
            reader.Pop();
            return false;
        }

        payloadBytes = reader.ReadBytes(byteCount).ToArray();
        return !reader.IsError;
    }

    private void ParseMovementSection(NetBitReader reader)
    {
        if (!TryReadByte(reader, out var magic))
        {
            MovementParseError = "Missing movement magic";
            return;
        }

        HasValidMovementMagic = magic == MovementMagic;
        if (!HasValidMovementMagic)
        {
            MovementParseError = $"Invalid movement magic 0x{magic:X2}";
            return;
        }

        var expectedMarker = 1;
        if (!TryReadBits(reader, 3, out var marker))
        {
            MovementParseError = "Missing first movement marker";
            return;
        }

        while (marker != 0 && !reader.IsError)
        {
            if (marker != expectedMarker)
            {
                MovementParseError = $"Movement marker mismatch: expected {expectedMarker}, got {marker}";
                return;
            }

            if (!TryReadMove(reader, marker, out var move, out var error))
            {
                MovementParseError = error ?? "Invalid movement record";
                return;
            }

            Moves.Add(move);

            if (reader.GetBitsLeft() <= MaxMovementPaddingBits)
            {
                return;
            }

            expectedMarker = NextMarker(expectedMarker);
            if (!TryReadBits(reader, 3, out marker))
            {
                MovementParseError = "Missing next movement marker";
                return;
            }
        }
    }

    private static bool TryReadMove(NetBitReader reader, int marker, out MovementMove move, out string? error)
    {
        move = new MovementMove();
        error = null;

        if (!TryReadBit(reader, out var moveType) ||
            !TryReadByte(reader, out var rotationYawMultiplier) ||
            !TryReadByte(reader, out var movementState) ||
            !TryReadByte(reader, out var unusedByte))
        {
            error = "Missing movement record header";
            return false;
        }

        move.Marker = marker;
        move.MoveType = moveType ? (byte)1 : (byte)0;
        move.RotationYawMultiplier = unchecked((sbyte)rotationYawMultiplier);
        move.ModeFlags = movementState;
        move.MovementState = movementState;
        move.UnusedByte = unusedByte;

        if (!TryReadFixedVector(reader, out var rotationInput) ||
            !TryReadVLQ(reader, out var timestamp) ||
            !TryReadQuantizedVector(reader, 100, out var position))
        {
            error = "Missing movement common vector/timestamp fields";
            return false;
        }

        move.RotationInput = rotationInput;
        move.Timestamp = timestamp;
        move.Position = position;

        if (!TryReadBit(reader, out var hasOptionalByte))
        {
            error = "Missing optional movement value flag";
            return false;
        }

        move.HasOptionalMovementValue = hasOptionalByte;
        if (hasOptionalByte)
        {
            if (!TryReadByte(reader, out var optionalByte))
            {
                error = "Missing optional movement value";
                return false;
            }

            move.OptionalMovementRawByte = optionalByte;
            move.OptionalMovementValue = optionalByte * OptionalByteScale;
        }

        if (!TryReadBit(reader, out var flag48))
        {
            error = "Missing movement flag/angle fields";
            return false;
        }

        if (!TryReadUInt32(reader, out var packedAngles))
        {
            error = "Missing movement flag/angle fields";
            return false;
        }

        var pitch = (ushort)(packedAngles & 0xFFFF);
        var yaw = (ushort)(packedAngles >> 16);
        move.Flag48 = flag48;
        move.PackedAngles = packedAngles;
        move.RawYaw = yaw;
        move.RawPitch = pitch;
        move.Yaw = yaw * AngleScale;
        move.Pitch = pitch * AngleScale;

        if (moveType)
        {
            if (!TryReadBit(reader, out var variant1Flag) ||
                !TryReadQuantizedVector(reader, 10, out var variant1Vector))
            {
                error = "Missing variant-1 movement fields";
                return false;
            }

            move.Variant1Flag = variant1Flag;
            move.Variant1Vector = variant1Vector;
            move.Velocity = variant1Vector;
        }
        else if (!TryReadVariant0Extra(reader, move, out error))
        {
            return false;
        }

        if (!TryReadBit(reader, out var errorSentinel))
        {
            error = "Missing movement error sentinel";
            return false;
        }

        move.ErrorSentinel = errorSentinel;
        if (errorSentinel)
        {
            error = "Movement error sentinel was set";
        }

        return !errorSentinel;
    }

    private static bool TryReadVariant0Extra(NetBitReader reader, MovementMove move, out string? error)
    {
        error = null;
        if (!TryReadBit(reader, out var hasExternalCharacterRef))
        {
            error = "Missing variant-0 external reference flag";
            return false;
        }

        move.Variant0HasExternalCharacterRef = hasExternalCharacterRef;
        if (hasExternalCharacterRef)
        {
            error = "Variant-0 external character reference is not decoded yet";
            return false;
        }

        if (!TryReadUInt32(reader, out var packedAngles))
        {
            error = "Missing variant-0 packed angle dword";
            return false;
        }

        move.Variant0PackedAngles = packedAngles;
        return true;
    }

    private static bool TryReadFixedVector(NetBitReader reader, out FVector vector)
    {
        vector = new FVector(0, 0, 0)
        {
            ScaleFactor = 65536,
            Bits = 16,
        };

        if (!TryReadSerializedInt(reader, 0x10000, out var x) ||
            !TryReadSerializedInt(reader, 0x10000, out var y) ||
            !TryReadSerializedInt(reader, 0x10000, out var z))
        {
            return false;
        }

        vector.X = ((int)x - 0x8000) * FixedVectorScale;
        vector.Y = ((int)y - 0x8000) * FixedVectorScale;
        vector.Z = ((int)z - 0x8000) * FixedVectorScale;
        return true;
    }

    private static bool TryReadQuantizedVector(NetBitReader reader, int scaleFactor, out FVector vector)
    {
        vector = new FVector(0, 0, 0)
        {
            ScaleFactor = scaleFactor,
        };

        if (!TryReadSerializedInt(reader, 1 << 7, out var componentBitCountAndExtraInfo))
        {
            return false;
        }

        var componentBits = (int)(componentBitCountAndExtraInfo & 63U);
        var extraInfo = componentBitCountAndExtraInfo >> 6;
        vector.Bits = componentBits;

        if (componentBits > 0)
        {
            if (!TryReadSignedQuantizedComponent(reader, componentBits, out var x) ||
                !TryReadSignedQuantizedComponent(reader, componentBits, out var y) ||
                !TryReadSignedQuantizedComponent(reader, componentBits, out var z))
            {
                return false;
            }

            vector.X = extraInfo > 0 ? x / (double)scaleFactor : x;
            vector.Y = extraInfo > 0 ? y / (double)scaleFactor : y;
            vector.Z = extraInfo > 0 ? z / (double)scaleFactor : z;
            return true;
        }

        if (extraInfo == 0)
        {
            if (!reader.CanRead(96))
            {
                return false;
            }

            vector.X = reader.ReadSingle();
            vector.Y = reader.ReadSingle();
            vector.Z = reader.ReadSingle();
            vector.Bits = 32;
            return !reader.IsError;
        }

        if (!reader.CanRead(192))
        {
            return false;
        }

        vector.X = reader.ReadDouble();
        vector.Y = reader.ReadDouble();
        vector.Z = reader.ReadDouble();
        vector.Bits = 64;
        return !reader.IsError;
    }

    private static bool TryReadSignedQuantizedComponent(NetBitReader reader, int componentBits, out long value)
    {
        value = 0;
        if (componentBits <= 0 || componentBits > 62 || !reader.CanRead(componentBits))
        {
            return false;
        }

        var raw = reader.ReadBitsToLong(componentBits);
        var signBit = 1UL << (componentBits - 1);
        value = (long)(raw ^ signBit) - (long)signBit;
        return !reader.IsError;
    }

    private static bool TryReadVLQ(NetBitReader reader, out uint value)
    {
        value = 0;
        var shift = 0;

        while (true)
        {
            if (!TryReadByte(reader, out var b))
            {
                return false;
            }

            value |= (uint)(((b >> 1) & 0x7F) << shift);
            if ((b & 1) == 0)
            {
                return true;
            }

            shift += 7;
            if (shift >= 32)
            {
                return false;
            }
        }
    }

    private static bool TryReadSerializedInt(NetBitReader reader, int maxValue, out uint value)
    {
        value = 0;
        for (uint mask = 1; value + mask < maxValue; mask <<= 1)
        {
            if (!TryReadBit(reader, out var bit))
            {
                return false;
            }

            if (bit)
            {
                value |= mask;
            }
        }

        return true;
    }

    private static bool TryReadBit(NetBitReader reader, out bool value)
    {
        value = false;
        if (!reader.CanRead(1))
        {
            return false;
        }

        value = reader.ReadBit();
        return !reader.IsError;
    }

    private static bool TryReadBits(NetBitReader reader, int bitCount, out int value)
    {
        value = 0;
        if (!reader.CanRead(bitCount))
        {
            return false;
        }

        value = reader.ReadBitsToInt(bitCount);
        return !reader.IsError;
    }

    private static bool TryReadByte(NetBitReader reader, out byte value)
    {
        value = 0;
        if (!reader.CanRead(8))
        {
            return false;
        }

        value = reader.ReadByte();
        return !reader.IsError;
    }

    private static bool TryReadUInt16(NetBitReader reader, out ushort value)
    {
        value = 0;
        if (!reader.CanRead(16))
        {
            return false;
        }

        value = reader.ReadUInt16();
        return !reader.IsError;
    }

    private static bool TryReadUInt32(NetBitReader reader, out uint value)
    {
        value = 0;
        if (!reader.CanRead(32))
        {
            return false;
        }

        value = reader.ReadUInt32();
        return !reader.IsError;
    }

    private static int NextMarker(int marker)
    {
        var next = (marker + 1) & 7;
        return next < 2 ? 1 : next;
    }
}

public class MovementMove
{
    public int Marker { get; set; }
    public byte MoveType { get; set; }
    public FVector? Position { get; set; }
    public FVector? Velocity { get; set; }
    public FVector? RotationInput { get; set; }
    public FVector? Variant1Vector { get; set; }
    public uint Timestamp { get; set; }
    public byte ModeFlags { get; set; }
    public byte MovementState { get; set; }
    public sbyte RotationYawMultiplier { get; set; }
    public byte UnusedByte { get; set; }
    public bool HasOptionalMovementValue { get; set; }
    public byte? OptionalMovementRawByte { get; set; }
    public double? OptionalMovementValue { get; set; }
    public bool Flag48 { get; set; }
    public uint PackedAngles { get; set; }
    public ushort RawYaw { get; set; }
    public ushort RawPitch { get; set; }
    public double Yaw { get; set; }
    public double Pitch { get; set; }
    public bool? Variant0HasExternalCharacterRef { get; set; }
    public uint? Variant0PackedAngles { get; set; }
    public bool? Variant1Flag { get; set; }
    public bool ErrorSentinel { get; set; }
}
