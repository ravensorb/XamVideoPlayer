using System;
using System.ComponentModel;
using AVFoundation;
using AVKit;
using CoreMedia;
using PropertyChanged;
using UIKit;
using Xam.Plugins.VideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoPlayerView), typeof(VideoPlayerRenderer))]

namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayer Renderer for iOS (Not implemented!).
	/// </summary>
	public class VideoPlayerRenderer : ViewRenderer<VideoPlayerView, UIView>
	{
		//globally declare variables
		AVPlayerViewController _playerController;
		AVPlayerItem _playerItem;
		AVPlayer _player;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public new static void Init()
		{
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var videoPlayerView = sender as VideoPlayerView;

			if (videoPlayerView != null)
			{
				UpdateOrCreateMediaElement(videoPlayerView, string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0);

				//UpdateNativeControl();
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

			var videoPlayerView = e.NewElement;

			if (e.OldElement != null)
			{
				//var oldVideoPlayerVideo = e.OldElement as VideoPlayerView;
			}

			if ((videoPlayerView != null) && (e.OldElement == null))
			{
				UpdateOrCreateMediaElement(videoPlayerView, !string.IsNullOrEmpty(videoPlayerView.VideoSource));
			}

		}

		private void UpdateOrCreateMediaElement(VideoPlayerView videoPlayerView, bool updateSource = false)
		{
			if (_playerController == null)
			{
				_playerController = new AVPlayerViewController {View = {Frame = Frame}};

				UpdatePlayerUrl(videoPlayerView.VideoSource);

				SetNativeControl(_playerController.View);

				// Hook up commands
				Element.PlayCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Play");

					_player?.Play();

				}, () =>
				{
					return _player?.Status == AVPlayerStatus.ReadyToPlay;
				});
				Element.PauseCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Pause");

					_player?.Pause();
				}, () =>
				{
					return _player?.Status == AVPlayerStatus.ReadyToPlay;
				});

				Element.SeekCommand = new Command<TimeSpan>((timeSpan) =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Seek");

					var dt = new CMTime();
					dt.Value = (long)timeSpan.TotalSeconds;
					_player.Seek(dt);
				}, (timeSpan) =>
				{
					return (_player?.Status == AVPlayerStatus.ReadyToPlay) && (timeSpan.TotalMilliseconds < _player?.CurrentTime.Value);
				});
				Element.StopCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Stop");

					_player?.Pause();
				}, () =>
				{
					return _player?.Status == AVPlayerStatus.ReadyToPlay;
				});
				Element.MuteCommand = new Command<bool>((muted) => { _player.Muted = muted; }, (muted) => _player.Muted);
			}

			if (updateSource)
			{
				UpdatePlayerUrl(videoPlayerView.VideoSource);
			}
		}

		private void UpdatePlayerUrl(string url)
		{
			if (string.IsNullOrEmpty(url) || _playerController == null) return;

			_playerItem?.Dispose();
			_player?.Dispose();

			_playerItem = AVPlayerItem.FromUrl(new Foundation.NSUrl(url));
			_playerItem = new AVPlayerItem(new Foundation.NSUrl(url));
			if (_playerItem.Error != null)
			{
				System.Diagnostics.Debug.WriteLine(_playerItem.Error.LocalizedDescription);
			}

			//_player = AVPlayer.FromUrl(new Foundation.NSUrl(url));
			_player = new AVPlayer(_playerItem);
			if (_player.Error != null)
			{
				System.Diagnostics.Debug.WriteLine(_player.Error.LocalizedDescription);
			}

			_player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;

			_playerItem.AddObserver(this, (Foundation.NSString)"status", Foundation.NSKeyValueObservingOptions.Initial | Foundation.NSKeyValueObservingOptions.New, IntPtr.Zero);
			_playerItem.AddObserver(this, (Foundation.NSString)"error", Foundation.NSKeyValueObservingOptions.Initial | Foundation.NSKeyValueObservingOptions.New, IntPtr.Zero);
			_player.AddObserver(this, (Foundation.NSString)"status", Foundation.NSKeyValueObservingOptions.Initial | Foundation.NSKeyValueObservingOptions.New, IntPtr.Zero);
			_player.AddObserver(this, (Foundation.NSString)"error", Foundation.NSKeyValueObservingOptions.Initial | Foundation.NSKeyValueObservingOptions.New, IntPtr.Zero);

			_playerController.Player = _player;
			if (_playerController.Player?.Error != null)
			{
				System.Diagnostics.Debug.WriteLine(_playerController.Player.Error.LocalizedDescription);
			}
		}

		public override void ObserveValue(Foundation.NSString keyPath, Foundation.NSObject ofObject, Foundation.NSDictionary change, IntPtr context)
		{
			if (Equals(ofObject, _playerItem))
			{
				if (keyPath.Equals((Foundation.NSString)"status"))
				{
					if (_playerItem.Status == AVPlayerItemStatus.ReadyToPlay)
					{
						System.Diagnostics.Debug.WriteLine("PlayerItem ReadyToPlay");
					}
				}
				else if (keyPath.Equals((Foundation.NSString)"error"))
				{
					System.Diagnostics.Debug.WriteLine("Error");
				}
			}
			else if (Equals(ofObject, _player))
			{
				if (keyPath.Equals((Foundation.NSString) "status"))
				{
					if (_player.Status == AVPlayerStatus.ReadyToPlay)
					{
						System.Diagnostics.Debug.WriteLine("Player ReadyToPlay");

						Element.OnMediaLoaded();
					}
				}
				else if (keyPath.Equals((Foundation.NSString) "error"))
				{
					System.Diagnostics.Debug.WriteLine("Error");
				}
			}

			//base.ObserveValue(keyPath, ofObject, change, context);
		}

		//public override void LayoutSubviews()
		//{
		//	base.LayoutSubviews();

		//	//layout the elements depending on what screen orientation we are.
		//	if (DeviceHelper.iOSDevice.Orientation == UIDeviceOrientation.Portrait)
		//	{
		//		playButton.Frame = new CGRect(0, NativeView.Frame.Bottom - 50, NativeView.Frame.Width, 50);
		//		_playerLayer.Frame = NativeView.Frame;
		//		NativeView.Layer.AddSublayer(_playerLayer);
		//		NativeView.Add(playButton);
		//	}
		//	else if (DeviceHelper.iOSDevice.Orientation == UIDeviceOrientation.LandscapeLeft || DeviceHelper.iOSDevice.Orientation == UIDeviceOrientation.LandscapeRight)
		//	{
		//		_playerLayer.Frame = NativeView.Frame;
		//		NativeView.Layer.AddSublayer(_playerLayer);
		//		playButton.Frame = new CGRect(0, 0, 0, 0);
		//	}
		//}
	}
}
