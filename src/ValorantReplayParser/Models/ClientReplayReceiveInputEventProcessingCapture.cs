// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.ReplayPlayerController:ClientReplayReceiveInputEventProcessingCapture", minimalParseMode: ParseMode.Normal)]
public class ClientReplayReceiveInputEventProcessingCapture : INetFieldExportGroup
{
    [NetFieldExport("PlayerID", RepLayoutCmdType.PropertyInt)]
    public int? PlayerID { get; set; }
    [NetFieldExport("InputEventData", RepLayoutCmdType.DynamicArray)]
    public char[]? InputEventData { get; set; }
}
