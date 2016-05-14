using System;
using System.ComponentModel;
using Android.Media;
using Android.Widget;
using Xam.Plugins.VideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoPlayerView), typeof(VideoPlayerRenderer))]
namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayer Renderer for Android.
	/// </summary>
	public class VideoPlayerRenderer : ViewRenderer<VideoPlayerView,VideoView>
	{
		private VideoView _videoView;
		//private VideoPlayerView _videoPlayerView;

	    private bool _isPaused;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayerView> e)
		{
			base.OnElementChanged(e);

			var videoPlayerView = Element;

			if (e.OldElement != null)
			{
				//var oldVideoPlayerVideo = e.OldElement as VideoPlayerView;
			}

			if (e.NewElement != null)
			{

			}

			if ((videoPlayerView != null) && (e.OldElement == null))
			{
				UpdateOrCreateMediaElement(true);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var videoPlayerView = sender as VideoPlayerView;

			if (videoPlayerView != null)
			{
				UpdateOrCreateMediaElement(string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0);

				//UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void UpdateOrCreateMediaElement(bool updateSource = false)
		{
			if (Control == null)
			{
				System.Diagnostics.Debug.WriteLine("VideoPlayer: creating control");

				_videoView = new VideoView(Context);
				_videoView.Prepared += MediaElement_Prepared;
				_videoView.Error += Element_Error;
				_videoView.Completion += Element_Completion;

				//var mediaController = new MediaController(Context);
				//mediaController.SetAnchorView(element);
				//element.SetMediaController(mediaController);
				SetNativeControl(_videoView);

				// Hook up commands
				Element.PlayCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Play");

					Control.Start();
				});
				Element.PauseCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Pause");

					if (_isPaused)
					{
						Control.Pause();

						_isPaused = true;
					}
					else
					{
						Control.Resume();

						_isPaused = false;
					}
				}, () => Control.IsPlaying);

				Element.SeekCommand = new Command<TimeSpan>((timeSpan) => {
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Seek");

					Control.SeekTo((int) timeSpan.TotalMilliseconds);
				}, (timeSpan) =>
				{
					return timeSpan.TotalMilliseconds < Control.CurrentPosition &
					       (Control.CanSeekBackward() || Control.CanSeekForward());
				});
				Element.StopCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Stop");

					Control.StopPlayback();
				});
				Element.MuteCommand = new Command<bool>((muted) =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Mute");

				}, (muted) => false);
			}

			if (updateSource)
			{
				UpdatePlayerUrl(Element.VideoSource);
			}
		}

		private void UpdatePlayerUrl(string url)

		{
			if (string.IsNullOrEmpty(url)) return;

			var metrics = Resources.DisplayMetrics;

			Element.HeightRequest = metrics.HeightPixels / metrics.Density / Element.VideoScale;
			Element.WidthRequest = metrics.WidthPixels / metrics.Density / Element.VideoScale;

			var uri = Android.Net.Uri.Parse(url);

			Control.SetVideoURI(uri);
		}

		private void MediaElement_Prepared(object sender, EventArgs e)
		{
			if (sender is MediaPlayer)
			{
				((MediaPlayer)sender).SetVolume((float)(Element.IsMuted ? 0 : Element.VolumeLevel), (float)(Element.IsMuted ? 0 : Element.VolumeLevel));
			}

			if (Element.AutoPlay)
			{
				Control.Start();
			}

			Element?.OnMediaLoaded();
		}

		private void Element_Completion(object sender, EventArgs e)
		{
			Element?.OnMediaCompleted();
		}

		private void Element_Error(object sender, MediaPlayer.ErrorEventArgs e)
		{
			Element?.OnMediaErrorOccurred(e.What.ToString());
		}
	}
}