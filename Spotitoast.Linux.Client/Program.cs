using Spectre.Console.Cli;
using Spotitoast.Linux.Client.Commands;

var app = new CommandApp();

app.Configure(config =>
{
      config.SetApplicationName("spotitoast");
      config.SetApplicationVersion("1.0.0");

      config.AddCommand<SendCommand>("send")
            .WithDescription("Send a command to the Spotitoast server.")
            .WithExample("send", "Like")
            .WithExample("send", "Skip")
            .WithExample("send", "TogglePlayback");

      config.AddCommand<StatusCommand>("status")
            .WithDescription("Check if the Spotitoast server is running.");

      // Shorthand commands — each one maps directly to a PlayerCommand.
      config.AddCommand<LikeCommand>("like")
            .WithDescription("Like the currently playing track.");

      config.AddCommand<DislikeCommand>("dislike")
            .WithDescription("Dislike the currently playing track and skip.");

      config.AddCommand<TogglePlaybackCommand>("toggle")
            .WithDescription("Toggle playback (play/pause).");

      config.AddCommand<SkipCommand>("skip")
            .WithDescription("Skip to the next track.");

      config.AddCommand<CurrentlyPlayingCommand>("now")
            .WithDescription("Show the currently playing track notification.");

      config.AddCommand<ExitCommand>("exit")
            .WithDescription("Shut down the Spotitoast server.");
});

return await app.RunAsync(args);
