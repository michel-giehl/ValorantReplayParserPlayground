// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportClassNetCache("ReplayEffectComponent_ClassNetCache", minimalParseMode: ParseMode.Normal)]
public class ReplayEffectComponent_ClassNetCache
{
    [NetFieldExportRPC("ReplayPlayContinuousEffectAtLocation", "/Script/ShooterGame.ReplayEffectComponent", true)]
    public ReplayPlayContinuousEffectAtLocation? ReplayPlayContinuousEffectAtLocation { get; set; }
}

[NetFieldExportGroup("/Script/ShooterGame.ReplayEffectComponent", ParseMode.Normal)]
public class ReplayPlayContinuousEffectAtLocation : INetFieldExportGroup
{
}
