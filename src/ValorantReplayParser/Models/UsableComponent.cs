using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.UsableComponent", minimalParseMode: ParseMode.Normal)]
public class UsableComponent : INetFieldExportGroup
{
    [NetFieldExport("bIsActive", RepLayoutCmdType.PropertyBool)]
    public bool? bIsActive { get; set; }
}
