using System.Collections.Generic;
using Unreal.Core.Models;

namespace ValorantReplayParser.Models;

public class PlayerState
{
    public uint? ActorId { get; set; }
    public uint? ControllerId { get; set; }
    public uint? CharacterId { get; set; }
    public int? PlayerId { get; set; }
    public string? SubjectUniqueId { get; set; }
    public bool? IsAfk { get; set; }
    public EConnectionStatus ConnectionStatus { get; set; }

    public FVector? Position { get; set; }
    public FVector? SpawnLocation { get; set; }
    public bool? IsAlive { get; set; }

    public int? Health { get; set; }
    public int? Armor { get; set; }
    public int? MaxHealth { get; set; }

    public int? Money { get; set; }
    public int? StartOfRoundMoneyCache { get; set; }
    public int? StartOfRoundLoadoutValueCache { get; set; }
    public int? EndOfRoundBeforeRewardsMoney { get; set; }

    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public int? NumDeathStreak { get; set; }

    public bool? bLoadoutFinalized { get; set; }
    public bool? bCanProgressAchievements { get; set; }
    public bool? bOnlySpectator { get; set; }

    public int? CompetitiveTier { get; set; }
    public string? ProfileName { get; set; }

    public List<FAresTrackedReward> TrackedRewards { get; set; } = [];
    public List<FObfuscatedPlayerInformation> AllPlayersInMatch { get; set; } = [];
}
