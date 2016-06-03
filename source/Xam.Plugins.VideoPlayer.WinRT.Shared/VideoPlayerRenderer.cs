using System;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Xam.Plugins.VideoPlayer;

#if NETFX_CORE && WINDOWS_UWP
using Xamarin.Forms.Platform.UWP;
#elif NETFX_CORE && WINDOWS_PHONE_APP
using Xamarin.Forms.Platform.WinRT;
#elif NETFX_CORE && WINDOWS_APP
using Xamarin.Forms.Platform.WinRT;
#elif SILVERLIGHT && WINDOWS_PHONE
using Xamarin.Forms.Platform.WinPhone;
#endif

[assembly: ExportRenderer(typeof(VideoPlayer),
						  typeof(VideoPlayerViewRenderer))]

namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayer Renderer for Windows Phone Silverlight.
	/// </summary>
	public class VideoPlayerViewRenderer : ViewRenderer<VideoPlayer, MediaElement>
	{
		MediaElement _mediaElement;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		/// <summary>
		/// Reload the view and hit up the MediaElement.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
		{
			base.OnElementChanged(e);

			var videoPlayerView = Element;

			if (e.OldElement != null)
			{
				//var oldVideoPlayerVideo = e.OldElement as VideoPlayer;
			}

			if ((videoPlayerView != null) && (e.OldElement == null))
			{
				UpdateOrCreateMediaElement(videoPlayerView, true);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var videoPlayerView = sender as VideoPlayer;

			if (videoPlayerView != null)
			{
				UpdateOrCreateMediaElement(videoPlayerView,
					string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0);

				UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void UpdateOrCreateMediaElement(VideoPlayer videoPlayer, bool updateSource = false)
		{
			if (_mediaElement == null && !string.IsNullOrEmpty(videoPlayer.VideoSource))
			{
				_mediaElement = new MediaElement();
					
				// Hook up event handlers
				//mediaElement.BufferingProgressChanged += (o, e1) => { };
				//mediaElement.CurrentStateChanged += (o, e1) => { };
				//mediaElement.DownloadProgressChanged += (o, e1) => { };
				//mediaElement.MarkerReached += (o, e1) => { };
				_mediaElement.MediaFailed += (o, e1) => { videoPlayer.OnMediaErrorOccurred(e1.ErrorMessage); };
				_mediaElement.MediaEnded += (o, e1) => { videoPlayer.OnMediaCompleted(); };
				_mediaElement.MediaOpened += (o, e1) => { videoPlayer.OnMediaLoaded(); };
				//mediaElement.RateChanged += (o, e1) => { };
				//_mediaElement.SeekCompleted += (o, e1) => { VideoPlayer.OnSeekCompleted(_mediaElement.Position); };
				//mediaElement.VolumeChanged += (o, e1) => { };

				// Hook up commands
				videoPlayer.PlayCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Play");

					_mediaElement.Play();
					Element.MediaState = MediaState.Playing;
				}, () => _mediaElement.Source != null);
				videoPlayer.PauseCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Pause");

					_mediaElement.Pause();
					Element.MediaState = MediaState.Paused;
				}, () => _mediaElement.CanPause);
				videoPlayer.SeekCommand = new Command<TimeSpan>((timeSpan) => {
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Seek");

					var ms = Element.MediaState;
					Element.MediaState = MediaState.Seeking;
					_mediaElement.Position = timeSpan;
					Element.MediaState = ms;
				}, (timeSpan) =>
				{
					return _mediaElement.CanSeek && timeSpan.TotalSeconds < _mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
				});
				videoPlayer.StopCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Stop");

					_mediaElement.Stop();
					Element.MediaState = MediaState.Stopped;
				}, () => _mediaElement.CurrentState == MediaElementState.Playing || _mediaElement.CurrentState == MediaElementState.Paused);
				videoPlayer.MuteCommand = new Command<bool>((muted) =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Mute");

					_mediaElement.IsMuted = muted;
				});

				SetNativeControl(_mediaElement);
			}

			//mediaElement.Height = mediaElement.Width / VideoPlayer.VideoScale;

			if (updateSource)
			{
				UpdatePlayerUrl(videoPlayer.VideoSource);
			}

			if (_mediaElement != null)
			{
				_mediaElement.AutoPlay = videoPlayer.AutoPlay;
				_mediaElement.AreTransportControlsEnabled = videoPlayer.AreControlsDisplayed;
				_mediaElement.Volume = videoPlayer.VolumeLevel;
				_mediaElement.Width = videoPlayer.Width > 0 ? videoPlayer.Width : 480;
				// TODO: figure a better way to set the right Width of the current view
				_mediaElement.Height = (videoPlayer.Height > 0 ? videoPlayer.Height : (480 / videoPlayer.VideoScale));

				videoPlayer.WidthRequest = _mediaElement.Width;
				videoPlayer.HeightRequest = _mediaElement.Height;
			}

			if (Element.AutoPlay)
			{
				Element.PlayCommand.Execute(null);
			}

		}

		private void UpdatePlayerUrl(string url)
		{
			if (string.IsNullOrEmpty(url) || _mediaElement == null) return;

			_mediaElement.Source = new Uri(url);
		}
	}
}
