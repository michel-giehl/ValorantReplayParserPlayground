using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.OwnerExclusivePlayerInfo", minimalParseMode: ParseMode.Normal)]
public class OwnerExclusivePlayerInfo : FObfuscatedPlayerInformation
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public uint? RemoteRole { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.Ignore)]
    public uint? Owner { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public uint? Role { get; set; }

    [NetFieldExport("AresController", RepLayoutCmdType.PropertyObject)]
    public uint? AresController { get; set; }

    [NetFieldExport("NumDeathStreak", RepLayoutCmdType.PropertyInt)]
    public int? NumDeathStreak { get; set; }

    [NetFieldExport("StartOfRoundMoneyCache", RepLayoutCmdType.PropertyInt)]
    public int? StartOfRoundMoneyCache { get; set; }

    [NetFieldExport("StartOfRoundLoadoutValueCache", RepLayoutCmdType.PropertyInt)]
    public int? StartOfRoundLoadoutValueCache { get; set; }

    [NetFieldExport("TrackedRewards", RepLayoutCmdType.DynamicArray)]
    public FAresTrackedReward[]? TrackedRewards { get; set; }

    [NetFieldExport("EndOfRoundBeforeRewardsMoney", RepLayoutCmdType.PropertyInt)]
    public int? EndOfRoundBeforeRewardsMoney { get; set; }

    [NetFieldExport("bLoadoutFinalized", RepLayoutCmdType.PropertyBool)]
    public bool? bLoadoutFinalized { get; set; }

    [NetFieldExport("bCanProgressAchievements", RepLayoutCmdType.PropertyBool)]
    public bool? bCanProgressAchievements { get; set; }

    [NetFieldExport("CombatReportComponent", RepLayoutCmdType.PropertyObject)]
    public uint? CombatReportComponent { get; set; }

    [NetFieldExport("KillStreakComponent", RepLayoutCmdType.PropertyObject)]
    public uint? KillStreakComponent { get; set; }

    [NetFieldExport("PersonalizationComponent", RepLayoutCmdType.PropertyObject)]
    public uint? PersonalizationComponent { get; set; }

    [NetFieldExport("SprayLoadoutComponent", RepLayoutCmdType.PropertyObject)]
    public uint? SprayLoadoutComponent { get; set; }

    [NetFieldExport("TotemLoadoutComponent", RepLayoutCmdType.PropertyObject)]
    public uint? TotemLoadoutComponent { get; set; }

    [NetFieldExport("PlayerPurchaseablesComponent", RepLayoutCmdType.PropertyObject)]
    public uint? PlayerPurchaseablesComponent { get; set; }

    [NetFieldExport("ExtendedCombatReportComponent", RepLayoutCmdType.PropertyObject)]
    public uint? ExtendedCombatReportComponent { get; set; }

    [NetFieldExport("AllPlayersObfuscatedPlayerInformation", RepLayoutCmdType.DynamicArray)]
    public FObfuscatedPlayerInformation[]? AllPlayersObfuscatedPlayerInformation { get; set; }
}
