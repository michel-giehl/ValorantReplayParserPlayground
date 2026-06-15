using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.RemoteCharacterUpdate", minimalParseMode: ParseMode.Normal)]
public class RemoteCharacterUpdate : INetFieldExportGroup
{
    [NetFieldExport("ShooterCharacterNetGuidValue", RepLayoutCmdType.PropertyUInt32)]
    public uint? ShooterCharacterNetGuidValue { get; set; }

    [NetFieldExport("ComponentDataStream", RepLayoutCmdType.Property)]
    public ComponentDataStream? ComponentDataStream { get; set; }
}
