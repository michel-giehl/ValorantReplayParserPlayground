// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportClassNetCache("BasePistol_C_ClassNetCache", minimalParseMode: ParseMode.Normal)]
public class BasePistol_C_ClassNetCache
{
    [NetFieldExportRPC("MulticastPlayContinuousEffectFromClient",
        "/Script/ShooterGame.AresEquippable:MulticastPlayContinuousEffectFromClient",
        isFunction: true)]
    public MulticastPlayContinuousEffectFromClient? MulticastPlayContinuousEffectFromClient { get; set; }
}

[NetFieldExportGroup("/Script/ShooterGame.AresEquippable:MulticastPlayContinuousEffectFromClient", minimalParseMode: ParseMode.Normal)]
public class MulticastPlayContinuousEffectFromClient : INetFieldExportGroup
{
    [NetFieldExport("EffectManagerComponent", RepLayoutCmdType.Ignore)]
    public object? EffectManagerComponent { get; set; }

    [NetFieldExport("EffectContainer", RepLayoutCmdType.Ignore)]
    public object? EffectContainer { get; set; }

    [NetFieldExport("WaitOnReplicationActor", RepLayoutCmdType.Ignore)]
    public object? WaitOnReplicationActor { get; set; }

    [NetFieldExport("ObjectValues", RepLayoutCmdType.Ignore)]
    public object? ObjectValues { get; set; }

    [NetFieldExport("Name", RepLayoutCmdType.Ignore)]
    public object? Name { get; set; }

    [NetFieldExport("Object", RepLayoutCmdType.Ignore)]
    public object? Object { get; set; }

    [NetFieldExport("Translation", RepLayoutCmdType.PropertyQuat)]
    public FQuat? Rotation { get; set; }

    [NetFieldExport("Translation", RepLayoutCmdType.PropertyVector)]
    public FVector? Translation { get; set; }

    [NetFieldExport("Scale3D", RepLayoutCmdType.PropertyVector)]
    public FVector? Scale3D { get; set; }

    [NetFieldExport("EffectID", RepLayoutCmdType.PropertyUInt64)]
    public uint? EffectID { get; set; }

    [NetFieldExport("SourceID", RepLayoutCmdType.PropertyString)]
    public string? SourceID { get; set; }

    [NetFieldExport("bLocalEffect", RepLayoutCmdType.PropertyBool)]
    public bool? bLocalEffect { get; set; }

    [NetFieldExport("bLocalEffect", RepLayoutCmdType.PropertyBool)]
    public bool? bTransient { get; set; }

    [NetFieldExport("StartMovementTime", RepLayoutCmdType.Ignore)]
    public object? ClientControllerThatTriggered { get; set; }

    [NetFieldExport("StartMovementTime", RepLayoutCmdType.PropertyFloat)]
    public float? StartMovementTime { get; set; }

    [NetFieldExport("AllianceFilter", RepLayoutCmdType.Enum)]
    public EAresAlliance? AllianceFilter { get; set; }
}

public enum EAresAlliance
{
    Alliance_Ally = 0,
    Alliance_Enemy = 1,
    Alliance_Neutral = 2,
    Alliance_Any = 3,
    Alliance_Count = 4,
    Alliance_MAX = 5
}
