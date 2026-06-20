using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.AbilityTrackingDelegateComponent", minimalParseMode: ParseMode.Normal)]
public class AbilityTrackingDelegateComponent : INetFieldExportGroup
{
    [NetFieldExport("AbilityTrackingComponent", RepLayoutCmdType.PropertyObject)]
    public uint? AbilityTrackingComponent { get; set; }
}
