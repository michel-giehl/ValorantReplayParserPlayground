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

    private bool HasMovementSection { get; set; }
    private bool HasValidMovementMagic { get; set; }
    private ushort MovementBitCount { get; set; }
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
            reader.Seek(reader.GetBitsLeft(), SeekOrigin.Current);
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
            return;
        }

        HasValidMovementMagic = magic == MovementMagic;
        if (!HasValidMovementMagic)
        {
            return;
        }

        var expectedMarker = 1;
        if (!TryReadBits(reader, 3, out var marker))
        {
            return;
        }

        while (marker != 0 && !reader.IsError)
        {
            if (marker != expectedMarker)
            {
                return;
            }

            if (!TryReadMove(reader, out var move))
            {
                return;
            }

            Moves.Add(move);

            expectedMarker = NextMarker(expectedMarker);
            if (!TryReadBits(reader, 3, out marker))
            {
                return;
            }
        }
    }

    private static bool TryReadMove(NetBitReader reader, out MovementMove move)
    {
        move = new MovementMove();

        if (!TryReadBit(reader, out var moveType) ||
            !TryReadByte(reader, out var rotationYawMultiplier) ||
            !TryReadByte(reader, out var movementState) ||
            !TryReadByte(reader, out _))
        {
            return false;
        }

        move.RotationYawMultiplier = unchecked((sbyte)rotationYawMultiplier);
        move.MovementState = movementState;

        if (!TryReadFixedVector(reader, out var rotationInput) ||
            !TryReadVLQ(reader, out var timestamp) ||
            !TryReadPackedVector(reader, 100, 30, out var position))
        {
            return false;
        }

        move.RotationInput = rotationInput;
        move.Timestamp = timestamp;
        move.Position = position;

        if (!TryReadBit(reader, out var hasOptionalByte))
        {
            return false;
        }

        if (hasOptionalByte)
        {
            if (!TryReadByte(reader, out var optionalByte))
            {
                return false;
            }

            move.OptionalMovementValue = optionalByte * OptionalByteScale;
        }

        if (!TryReadBit(reader, out _) ||
            !TryReadUInt16(reader, out var yaw) ||
            !TryReadUInt16(reader, out var pitch))
        {
            return false;
        }

        move.Yaw = yaw * AngleScale;
        move.Pitch = pitch * AngleScale;

        if (moveType)
        {
            if (!TryReadBit(reader, out _) ||
                !TryReadPackedVector(reader, 10, 24, out var velocity))
            {
                return false;
            }

            move.Velocity = velocity;
        }
        else if (!TrySkipVariant0Extra(reader))
        {
            return false;
        }

        if (!TryReadBit(reader, out var errorSentinel))
        {
            return false;
        }

        return !errorSentinel;
    }

    private static bool TrySkipVariant0Extra(NetBitReader reader)
    {
        if (!TryReadBit(reader, out var hasExternalCharacterRef))
        {
            return false;
        }

        if (hasExternalCharacterRef)
        {
            // The external character reference payload is still unidentified. Stop before
            // corrupting alignment for following moves.
            reader.Seek(reader.GetBitsLeft(), SeekOrigin.Current);
            return true;
        }

        return !reader.CanRead(32) || TryReadUInt32(reader, out _);
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

    private static bool TryReadPackedVector(NetBitReader reader, int scaleFactor, int maxBits, out FVector vector)
    {
        vector = new FVector(0, 0, 0)
        {
            ScaleFactor = scaleFactor,
        };

        if (!TryReadSerializedInt(reader, maxBits, out var bits) || bits > 28)
        {
            return false;
        }

        var componentBits = (int)bits;
        var bias = 1 << (componentBits + 1);
        var max = 1 << (componentBits + 2);

        if (!TryReadSerializedInt(reader, max, out var x) ||
            !TryReadSerializedInt(reader, max, out var y) ||
            !TryReadSerializedInt(reader, max, out var z))
        {
            return false;
        }

        vector.X = ((int)x - bias) / (double)scaleFactor;
        vector.Y = ((int)y - bias) / (double)scaleFactor;
        vector.Z = ((int)z - bias) / (double)scaleFactor;
        vector.Bits = componentBits;
        return true;
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
    public FVector? Position { get; set; }
    public FVector? Velocity { get; set; }
    public FVector? RotationInput { get; set; }
    public uint Timestamp { get; set; }
    public byte MovementState { get; set; }
    public sbyte RotationYawMultiplier { get; set; }
    public double? OptionalMovementValue { get; set; }
    public double Yaw { get; set; }
    public double Pitch { get; set; }
}
