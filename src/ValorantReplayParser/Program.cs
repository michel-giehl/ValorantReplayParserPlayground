// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Unreal.Core.Models.Enums;
using ValorantReplays;

namespace ValorantReplayParser;

public class Program
{
    private const string IsolatedSampleReplay = "5c673443-5bdc-4576-b416-aab3f62471a5";
    private const string FullMatchReplay = "5d3cd25d-32cf-4a15-afe0-7ed94f752c75";
    private static readonly string DefaultReplayPath = $@"C:\Users\michel\Desktop\replays\{IsolatedSampleReplay}.vrf";

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
            var reader = new ValorantReplayReader(logger, ParseMode.Full);

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

