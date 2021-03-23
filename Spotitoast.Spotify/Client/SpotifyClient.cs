using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Scheduler;
using SpotifyAPI.Web;
using Spotitoast.Spotify.Client.Auth;
using Spotitoast.Spotify.Client.Job;
using Spotitoast.Spotify.Configuration;
using Spotitoast.Spotify.Model;
using SpotifyWebClient = SpotifyAPI.Web.SpotifyClient;

namespace Spotitoast.Spotify.Client
{
    public class SpotifyClient
    {
        private readonly SpotifyWebClient _spotifyWebClient;

        [CanBeNull] private CurrentlyPlayingContext _playbackContext;


        private readonly ISubject<FullTrack> _playedTrackSubject = new Subject<FullTrack>();
        private readonly ISubject<FullTrack> _trackLiked = new Subject<FullTrack>();
        private readonly ISubject<FullTrack> _trackDisliked = new Subject<FullTrack>();
        private readonly SpotifyAuth _authClient;
        private readonly SpotifyAuthConfiguration _spotifyAuthConfiguration;
        private readonly TokenAuthenticator _tokenAuthenticator = new("", "Bearer");

        public IObservable<FullTrack> PlayedTrack => _playedTrackSubject.AsObservable();
        public IObservable<FullTrack> TrackLiked => _trackLiked.AsObservable();
        public IObservable<FullTrack> TrackDisliked => _trackDisliked.AsObservable();
        public bool IsPlaying => _playbackContext?.IsPlaying ?? false;

        public SpotifyClient(SpotifyWebClientConfiguration webConfiguration, SpotifyAuthConfiguration authConfiguration, IJobScheduler jobScheduler)
        {
            _spotifyAuthConfiguration = authConfiguration;
            if (_spotifyAuthConfiguration.LastToken != null)
            {
                _tokenAuthenticator.Token = _spotifyAuthConfiguration.LastToken.AccessToken;
            }

            _spotifyWebClient = new SpotifyWebClient(SpotifyClientConfig.CreateDefault().WithAuthenticator(_tokenAuthenticator));
            _authClient = new SpotifyAuth(_spotifyAuthConfiguration, jobScheduler);
            _authClient.TokenUpdated += AuthOnTokenUpdated;
            jobScheduler.ScheduleJob(new CheckCurrentlyPlayingJob(this, TimeSpan.FromSeconds(webConfiguration.CheckCurrentlyPlayedSeconds)));
        }

        private void AuthOnTokenUpdated(object sender, SpotifyAuth.TokenUpdatedEventArg e)
        {
            _tokenAuthenticator.Token = e.NewToken.AccessToken;
        }

        internal async Task CheckCurrentPlayedTrackWithAutoRefresh()
        {
            if (_spotifyAuthConfiguration.LastToken?.IsExpired() ?? false)
            {
                await _authClient.RefreshAccessToken();
            }

            var result = await CheckCurrentPlayedTrack();
            if (result == ActionResult.Error)
            {
                await _authClient.RefreshAccessToken();
            }
        }

