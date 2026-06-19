using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportSubGroup("/Script/ShooterGame.OwnerExclusivePlayerInfo", minimalParseMode: ParseMode.Normal)]
public class FAresTrackedReward
{
    [NetFieldExport("Rewards", RepLayoutCmdType.Ignore)]
    public object? Rewards { get; set; }

    [NetFieldExport("RewardName", RepLayoutCmdType.PropertyName)]
    public string? RewardName { get; set; }

    [NetFieldExport("LocalizedRewardName", RepLayoutCmdType.Property)]
    public FText? LocalizedRewardName { get; set; }

    [NetFieldExport("InstancesOfReward", RepLayoutCmdType.PropertyInt)]
    public int? InstancesOfReward { get; set; }

    [NetFieldExport("RewardGrantStrategy", RepLayoutCmdType.Enum)]
    public EAresRewardGrantStrategy RewardGrantStrategy { get; set; }

    [NetFieldExport("Source", RepLayoutCmdType.Enum)]
    public ERewardSource Source { get; set; }
}
