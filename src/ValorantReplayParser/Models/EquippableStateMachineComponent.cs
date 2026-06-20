using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.EquippableStateMachineComponent", minimalParseMode: ParseMode.Normal)]
public class EquippableStateMachineComponent : INetFieldExportGroup
{
    [NetFieldExport("CurrentState", RepLayoutCmdType.Ignore)]
    public uint? CurrentState { get; set; }

    [NetFieldExport("TransitionContext", RepLayoutCmdType.Ignore)]
    public uint? TransitionContext { get; set; }

    [NetFieldExport("AuthStartWorldTime", RepLayoutCmdType.PropertyFloat)]
    public float? AuthStartWorldTime { get; set; }
}
