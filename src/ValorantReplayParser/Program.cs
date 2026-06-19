// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser;

public class Program
{
    // Disable logger to see only correctly parsed exports.
    // Enable logger to see errors/warnings
    private const bool LoggerEnabled = false;

    // Normal: No movement
    // Full: With movement
    private static ParseMode ParseMode = ParseMode.Normal;

    private const string IsolatedSampleReplay1211 = "5c673443-5bdc-4576-b416-aab3f62471a5";
    private const string IsolatedSampleReplay1210 = "9f8b32c5-c243-41ec-bbbb-832582edf652";
    private static readonly string DefaultReplayPath = $"C:\\Users\\michel\\Desktop\\replays\\5d3cd25d-32cf-4a15-afe0-7ed94f752c75.vrf";

    public static int Main(string[] args)
    {
        try
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(options =>
                {
                    options.FormatterName = "minimal";
                });
                builder.SetMinimumLevel(LogLevel.Information);
            });

            ILogger logger = loggerFactory.CreateLogger<ValorantReplayReader>();
            var reader = new ValorantReplayReader(LoggerEnabled ? logger : null, ParseMode);

            var replay = reader.ReadReplay(DefaultReplayPath);

            Console.WriteLine("-------- Finished Reading Replay -------");
            Console.WriteLine($"Date: {replay.Info.Timestamp.ToLongDateString()}");
            Console.WriteLine($"Length: {TimeSpan.FromMilliseconds(replay.Info.LengthInMs).ToString()}");
            Console.WriteLine($"Name: {replay.Info.FriendlyName}");
            Console.WriteLine($"Branch: {replay.Header.Branch}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}

