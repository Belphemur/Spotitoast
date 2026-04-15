using System;
using System.IO;
using Job.Scheduler.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Spotitoast.Configuration;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Business.Command;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Framework.Extensions;
using Spotitoast.Spotify.Client;
using Spotitoast.Spotify.Configuration;

namespace Spotitoast.Logic.Dependencies
{
    public static class Bootstrap
    {
        /// <summary>
        /// Register all core Spotitoast services into the given service collection.
        /// </summary>
        public static IServiceCollection AddSpotitoastCore(this IServiceCollection services)
        {
            // Configuration
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configManager = new ConfigurationManager(Path.Combine(folderPath, "Spotitoast"));

            services.AddSingleton(configManager);
            services.AddSingleton(sp =>
                sp.GetRequiredService<ConfigurationManager>()
                  .LoadConfiguration<SpotifyAuthConfiguration>().GetAwaiter().GetResult());
            services.AddSingleton(sp =>
                sp.GetRequiredService<ConfigurationManager>()
                  .LoadConfiguration<SpotifyWebClientConfiguration>().GetAwaiter().GetResult());

            // Job scheduler
            services.AddSingleton<IJobScheduler, Job.Scheduler.Scheduler.JobScheduler>();
            services.AddSingleton<IJobRunnerBuilder, JobRunnerBuilder>();

            // Spotify
            services.AddSingleton<SpotifyClient>();
            services.AddSingleton<ISpotifyNotifier, SpotifyNotifier>();

            // Actions
            services.AddSingleton<IAction, Like>();
            services.AddSingleton<IAction, Dislike>();
            services.AddSingleton<IAction, TogglePlayback>();
            services.AddSingleton<IAction, CurrentlyPlaying>();
            services.AddSingleton<IAction, Exit>();
            services.AddSingleton<IAction, SkipTrack>();
            services.AddSingleton<IActionFactory, ActionFactory>();
            services.AddSingleton<ICommandExecutor, CommandExecutor>();

            // HTTP + image downloading
            services.AddHttpClient("ImageDownloader");
            services.AddMemoryCache();
            services.AddSingleton<ImageDownloader>();

            return services;
        }
    }
}