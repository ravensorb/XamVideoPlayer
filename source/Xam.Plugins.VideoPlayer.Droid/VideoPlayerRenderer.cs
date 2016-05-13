using System;
using System.ComponentModel;
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

			System.Diagnostics.Debug.WriteLine("VideoPlayer: changing element");

			if (Control == null)
			{
				System.Diagnostics.Debug.WriteLine("VideoPlayer: creating control");
				var element = new VideoView(Context);
				element.Prepared += MediaElement_Prepared;
				element.Error += Element_Error;
				element.Completion += Element_Completion;

				//var mediaController = new MediaController(Context);
				//mediaController.SetAnchorView(element);
				//element.SetMediaController(mediaController);
				SetNativeControl(element);
			}

			// Hook up commands
			Element.PlayCommand = new Command(() => Control.Start());
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
				Control.SeekTo((int)timeSpan.TotalMilliseconds);
			}, (timeSpan) =>
			{
				return timeSpan.TotalMilliseconds < Control.CurrentPosition & (Control.CanSeekBackward() || Control.CanSeekForward());
			});
			Element.StopCommand = new Command(() => Control.StopPlayback());
			Element.MuteCommand = new Command<bool>((muted) => { }, (muted) => false);

			UpdateUri();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0)
			{
				UpdateUri();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void Element_Completion(object sender, EventArgs e)
		{
			Element?.OnMediaCompleted();
		}

		private void Element_Error(object sender, Android.Media.MediaPlayer.ErrorEventArgs e)
		{
			Element?.OnMediaErrorOccurred(e.What.ToString());

		}

		private void UpdateUri()
		{
			if (string.IsNullOrEmpty(Element.VideoSource)) return;

			var metrics = Resources.DisplayMetrics;

			Element.HeightRequest = metrics.HeightPixels / metrics.Density / Element.VideoScale;
			Element.WidthRequest = metrics.WidthPixels / metrics.Density / Element.VideoScale;

			var uri = Android.Net.Uri.Parse(Element.VideoSource);

			Control.SetVideoURI(uri);

			if (Element.AutoPlay)
			{
				Control.Start();
			}

		}

		private void MediaElement_Prepared(object sender, EventArgs e)
		{
			Element?.OnMediaLoaded();
		}
	}
}