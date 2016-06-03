// ***********************************************************************
// Assembly         : Xam.Plugins.VideoPlayer.IOS
// Author           : raven
// Created          : 05-12-2016
//
// Last Modified By : raven
// Last Modified On : 05-14-2016
// ***********************************************************************
// <copyright file="VideoPlayerRenderer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AVFoundation;
using AVKit;
using CoreMedia;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Xam.Plugins.VideoPlayer.VideoPlayer), typeof(Xam.Plugins.VideoPlayer.iOS.VideoPlayerRenderer))]

namespace Xam.Plugins.VideoPlayer.iOS
{
	/// <summary>
	/// VideoPlayer Renderer for iOS (Not implemented!).
	/// </summary>
	public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, UIView>
	{
		#region Fields
		AVPlayerViewController _playerController;
		AVPlayer _player;
		#endregion Fields

		#region Properties

		private AVPlayer SafePlayer => _playerController?.Player;

		#endregion Properties

		#region Methods: Overrides
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public new static void Init()
		{
			System.Diagnostics.Debug.WriteLine("VideoPlayerRenderer for iOS Init Called");
		}

		/// <summary>
		/// Reload the view and hit up the MediaElement.
		/// </summary>
		/// <param name="e">The e.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
		{
			base.OnElementChanged(e);

			var videoPlayerView = e.NewElement;

			if (e.OldElement != null)
			{
				//var oldVideoPlayerVideo = e.OldElement as VideoPlayer;
			}

			if ((videoPlayerView != null) && (e.OldElement == null))
			{
				UpdateOrCreateMediaElement(videoPlayerView, !string.IsNullOrEmpty(videoPlayerView.VideoSource));
			}
		}

		/// <summary>
		/// Handles the <see cref="E:ElementPropertyChanged" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var videoPlayerView = sender as VideoPlayer;

			if (videoPlayerView != null)
			{
				UpdateOrCreateMediaElement(videoPlayerView, string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0);

				//UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		/// <summary>
		/// Indicates that the value at the specified keyPath relative to this object has changed.
		/// </summary>
		/// <param name="keyPath">Key-path to use to perform the value lookup.   The keypath consists of a series of lowercase ASCII-strings with no spaces in them separated by dot characters.</param>
		/// <param name="ofObject">The of object.</param>
		/// <param name="change">A dictionary that describes the changes that have been made to the value of the property at the key path keyPath relative to object. Entries are described in Change Dictionary Keys.</param>
		/// <param name="context">The value that was provided when the receiver was registered to receive key-value observation notifications.</param>
		/// <remarks>This method is invoked if you have registered an observer using the <see cref="M:Foundation.NSObject.AddObserver" /> method</remarks>
		public override void ObserveValue(Foundation.NSString keyPath, Foundation.NSObject ofObject, Foundation.NSDictionary change, IntPtr context)
		{
			if (Equals(ofObject, _player))
			{
				if (keyPath.Equals((Foundation.NSString)"status"))
				{
					if (_player.Status == AVPlayerStatus.ReadyToPlay)
					{
						System.Diagnostics.Debug.WriteLine("Player ReadyToPlay");

						Element.OnMediaLoaded();

						if (Element.AutoPlay)
						{
							Element.PlayCommand.Execute(null);
						}
					}
				}
				else if (keyPath.Equals((Foundation.NSString)"error") && _player.Error != null)
				{
					System.Diagnostics.Debug.WriteLine("Error");

					Element.OnMediaErrorOccurred(_player.Error?.LocalizedDescription ?? "Error loading player");
				}
			}

			//base.ObserveValue(keyPath, ofObject, change, context);
		}

		#endregion Methods: Overrides
		
		#region Methods: Private
		/// <summary>
		/// Updates the or create media element.
		/// </summary>
		/// <param name="videoPlayer">The video player view.</param>
		/// <param name="updateSource">if set to <c>true</c> [update source].</param>
		private void UpdateOrCreateMediaElement(VideoPlayer videoPlayer, bool updateSource = false)
		{
			if (_playerController == null)
			{
				if (_playerController == null)
				{
					_playerController = new AVPlayerViewController { View = { Frame = Frame }, ShowsPlaybackControls = videoPlayer.AreControlsDisplayed };
				}

				UpdatePlayerUrl(videoPlayer.VideoSource);

				SetNativeControl(_playerController.View);

				// Hook up commands
				videoPlayer.PlayCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Play");

					SafePlayer?.Play();

					videoPlayer.MediaState = MediaState.Playing;
				}, () =>
				{
					return SafePlayer?.Status == AVPlayerStatus.ReadyToPlay;
				});
				videoPlayer.PauseCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Pause");

					SafePlayer?.Pause();

					videoPlayer.MediaState = MediaState.Paused;
				});

