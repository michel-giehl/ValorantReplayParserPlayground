using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Game/Characters/Gumshoe/S0/Ability_E/Ability_Gumshoe_E_TripWire.Ability_Gumshoe_E_TripWire_C", minimalParseMode: ParseMode.Normal)]
public class Ability_Gumshoe_E_TripWire : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public uint? RemoteRole { get; set; }

    [NetFieldExport("AttachParent", RepLayoutCmdType.PropertyObject)]
    public uint? AttachParent { get; set; }

    [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100)]
    public FVector? RelativeScale3D { get; set; }

    [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyObject)]
    public uint? AttachComponent { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)]
    public uint? Owner { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public uint? Role { get; set; }

    [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
    public uint? Instigator { get; set; }

    [NetFieldExport("CosmeticRandomSeed", RepLayoutCmdType.PropertyInt)]
    public int? CosmeticRandomSeed { get; set; }

    [NetFieldExport("CreatedByCharacter", RepLayoutCmdType.PropertyObject)]
    public uint? CreatedByCharacter { get; set; }
}
