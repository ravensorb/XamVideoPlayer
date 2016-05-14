using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer;
using Com.Google.Android.Exoplayer.Audio;
using Com.Google.Android.Exoplayer.Drm;
using Com.Google.Android.Exoplayer.Text;
using Com.Google.Android.Exoplayer.Util;
using Java.Lang;
using Xam.Plugins.VideoPlayer;
using Xam.Plugins.VideoPlayer.ExoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoPlayerView), typeof(VideoPlayerRenderer))]
namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayer Renderer for Android.
	/// </summary>
	public class VideoPlayerRenderer : ViewRenderer<VideoPlayerView, AspectRatioFrameLayout>
	{
		public const int TypeDash = 0;
		public const int TypeSs = 1;
		public const int TypeHls = 2;
		public const int TypeOther = 3;

		private const string ContentExtExtra = "type";
		private const string ExtDash = ".mpd";
		private const string ExtSs = ".ism";
		private const string ExtHls = ".m3u8";

		private MediaController _mediaController;
		//private View _debugRootView;
		//private View _shutterView;
		private AspectRatioFrameLayout _videoFrame;
		private SurfaceView _surfaceView;
		//private TextView _debugTextView;
		//private TextView _playerStateTextView;
		//private SubtitleLayout _subtitleLayout;

		private ExoPlayer.VideoPlayer _player;
		private DebugTextViewHelper _debugViewHelper;
		private bool _playerNeedsPrepare;

		private long _playerPosition;
		private bool _enableBackgroundAudio;

		private Android.Net.Uri _contentUri;
		private int _contentType;
		private string _contentId;

		private AudioCapabilitiesReceiver _audioCapabilitiesReceiver;

//		private bool _isPaused;

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
			if (_player == null)
			{
				//SetContentView(Resource.Layout.player_activity);
				//var root = FindViewById(Resource.Id.root);

				_videoFrame = new AspectRatioFrameLayout(Context);
				_surfaceView = new SurfaceView(Context);

				_videoFrame.AddView(_surfaceView);

				SetNativeControl(_videoFrame);

				_mediaController = new MediaController(Context);
				_mediaController.SetAnchorView(this);

				_audioCapabilitiesReceiver = new AudioCapabilitiesReceiver(Context, null);
				_audioCapabilitiesReceiver.Register();
			}

			if (updateSource)
			{
				UpdatePlayerUrl(Element.VideoSource);
			}
		}

		private void UpdatePlayerUrl(string url)

		{
			if (string.IsNullOrEmpty(url)) return;

			OnResume();
		}

		#region Activity lifecycle
		protected void OnNewIntent(Intent intent)
		{
			ReleasePlayer();
			_playerPosition = 0;
		}

		protected void OnResume()
		{
			_contentUri = Android.Net.Uri.Parse(Element.VideoSource);
			_contentType = InferContentType(_contentUri, System.IO.Path.GetExtension(_contentUri.LastPathSegment));
			//_contentId = intent.GetStringExtra(ContentIdExtra);

			if (_player == null)
			{
				PreparePlayer(true);
			}
			else
			{
				_player.Backgrounded = false;
			}
		}

		protected void OnPause()
		{
			if (!_enableBackgroundAudio)
			{
				ReleasePlayer();
			}
			else
			{
				_player.Backgrounded = true;
			}
			//_shutterView.Visibility = ViewStates.Visible;
		}

		protected void OnDestroy()
		{
			_audioCapabilitiesReceiver.Unregister();
			ReleasePlayer();
		}

		#endregion

		#region OnClickListener methods

		public void OnClick()
		{
			PreparePlayer(true);
		}

		#endregion

		#region AudioCapabilitiesReceiver.Listener methods

		public void OnAudioCapabilitiesChanged(AudioCapabilities audioCapabilities)
		{
			if (_player == null)
			{
				return;
			}
			var backgrounded = _player.Backgrounded;
			var playWhenReady = _player.PlayWhenReady;
			ReleasePlayer();
			PreparePlayer(playWhenReady);
			_player.Backgrounded = backgrounded;
		}

		#endregion

		#region Internal methods

		private ExoPlayer.VideoPlayer.IRendererBuilder GetRendererBuilder()
		{
			var userAgent = ExoPlayerUtil.GetUserAgent(Context, "ExoPlayerDemo");
			switch (_contentType)
			{
				case TypeSs:
					return new SmoothStreamingRendererBuilder(Context, userAgent, _contentUri.ToString(),
						new SmoothStreamingTestMediaDrmCallback());
				case TypeDash:
					return new DashRendererBuilder(Context, userAgent, _contentUri.ToString(),
						new ExoPlayer.WidevineTestMediaDrmCallback(_contentId));
				case TypeHls:
					return new HlsRendererBuilder(Context, userAgent, _contentUri.ToString());
				case TypeOther:
					return new ExtractorRendererBuilder(Context, userAgent, Android.Net.Uri.Parse(Element.VideoSource));
				default:
					throw new IllegalStateException("Unsupported type: " + _contentType);
			}
		}

		private void PreparePlayer(bool playWhenReady)
		{
			if (_player == null)
			{
				_player = new ExoPlayer.VideoPlayer(GetRendererBuilder());
				//_player.AddListener(this);
				//_player.SetCaptionListener(this);
				//_player.SetMetadataListener(this);
				_player.SeekTo(_playerPosition);
				_playerNeedsPrepare = true;
				_mediaController.SetMediaPlayer(_player.PlayerControl);
				_mediaController.Enabled = true;
				//_debugViewHelper = new DebugTextViewHelper(_player, _debugTextView);
				//_debugViewHelper.Start();
			}
			if (_playerNeedsPrepare)
			{
				_player.Prepare();
				_playerNeedsPrepare = false;
			}
			_player.Surface = _surfaceView.Holder.Surface;
			_player.PlayWhenReady = playWhenReady;
		}

		private void ReleasePlayer()
		{
			if (_player != null)
			{
				_debugViewHelper.Stop();
				_debugViewHelper = null;
				_playerPosition = _player.CurrentPosition;
				_player.Release();
				_player = null;
			}
		}

		#endregion

		#region DemoPlayer.Listener implementation

		public void OnStateChanged(bool playWhenReady, int playbackState)
		{
			if (playbackState == global::Com.Google.Android.Exoplayer.ExoPlayer.StateEnded)
			{
			}
			var text = "playWhenReady=" + playWhenReady + ", playbackState=";
			switch (playbackState)
			{
				case global::Com.Google.Android.Exoplayer.ExoPlayer.StateBuffering:
					text += "buffering";
					break;
				case global::Com.Google.Android.Exoplayer.ExoPlayer.StateEnded:
					text += "ended";
					break;
				case global::Com.Google.Android.Exoplayer.ExoPlayer.StateIdle:
					text += "idle";
					break;
				case global::Com.Google.Android.Exoplayer.ExoPlayer.StatePreparing:
					text += "preparing";
					break;
				case global::Com.Google.Android.Exoplayer.ExoPlayer.StateReady:
					text += "ready";
					break;
				default:
					text += "unknown";
					break;
			}
			//_playerStateTextView.Text = text;
		}

		public void OnError(System.Exception e)
		{
			var exception = e as UnsupportedDrmException;
			if (exception != null)
			{
				// Special case DRM failures.
				var stringId = ExoPlayerUtil.SdkInt < 18
					? "rm_error_not_supported"
					: exception.Reason == UnsupportedDrmException.ReasonUnsupportedScheme
						? "drm_error_unsupported_scheme"
						: "drm_error_unknown";
				Toast.MakeText(Context, stringId, ToastLength.Long).Show();
			}
			_playerNeedsPrepare = true;
		}

		public void OnVideoSizeChanged(
			int width,
			int height,
			int unappliedRotationDegrees,
			float pixelWidthAspectRatio)
		{
			//_shutterView.Visibility = ViewStates.Gone;
			_videoFrame.SetAspectRatio(height == 0 ? 1 : (width * pixelWidthAspectRatio) / height);
		}

		#endregion

		#region DemoPlayer.CaptionListener implementation

		public void OnCues(IList<Cue> cues)
		{
			//_subtitleLayout.SetCues(cues);
		}

		#endregion

		#region DemoPlayer.MetadataListener implementation

		public void OnId3Metadata(object metadata)
		{
			/*for (Map.Entry<String, Object> entry : metadata.entrySet()) {
	  if (TxxxMetadata.TYPE.equals(entry.getKey())) {
		TxxxMetadata txxxMetadata = (TxxxMetadata) entry.getValue();
		Log.i(TAG, String.format("ID3 TimedMetadata %s: description=%s, value=%s",
			TxxxMetadata.TYPE, txxxMetadata.description, txxxMetadata.value));
	  } else if (PrivMetadata.TYPE.equals(entry.getKey())) {
		PrivMetadata privMetadata = (PrivMetadata) entry.getValue();
		Log.i(TAG, String.format("ID3 TimedMetadata %s: owner=%s",
			PrivMetadata.TYPE, privMetadata.owner));
	  } else if (GeobMetadata.TYPE.equals(entry.getKey())) {
		GeobMetadata geobMetadata = (GeobMetadata) entry.getValue();
		Log.i(TAG, String.format("ID3 TimedMetadata %s: mimeType=%s, filename=%s, description=%s",
			GeobMetadata.TYPE, geobMetadata.mimeType, geobMetadata.filename,
			geobMetadata.description));
	  } else {
		Log.i(TAG, String.format("ID3 TimedMetadata %s", entry.getKey()));
	  }
	}*/
		}

		#endregion

		#region SurfaceHolder.Callback implementation

		public void SurfaceCreated(ISurfaceHolder holder)
		{
			if (_player != null)
			{
				_player.Surface = holder.Surface;
			}
		}

		public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
		{
			// Do nothing.
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			if (_player != null)
			{
				_player.BlockingClearSurface();
			}
		}

		#endregion

		/// <summary>
		/// Makes a best guess to infer the type from a media <see cref="Uri"/> and an optional overriding file extension.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> of the media.</param>
		/// <param name="fileExtension">An overriding file extension.</param>
		/// <returns>The inferred type.</returns>
		private static int InferContentType(Android.Net.Uri uri, string fileExtension)
		{
			var lastPathSegment = !string.IsNullOrEmpty(fileExtension)
				? "." + fileExtension
				: uri.LastPathSegment;
			if (lastPathSegment == null)
			{
				return TypeOther;
			}
			if (lastPathSegment.EndsWith(ExtDash))
			{
				return TypeDash;
			}
			if (lastPathSegment.EndsWith(ExtSs))
			{
				return TypeSs;
			}
			if (lastPathSegment.EndsWith(ExtHls))
			{
				return TypeHls;
			}
			return TypeOther;
		}

		private class KeyCompatibleMediaController : MediaController
		{
			private IMediaPlayerControl _playerControl;

			public KeyCompatibleMediaController(Context context) : base(context)
			{
			}

			public override void SetMediaPlayer(IMediaPlayerControl playerControl)
			{
				base.SetMediaPlayer(playerControl);
				_playerControl = playerControl;
			}

			public override bool DispatchKeyEvent(KeyEvent ev)
			{
				var keyCode = ev.KeyCode;
				if (_playerControl.CanSeekForward() && (keyCode == Keycode.MediaFastForward || keyCode == Keycode.DpadRight))
					if (_playerControl.CanSeekForward() && keyCode == Keycode.MediaFastForward)
					{
						if (ev.Action == KeyEventActions.Down)
						{
							_playerControl.SeekTo(_playerControl.CurrentPosition + 15000); // milliseconds
							Show();
						}
						return true;
					}
					else if (_playerControl.CanSeekBackward() && (keyCode == Keycode.MediaRewind || keyCode == Keycode.DpadLeft))
					{
						if (ev.Action == KeyEventActions.Down)
						{
							_playerControl.SeekTo(_playerControl.CurrentPosition - 5000); // milliseconds
							Show();
						}
						return true;
					}
				return base.DispatchKeyEvent(ev);
			}
		}
	}
}
