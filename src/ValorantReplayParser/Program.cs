// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Unreal.Core.Models.Enums;
using ValorantReplays;

namespace ValorantReplayParser;

public class Program
{
    private const string IsolatedSampleReplay = "9f8b32c5-c243-41ec-bbbb-832582edf652";
    private const string FullMatchReplay = "530afd78-cabd-4758-ba71-b27af2d06a74";
    private const string FullMatchReplay1211 = "5d3cd25d-32cf-4a15-afe0-7ed94f752c75";
    private static readonly string DefaultReplayPath = $@"C:\Users\michel\Desktop\replays\{FullMatchReplay1211}.vrf";

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
                builder.AddConsoleFormatter<MinimalConsoleFormatter, ConsoleFormatterOptions>();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            ILogger logger = loggerFactory.CreateLogger<ValorantReplayReader>();
            var reader = new ValorantReplayReader(null, ParseMode.Debug);

            reader.ReadReplay(DefaultReplayPath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}

