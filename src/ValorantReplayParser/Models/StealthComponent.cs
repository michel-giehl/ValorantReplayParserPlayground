using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.StealthComponent", minimalParseMode: ParseMode.Normal)]
public class StealthComponent : INetFieldExportGroup
{
    [NetFieldExport("bReplicates", RepLayoutCmdType.PropertyBool)]
    public bool? bReplicates { get; set; }

    [NetFieldExport("bStealthIsActive", RepLayoutCmdType.PropertyBool)]
    public bool? bStealthIsActive { get; set; }

    [NetFieldExport("SubscribedToComponent", RepLayoutCmdType.PropertyObject)]
    public uint? SubscribedToComponent { get; set; }
}
