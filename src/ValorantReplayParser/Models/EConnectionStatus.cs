namespace ValorantReplayParser.Models;

public enum EConnectionStatus
{
    Uninitialized = 0,
    Disconnected = 1,
    Unresponsive = 2,
    Connecting = 3,
    Connected = 4,
    Count = 5,
    EConnectionStatus_MAX = 6
}
