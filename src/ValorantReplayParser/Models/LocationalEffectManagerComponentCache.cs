using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportClassNetCache("LocationalEffectManagerComponent_ClassNetCache", minimalParseMode: ParseMode.Normal)]
public class LocationalEffectManagerComponentCache
{
    [NetFieldExportRPC("ClientCleanUpLocationalEffects",
        "/Script/ShooterGame.LocationalEffectManagerComponent:ClientCleanUpLocationalEffects",
        isFunction: true)]
    public ClientCleanUpLocationalEffects? ClientCleanUpLocationalEffects { get; set; }

    [NetFieldExportRPC("ClientPlayOneShotEffectAtLocation",
        "/Script/ShooterGame.LocationalEffectManagerComponent:ClientPlayOneShotEffectAtLocation",
        isFunction: true)]
    public ClientPlayOneShotEffectAtLocation? ClientPlayOneShotEffectAtLocation { get; set; }
}

[NetFieldExportGroup("/Script/ShooterGame.LocationalEffectManagerComponent:ClientCleanUpLocationalEffects", minimalParseMode: ParseMode.Normal)]
public class ClientCleanUpLocationalEffects : INetFieldExportGroup
{
}

[NetFieldExportGroup("/Script/ShooterGame.LocationalEffectManagerComponent:ClientPlayOneShotEffectAtLocation", minimalParseMode: ParseMode.Normal)]
public class ClientPlayOneShotEffectAtLocation : INetFieldExportGroup
{
}
