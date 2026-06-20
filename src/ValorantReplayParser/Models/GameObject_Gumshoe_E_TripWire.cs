using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Game/Characters/Gumshoe/S0/Ability_E/GameObject_Gumshoe_E_TripWire.GameObject_Gumshoe_E_TripWire_C", minimalParseMode: ParseMode.Normal)]
public class GameObject_Gumshoe_E_TripWire : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public uint? RemoteRole { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)]
    public uint? Owner { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public uint? Role { get; set; }

    [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
    public uint? Instigator { get; set; }

    [NetFieldExport("Deployed", RepLayoutCmdType.PropertyBool)]
    public bool? Deployed { get; set; }
}
