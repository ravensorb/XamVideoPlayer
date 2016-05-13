using System;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
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

[assembly: ExportRenderer(typeof(VideoPlayerView),
						  typeof(VideoPlayerViewRenderer))]

namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayer Renderer for Windows Phone Silverlight.
	/// </summary>
	public class VideoPlayerViewRenderer : ViewRenderer<VideoPlayerView, MediaElement>
	{
		MediaElement _mediaElement;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var videoPlayerView = sender as VideoPlayerView;

			if (videoPlayerView != null)
			{
				UpdateOrCreateMediaElement(videoPlayerView, string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0);

				UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		/// <summary>
		/// Reload the view and hit up the MediaElement.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayerView> e)
		{

			base.OnElementChanged(e);

			var videoPlayerView = Element as VideoPlayerView;

			if (e.OldElement != null)
			{
				var oldVideoPlayerVideo = e.OldElement as VideoPlayerView;
			}

			if ((videoPlayerView != null) && (e.OldElement == null))
			{
					if (string.IsNullOrEmpty(videoPlayerView.VideoSource)) return;

				UpdateOrCreateMediaElement(videoPlayerView, true);
			}

		}

		private void UpdateOrCreateMediaElement(VideoPlayerView videoPlayerView, bool updateSource = false)
		{
			if (_mediaElement == null)
			{
				_mediaElement = new MediaElement();

				// Hook up event handlers
				//mediaElement.BufferingProgressChanged += (o, e1) => { };
				//mediaElement.CurrentStateChanged += (o, e1) => { };
				//mediaElement.DownloadProgressChanged += (o, e1) => { };
				//mediaElement.MarkerReached += (o, e1) => { };
				_mediaElement.MediaFailed += (o, e1) => { videoPlayerView.OnMediaErrorOccurred(e1.ErrorMessage); };
				_mediaElement.MediaEnded += (o, e1) => { videoPlayerView.OnMediaCompleted(); };
				_mediaElement.MediaOpened += (o, e1) => { videoPlayerView.OnMediaLoaded(); };
				//mediaElement.RateChanged += (o, e1) => { };
				//_mediaElement.SeekCompleted += (o, e1) => { videoPlayerView.OnSeekCompleted(_mediaElement.Position); };
				//mediaElement.VolumeChanged += (o, e1) => { };

				// Hook up commands
				videoPlayerView.PlayCommand = new Command(() => _mediaElement.Play());
				videoPlayerView.PauseCommand = new Command(() => {
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Pause");
					_mediaElement.Pause();
				}
				, () => _mediaElement.CanPause);
				videoPlayerView.SeekCommand = new Command<TimeSpan>((timeSpan) => {
					_mediaElement.Position = timeSpan;
				}, (timeSpan) =>
				{
					return _mediaElement.CanSeek && timeSpan.TotalSeconds < _mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
				});
				videoPlayerView.StopCommand = new Command(() => _mediaElement.Stop());
				videoPlayerView.MuteCommand = new Command<bool>((muted) => _mediaElement.IsMuted = muted);

				SetNativeControl(_mediaElement);
			}
			else
			{
				UpdateNativeControl();
			}

			//mediaElement.Height = mediaElement.Width / videoPlayerView.VideoScale;

			if (updateSource && !string.IsNullOrEmpty(videoPlayerView.VideoSource))
			{
				_mediaElement.Source = new Uri(videoPlayerView.VideoSource);
			}

			_mediaElement.AutoPlay = videoPlayerView.AutoPlay;
			_mediaElement.Width = videoPlayerView.Width > 0 ? videoPlayerView.Width : 480; // TODO: figure a better way to set the right Width of the current view
			_mediaElement.Height = (videoPlayerView.Height > 0 ? videoPlayerView.Height : (480 / videoPlayerView.VideoScale));

			videoPlayerView.WidthRequest = _mediaElement.Width;
			videoPlayerView.HeightRequest = _mediaElement.Height;
		}
	}
}
