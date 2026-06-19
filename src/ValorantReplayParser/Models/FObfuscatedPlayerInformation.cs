using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

public class FObfuscatedPlayerInformation : INetFieldExportGroup
{
    [NetFieldExport("SubjectUniqueId", RepLayoutCmdType.PropertyNetId)]
    public string? SubjectUniqueId { get; set; }

    [NetFieldExport("bIsAfk", RepLayoutCmdType.PropertyBool)]
    public bool? bIsAfk { get; set; }

    [NetFieldExport("ConnectionStatus", RepLayoutCmdType.Enum)]
    public EConnectionStatus ConnectionStatus { get; set; }
}
