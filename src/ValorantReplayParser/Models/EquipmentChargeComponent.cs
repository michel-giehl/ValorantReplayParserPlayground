using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.EquipmentChargeComponent", minimalParseMode: ParseMode.Normal)]
public class EquipmentChargeComponent : INetFieldExportGroup
{
    [NetFieldExport("AuthResourceAmount", RepLayoutCmdType.PropertyFloat)]
    public float? AuthResourceAmount { get; set; }
}
