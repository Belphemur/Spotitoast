using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using Spotitoast.Configuration;
using Spotitoast.Spotify.Client.Auth;
using Spotitoast.Spotify.Configuration;
using Spotitoast.Spotify.Model;
using Unosquare.Swan.Components;

namespace Spotitoast.Spotify.Client
{
    public class SpotifyClient
    {
        private readonly SpotifyWebAPI _spotifyWebClient;
        private readonly SpotifyWebClientConfiguration _webConfiguration;

        private PlaybackContext _playbackContext;

        public bool IsPlaying => _playbackContext?.IsPlaying ?? false;


        private readonly ISubject<FullTrack> _playedTrackSubject = new Subject<FullTrack>();
        private readonly ISubject<FullTrack> _trackLiked = new Subject<FullTrack>();
        private readonly ISubject<FullTrack> _trackDisliked = new Subject<FullTrack>();
        private readonly Timer _timer;
        private SpotifyAuth _authClient;

        public IObservable<FullTrack> PlayedTrack => _playedTrackSubject.AsObservable();
        public IObservable<FullTrack> TrackLiked => _trackLiked.AsObservable();
        public IObservable<FullTrack> TrackDisliked => _trackDisliked.AsObservable();

        public SpotifyClient(SpotifyWebClientConfiguration webConfiguration, SpotifyAuthConfiguration authConfiguration)
        {
            _spotifyWebClient = new SpotifyWebAPI()
            {
                UseAuth = true,
                UseAutoRetry = true
            };
            _webConfiguration = webConfiguration;
            _authClient = new SpotifyAuth(authConfiguration);
            _authClient.TokenUpdated += AuthOnTokenUpdated;
            _authClient.RefreshAccessToken();
            _timer = InitTimerTrack();
        }

        private void AuthOnTokenUpdated(object sender, SpotifyAuth.TokenUpdatedEventArg e)
        {
            _spotifyWebClient.AccessToken = e.NewToken.AccessToken;
            _spotifyWebClient.TokenType = e.NewToken.TokenType;
        }

        private Timer InitTimerTrack()
        {
            return new Timer(
                callback: async e =>
                {
                    var result = await CheckCurrentPlayedTrack();
                    if (result == ActionResult.Error)
                    {
                        _authClient.RefreshAccessToken();
                    }
                },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(_webConfiguration.CheckCurrentlyPlayedSeconds));
        }

        private async Task<ActionResult> CheckCurrentPlayedTrack(bool forceNotify = false)
        {
            try
            {
                Trace.WriteLine("Checking for playing track");
                var trackResponse = await _spotifyWebClient.GetPlayingTrackAsync();

                if (trackResponse.HasError())
                {
                    Trace.WriteLine(trackResponse.Error.Message);
                    return ActionResult.Error;
                }

                var oldTrack = _playbackContext?.Item;
                _playbackContext = trackResponse;

                var playedTrack = _playbackContext?.Item;

                Trace.WriteLine($"Track Found: ${playedTrack?.Id}");

                if (playedTrack == null)
                {
                    return ActionResult.Error;
                }

                if (forceNotify
                    || oldTrack == null
                    || playedTrack.Id != oldTrack.Id)
                {
                    _playedTrackSubject.OnNext(playedTrack);
                }
            }
            catch (Exception)
            {
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
            var track = _playbackContext?.Item;
            var trackId = track?.Id;

            Trace.WriteLine($"Loving ${trackId}.");
            var loveState = await CheckLoveState(trackId);
            if (loveState != ActionResult.NotLoved)
            {
                return loveState;
            }

            var result = await _spotifyWebClient.SaveTrackAsync(trackId);
            if (result.HasError())
            {
                Trace.WriteLine(result.Error.Message);
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
            return (await CheckLoveState(trackId)) == ActionResult.AlreadyLoved;
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

            var isLovedResult = await _spotifyWebClient.CheckSavedTracksAsync(new List<string> {trackId});
            if (isLovedResult.HasError())
            {
                return ActionResult.Error;
            }

            return isLovedResult.List.First() ? ActionResult.AlreadyLoved : ActionResult.NotLoved;
        }

        /// <summary>
        /// Unlove the Played Track
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DislikePlayedTrack()
        {
            var track = _playbackContext?.Item;
            var trackId = track?.Id;
            Trace.WriteLine($"Dislinking ${trackId}.");
            var result = await _spotifyWebClient.SkipPlaybackToNextAsync();

            if (result.HasError())
            {
                Trace.WriteLine(result.Error.Message);
                return ActionResult.Error;
            }

            _trackDisliked.OnNext(track);

            var loveState = await CheckLoveState(trackId);
            if (loveState != ActionResult.AlreadyLoved)
            {
                return loveState;
            }

            result = await _spotifyWebClient.RemoveSavedTracksAsync(new List<string> {trackId});
            return result.HasError() ? ActionResult.Error : ActionResult.Success;
        }

        /// <summary>
        /// Resume playback
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Resume()
        {
            var deviceId = _playbackContext?.Device?.Id;
            return await ResumeInternal(deviceId);
        }

        private async Task<ActionResult> ResumeInternal(string deviceId)
        {
            var result =
                await _spotifyWebClient.ResumePlaybackAsync(deviceId, "",
                    null, "", 0);

            if (deviceId == null && result.HasError() && result.Error.Status == 404)
            {
                var firstDevice = await GetFirstDevice();
                if (firstDevice == null)
                {
                    return ActionResult.Error;
                }

                return await ResumeInternal(firstDevice.Id);
            }

            if (result.HasError())
            {
                return ActionResult.Error;
            }


            if (_playbackContext != null) _playbackContext.IsPlaying = true;

            return ActionResult.Success;
        }

        private async Task<Device> GetFirstDevice()
        {
            var devices = await _spotifyWebClient.GetDevicesAsync();
            return devices.HasError() ? null : devices.Devices.First(device => device.Type.ToLower() == "computer");
        }

        /// <summary>
        /// Resume playback
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Pause()
        {
            var result =
                await _spotifyWebClient.PausePlaybackAsync(_playbackContext?.Device?.Id);
            if (result.HasError())
                return ActionResult.Error;

            if (_playbackContext != null) _playbackContext.IsPlaying = false;

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