				videoPlayer.SeekCommand = new Command<TimeSpan>((timeSpan) =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Seek");

					var dt = new CMTime {Value = (long) timeSpan.TotalSeconds};

					var ms = videoPlayer.MediaState;
					videoPlayer.MediaState = MediaState.Seeking;
					SafePlayer.Seek(dt);
					videoPlayer.MediaState = ms;

				}, (timeSpan) =>
				{
					return (SafePlayer?.Status == AVPlayerStatus.ReadyToPlay) && (timeSpan.TotalMilliseconds < SafePlayer?.CurrentTime.Value) && (SafePlayer?.CurrentTime.Value > 0);
				});
				videoPlayer.StopCommand = new Command(() =>
				{
					System.Diagnostics.Debug.WriteLine("VideoPlayer: Stop");

					SafePlayer?.Pause();
					videoPlayer.MediaState = MediaState.Stopped;
				});
				videoPlayer.MuteCommand = new Command<bool>((muted) => { if (SafePlayer != null) SafePlayer.Muted = muted; });
			} 
			else if (updateSource)
			{
				UpdatePlayerUrl(videoPlayer.VideoSource);
			}

			_playerController.ShowsPlaybackControls = videoPlayer.AreControlsDisplayed;

			if (SafePlayer != null)
			{
				if ((int)videoPlayer.VolumeLevel != 0)
				{
					SafePlayer.Volume = (float) videoPlayer.VolumeLevel / 100; // iOS is a slide scale from 0 to 1, so if divde our 0 to 100 number we should get the correct volume
				}
				SafePlayer.Muted = videoPlayer.IsMuted;
			}
		}

		/// <summary>
		/// Updates the player URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		private void UpdatePlayerUrl(string url)
		{
			if (string.IsNullOrEmpty(url)) return;

			if (_player != null)
			{
				//_player.RemoveObserver(this, (Foundation.NSString)"status");
				//_player.RemoveObserver(this, (Foundation.NSString)"error");
				_player.Dispose();
				_player = null;
			}			
			
			//_player = AVPlayer.FromUrl(new Foundation.NSUrl(url));
			_player = new AVPlayer(Foundation.NSUrl.FromString(url));
			if (_player.Error != null)
			{
				System.Diagnostics.Debug.WriteLine(_player.Error.LocalizedDescription);

				Element.OnMediaErrorOccurred(_player?.Error?.LocalizedDescription);

				throw new InvalidOperationException(_player?.Error?.LocalizedDescription);
			}

			_player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;

			//_player.AddObserver(this, (Foundation.NSString)"status", Foundation.NSKeyValueObservingOptions.Initial | Foundation.NSKeyValueObservingOptions.New, IntPtr.Zero);
			//_player.AddObserver(this, (Foundation.NSString)"error", Foundation.NSKeyValueObservingOptions.Initial | Foundation.NSKeyValueObservingOptions.New, IntPtr.Zero);

			_playerController.Player = _player;
			if (_playerController.Player?.Error != null)
			{
				System.Diagnostics.Debug.WriteLine(_playerController.Player.Error.LocalizedDescription);

				Element.OnMediaErrorOccurred(_playerController?.Player?.Error?.LocalizedDescription);

				throw new InvalidOperationException(_playerController?.Player?.Error?.LocalizedDescription);
			}
		}
		#endregion Methods: Private


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
