using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.AresAbilitySystemComponent", minimalParseMode: ParseMode.Normal)]
public class AresAbilitySystemComponent : INetFieldExportGroup
{
    [NetFieldExport("OwnerActor", RepLayoutCmdType.PropertyObject)]
    public uint? OwnerActor { get; set; }

    [NetFieldExport("AvatarActor", RepLayoutCmdType.PropertyObject)]
    public uint? AvatarActor { get; set; }

    [NetFieldExport("Def", RepLayoutCmdType.Ignore)]
    public uint? Def { get; set; }

    [NetFieldExport("ModifiedAttributes", RepLayoutCmdType.Ignore)]
    public uint? ModifiedAttributes { get; set; }

    [NetFieldExport("Duration", RepLayoutCmdType.PropertyFloat)]
    public float? Duration { get; set; }

    [NetFieldExport("Period", RepLayoutCmdType.PropertyFloat)]
    public float? Period { get; set; }

    [NetFieldExport("ChanceToApplyToTarget", RepLayoutCmdType.PropertyFloat)]
    public float? ChanceToApplyToTarget { get; set; }

    [NetFieldExport("DynamicGrantedTags", RepLayoutCmdType.Ignore)]
    public uint? DynamicGrantedTags { get; set; }

    [NetFieldExport("DynamicAssetTags", RepLayoutCmdType.Ignore)]
    public uint? DynamicAssetTags { get; set; }

    [NetFieldExport("Modifiers", RepLayoutCmdType.Ignore)]
    public uint? Modifiers { get; set; }

    [NetFieldExport("EvaluatedMagnitude", RepLayoutCmdType.Ignore)]
    public uint? EvaluatedMagnitude { get; set; }

    [NetFieldExport("StackCount", RepLayoutCmdType.PropertyInt)]
    public int? StackCount { get; set; }

    [NetFieldExport("GrantedAbilitySpecs", RepLayoutCmdType.Ignore)]
    public uint? GrantedAbilitySpecs { get; set; }

    [NetFieldExport("EffectContext", RepLayoutCmdType.Ignore)]
    public uint? EffectContext { get; set; }

    [NetFieldExport("Level", RepLayoutCmdType.PropertyFloat)]
    public float? Level { get; set; }

    [NetFieldExport("PredictionKey", RepLayoutCmdType.Ignore)]
    public uint? PredictionKey { get; set; }

    [NetFieldExport("GrantedAbilityHandles", RepLayoutCmdType.Ignore)]
    public uint? GrantedAbilityHandles { get; set; }

    [NetFieldExport("StartServerWorldTime", RepLayoutCmdType.PropertyFloat)]
    public float? StartServerWorldTime { get; set; }

    [NetFieldExport("SpawnedAttributes", RepLayoutCmdType.Ignore)]
    public uint? SpawnedAttributes { get; set; }

    [NetFieldExport("CachedAttributeSet", RepLayoutCmdType.PropertyObject)]
    public uint? CachedAttributeSet { get; set; }
}
