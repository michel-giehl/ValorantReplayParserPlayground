using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportClassNetCache("BaseReplayController_C_ClassNetCache", minimalParseMode: ParseMode.Normal)]
public class BaseReplayControllerCache
{
    [NetFieldExportRPC("ClientReplayReceiveInputEventProcessingCapture",
        "/Game/Characters/_Core/BaseReplayController.BaseReplayController_C:ClientReplayReceiveInputEventProcessingCapture",
        isFunction: true)]
    public ClientReplayReceiveInputEventProcessingCapture? ClientReplayReceiveInputEventProcessingCapture { get; set; }

    [NetFieldExportRPC("ReplaysClientReceiveRemoteCharacterUpdatesSingleArrayNoAutonomous",
        "/Script/ShooterGame.ReplayPlayerController:ReplaysClientReceiveRemoteCharacterUpdatesSingleArrayNoAutonomous",
        isFunction: true)]
    public ReplaysClientReceiveRemoteCharacterUpdatesSingleArrayNoAutonomous? ReplaysClientReceiveRemoteCharacterUpdatesSingleArrayNoAutonomous { get; set; }

    [NetFieldExportRPC("ClientGamePhaseBegin",
        "/Script/ShooterGame.AresPlayerController:ClientGamePhaseBegin",
        isFunction: true)]
    public object? ClientGamePhaseBegin { get; set; }

    [NetFieldExportRPC("ClientGamePhaseEnded",
        "/Script/ShooterGame.AresPlayerController:ClientGamePhaseEnded",
        isFunction: true)]
    public ClientGamePhaseEnded? ClientGamePhaseEnded { get; set; }

    [NetFieldExportRPC("ClientOnWinningTeam",
        "/Script/ShooterGame.AresPlayerController:ClientOnWinningTeam",
        isFunction: true)]
    public object? ClientOnWinningTeam { get; set; }

    [NetFieldExportRPC("ClientFlushLevelStreaming",
        "/Script/Engine.PlayerController:ClientFlushLevelStreaming",
        isFunction: true)]
    public object? ClientFlushLevelStreaming { get; set; }

    [NetFieldExportRPC("ClientUpdateMultipleLevelsStreamingStatus",
        "/Script/Engine.PlayerController:ClientUpdateMultipleLevelsStreamingStatus",
        isFunction: true)]
    public object? ClientUpdateMultipleLevelsStreamingStatus { get; set; }
}
