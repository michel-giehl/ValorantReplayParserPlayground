using Unreal.Core.Models;
using ValorantReplayParser.Models;

namespace ValorantReplayParser;

public class ValorantReplay : Replay
{
    public Dictionary<uint, PlayerState> PlayerStates { get; set; } = [];
    public Dictionary<uint, uint> CharacterToPlayer { get; set; } = [];
}
