using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.ReplayPlayerController:ReplaysClientReceiveRemoteCharacterUpdatesSingleArrayNoAutonomous", minimalParseMode: ParseMode.Normal)]
public class ReplaysClientReceiveRemoteCharacterUpdatesSingleArrayNoAutonomous : RemoteCharacterUpdate
{
    [NetFieldExport("RemoteCharacterUpdates", RepLayoutCmdType.DynamicArray)]
    public RemoteCharacterUpdate[]? RemoteCharacterUpdates { get; set; }
}