        private async Task<ActionResult> CheckCurrentPlayedTrack(bool forceNotify = false)
        {
            try
            {
                Trace.WriteLine("Checking for playing track");

                var trackResponse = await _spotifyWebClient.Player.GetCurrentPlayback(new PlayerCurrentPlaybackRequest());

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (trackResponse == null || !trackResponse.IsPlaying)
                {
                    _playbackContext = trackResponse;
                    return ActionResult.NoTrackPlayed;
                }

                if (!(trackResponse.Item is FullTrack fullTrack))
                {
                    return ActionResult.Error;
                }


                var oldTrack = _playbackContext?.Item as FullTrack;
                _playbackContext = trackResponse;

                var playedTrack = fullTrack;

                Trace.WriteLine($"Track Found: ${fullTrack.Id}");

                if (forceNotify
                    || oldTrack == null
                    || fullTrack.Id != oldTrack.Id)
                {
                    _playedTrackSubject.OnNext(playedTrack);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return ActionResult.Error;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Love the Currently played track
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> LikePlayedTrack()
        {
            var track = _playbackContext?.Item as FullTrack;
            var trackId = track?.Id;

            Trace.WriteLine($"Loving ${trackId}.");
            var loveState = await CheckLoveState(trackId);
            if (loveState != ActionResult.NotLiked)
            {
                return loveState;
            }

            try
            {
                var result = await _spotifyWebClient.Library.SaveTracks(new LibrarySaveTracksRequest(new[] {trackId}));
                if (!result)
                {
                    return ActionResult.Error;
                }
            }
            catch (APIException e)
            {
                Trace.Write(e);
                return ActionResult.Error;
            }

            Trace.WriteLine($"Track ${trackId} loved.");

            _trackLiked.OnNext(track);
            return ActionResult.Success;
        }

        /// <summary>
        /// Check if the given track is loved
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public async Task<bool> IsLoved(string trackId)
        {
            return (await CheckLoveState(trackId)) == ActionResult.AlreadyLiked;
        }

        /// <summary>
        /// Check if the track is loved or not
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        private async Task<ActionResult> CheckLoveState(string trackId)
        {
            if (trackId == null)
            {
                return ActionResult.NoTrackPlayed;
            }

            try
            {
                var isLovedResult = await _spotifyWebClient.Library.CheckTracks(new LibraryCheckTracksRequest(new[] {trackId}));
                return isLovedResult.First() ? ActionResult.AlreadyLiked : ActionResult.NotLiked;
            }
            catch (APIException e)
            {
                Trace.Write(e);
                return ActionResult.Error;
            }
        }

        /// <summary>
        /// Unlove the Played Track
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DislikePlayedTrack()
        {
            var track = _playbackContext?.Item as FullTrack;
            var trackId = track?.Id;
            Trace.WriteLine($"Dislinking ${trackId}.");
            var resultSkip = await SkipTrack();

            if (resultSkip == ActionResult.Error)
            {
                return resultSkip;
            }

            _trackDisliked.OnNext(track);

            var loveState = await CheckLoveState(trackId);
            if (loveState != ActionResult.AlreadyLiked)
            {
                return loveState;
            }

            try
            {
                var result = await _spotifyWebClient.Library.RemoveTracks(new LibraryRemoveTracksRequest(new[] {trackId}));
                if (!result)
                {
                    return ActionResult.NotLiked;
                }
            }
            catch (APIException e)
            {
                Trace.Write(e);
                return ActionResult.Error;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Skip the current track
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SkipTrack()
        {
            Trace.WriteLine("Skipping track");
            try
            {
                var result = await _spotifyWebClient.Player.SkipNext();
                if (!result)
                {
                    return ActionResult.Error;
                }
            }
            catch (APIException e)
            {
                Trace.Write(e);
                return ActionResult.Error;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Resume playback
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Resume()
        {
            var result = await ResumeInternal(_playbackContext?.Device.Id);
            if (result == ActionResult.Error)
                return result;

            await Task.Delay(TimeSpan.FromMilliseconds(250));

            return await CheckCurrentPlayedTrack(true);
        }

        private async Task<ActionResult> ResumeInternal([CanBeNull] string deviceId = null)
        {
            try
            {
                await _spotifyWebClient.Player.ResumePlayback(new PlayerResumePlaybackRequest
                {
                    DeviceId = deviceId
                });
            }
            catch (APIException e) when (e.Response?.StatusCode == HttpStatusCode.NotFound)
            {
                var firstDevice = await GetFirstDevice();
                if (firstDevice == null)
                {
                    return ActionResult.Error;
                }

                return await ResumeInternal(firstDevice.Id);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return ActionResult.Error;
            }

            if (_playbackContext != null) _playbackContext.IsPlaying = true;

            return ActionResult.Success;
        }

        [ItemCanBeNull]
        private async Task<Device> GetFirstDevice()
        {
            try
            {
                var devices = await _spotifyWebClient.Player.GetAvailableDevices();
                return devices.Devices.FirstOrDefault(device => device.Type.ToLower() == "computer");
            }
            catch (APIException)
            {
                return null;
            }
        }

        /// <summary>
        /// Resume playback
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Pause()
        {
            try
            {
                var result = await _spotifyWebClient.Player.PausePlayback();
                if (!result)
                    return ActionResult.Error;

                if (_playbackContext != null) _playbackContext.IsPlaying = false;
            }
            catch (APIException e) when (e.Message.Contains("No active device found"))
            {
                _playbackContext = null;
            }
            catch (APIException e)
            {
                Trace.Write(e);
                return ActionResult.Error;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Force checking for the current playing track
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ForceCheckCurrentlyPlaying()
        {
            return await CheckCurrentPlayedTrack(true);
        }
    }
}