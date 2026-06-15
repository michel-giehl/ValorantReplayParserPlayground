// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;


[NetFieldExportGroup("/Script/ShooterGame.AresPlayerController:ClientGamePhaseEnded", minimalParseMode: ParseMode.Normal)]
public class ClientGamePhaseEnded : INetFieldExportGroup
{
    [NetFieldExport("OldPhase", RepLayoutCmdType.Enum)]
    public EAresGamePhase OldPhase { get; set; }
}
