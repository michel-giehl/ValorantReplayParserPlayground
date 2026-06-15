using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Unreal.Encryption;
using BinaryReader = Unreal.Core.BinaryReader;
using EventInfo = Unreal.Core.Models.EventInfo;

namespace ValorantReplayParser;

public class ValorantReplayReader(ILogger? logger = null, ParseMode parseMode = ParseMode.Minimal)
    : ReplayReader<ValorantReplay>(logger ?? NullLogger.Instance, parseMode)
{
    public ValorantReplay ReadReplay(string fileName)
    {
        using var stream = File.OpenRead(fileName);
        return ReadReplay(stream);
    }

    public ValorantReplay ReadReplay(Stream stream)
    {
        using var archive = new BinaryReader(stream);
        return ReadReplay(archive);
    }

    public override void ReadEvent(FArchive archive)
    {
        var info = new EventInfo { Id = archive.ReadFString(), Group = archive.ReadFString(), Metadata = archive.ReadFString(), StartTime = archive.ReadUInt32(), EndTime = archive.ReadUInt32(), SizeInBytes = archive.ReadInt32() };
        logger?.LogInformation("Event: t={T}ms group={G}", info.StartTime, info.Group);
    }


    public override bool ReceivedReplicatorBunch(DataBunch bunch, FBitArchive archive, uint? repObject, bool bHasRepLayout)
    {
        var payloadBits = archive.GetBitsLeft();
        if (payloadBits <= 0)
        {
            return base.ReceivedReplicatorBunch(bunch, archive, repObject, bHasRepLayout);
        }

        var rawPayload = CopyPayload(archive, payloadBits);
        var seed = (uint)payloadBits;
        var actorGuid = Channels[bunch.ChIndex]?.ActorId;
        if (actorGuid is not null)
        {
            seed ^= actorGuid.Value;
        }

        var transformedPayload = ValorantSeededPayloadTransform.Apply(rawPayload, payloadBits, seed, "release-12.11");
        var transformedReader = CreateReader(transformedPayload, payloadBits, archive);
        var transformedBunch = new DataBunch(bunch)
        {
            Archive = transformedReader,
        };

        // transformedReader.ReadBit();

        return base.ReceivedReplicatorBunch(transformedBunch, transformedReader, repObject, bHasRepLayout);
    }

    protected override void OnExportRead(uint channelIndex, INetFieldExportGroup? exportGroup)
    {

        if (exportGroup is null)
        {
            // Console.WriteLine($"Chindex={channelIndex}\tGroup=null");
            return;
        }
        var type = exportGroup.GetType();

        var props = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetIndexParameters().Length == 0)
            .Select(p =>
            {
                object? value;
                try { value = p.GetValue(exportGroup); }
                catch { value = "<error reading property>"; }

                return new { p.Name, Value = value };
            })
            .Where(x => x.Value is not null);

        var json = JsonSerializer.Serialize(props, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        Console.WriteLine($"Chindex={channelIndex}\tType={type.Name}\tFields={json}");
    }

    protected override void OnExternalDataRead(uint channelIndex, IExternalData? exportGroup)
    {
    }

    protected override void OnNetDeltaRead(uint channelIndex, NetDeltaUpdate update)
    {
    }

    private static NetBitReader CreateReader(byte[] payload, int payloadBits, FBitArchive archive) => new(payload, payloadBits)
    {
        EngineNetworkVersion = archive.EngineNetworkVersion,
        NetworkVersion = archive.NetworkVersion,
        ReplayHeaderFlags = archive.ReplayHeaderFlags,
        ReplayVersion = archive.ReplayVersion,
        NetworkReplayVersion = archive.NetworkReplayVersion
    };

    private static byte[] CopyPayload(FBitArchive archive, int payloadBits)
    {
        var start = archive.Position;
        var payload = archive.ReadBits(payloadBits).ToArray();
        archive.Seek(start, SeekOrigin.Begin);
        return payload;
    }

    protected override FArchive Decompress(FArchive archive)
    {
        if (!Replay.Info.IsCompressed) return archive;
        var ds = archive.ReadInt32(); var cs = archive.ReadInt32();
        var output = Oodle.DecompressReplayData(archive.ReadBytes(cs), ds);
        return new BinaryReader(new MemoryStream(output.ToArray())) { EngineNetworkVersion = Replay.Header.EngineNetworkVersion, NetworkVersion = Replay.Header.NetworkVersion, ReplayHeaderFlags = Replay.Header.Flags, ReplayVersion = Replay.Info.FileVersion };
    }

}
