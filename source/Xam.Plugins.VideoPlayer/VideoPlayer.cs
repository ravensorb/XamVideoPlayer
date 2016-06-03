// ***********************************************************************
// Assembly         : Xam.Plugins.VideoPlayer
// Author           : raven
// Created          : 05-12-2016
//
// Last Modified By : raven
// Last Modified On : 06-03-2016
// ***********************************************************************
// <copyright file="VideoPlayer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayer is a View which contains a MediaElement to play a video.
	/// </summary>
	/// <seealso cref="Xamarin.Forms.View" />
	/// <seealso cref="IVideoPlayer" />
	public class VideoPlayer : View, IVideoPlayer
	{
		#region Commands
		// Play
		/// <summary>
		/// The play command property
		/// </summary>
		public static readonly BindableProperty PlayCommandProperty = BindableProperty.Create(nameof(PlayCommand), typeof(ICommand), typeof(VideoPlayer));
		/// <summary>
		/// Gets the play command.
		/// </summary>
		/// <value>The play command.</value>
		public ICommand PlayCommand { get { return (ICommand)GetValue(PlayCommandProperty); } internal set { SetValue(PlayCommandProperty, value); } }
		// Pause
		/// <summary>
		/// The pause command property
		/// </summary>
		public static readonly BindableProperty PauseCommandProperty = BindableProperty.Create(nameof(PauseCommand), typeof(ICommand), typeof(VideoPlayer));
		/// <summary>
		/// Gets the pause command.
		/// </summary>
		/// <value>The pause command.</value>
		public ICommand PauseCommand { get { return (ICommand)GetValue(PauseCommandProperty); } internal set { SetValue(PauseCommandProperty, value); } }
		// Stop
		/// <summary>
		/// The stop command property
		/// </summary>
		public static readonly BindableProperty StopCommandProperty = BindableProperty.Create(nameof(StopCommand), typeof(ICommand), typeof(VideoPlayer));
		/// <summary>
		/// Gets the stop command.
		/// </summary>
		/// <value>The stop command.</value>
		public ICommand StopCommand { get { return (ICommand)GetValue(StopCommandProperty); } internal set { SetValue(StopCommandProperty, value); } }
		// Seek
		/// <summary>
		/// The seek command property
		/// </summary>
		public static readonly BindableProperty SeekCommandProperty = BindableProperty.Create(nameof(SeekCommand), typeof(ICommand), typeof(VideoPlayer));
		/// <summary>
		/// Gets the seek command.
		/// </summary>
		/// <value>The seek command.</value>
		public ICommand SeekCommand { get { return (ICommand)GetValue(SeekCommandProperty); } internal set { SetValue(SeekCommandProperty, value); } }
		// Mute
		/// <summary>
		/// The mute command property
		/// </summary>
		public static readonly BindableProperty MuteCommandProperty = BindableProperty.Create(nameof(MuteCommand), typeof(ICommand), typeof(VideoPlayer));
		/// <summary>
		/// Gets the mute command.
		/// </summary>
		/// <value>The mute command.</value>
		public ICommand MuteCommand { get { return (ICommand) GetValue(MuteCommandProperty); } internal set {  SetValue(MuteCommandProperty, value); }}
		#endregion Commands

		#region Properties
		#region Property: VideoSource
		/// <summary>
		/// The video source property
		/// </summary>
		public static readonly BindableProperty VideoSourceProperty =
			BindableProperty.Create(propertyName: nameof(VideoSource),
									returnType: typeof(string),
									declaringType: typeof(VideoPlayer),
									defaultValue: default(string),
									defaultBindingMode: BindingMode.Default
#if DEBUG
									, validateValue: (bindable, value) =>
									{
										// validate the new value
										System.Diagnostics.Debug.WriteLine("VideoPlayer: VideoSource [Value: {0}]", value);

										return true;
									}
									, propertyChanged: (bindable, oldvalue, newvalue) =>
									{
										//property changed
										System.Diagnostics.Debug.WriteLine("VideoPlayer: VideoSource [OldValue: {0}] [NewValue: {1}]", oldvalue, newvalue);
									}
									, propertyChanging: (bindable, oldvalue, newvalue) =>
									{
										//property changing
										System.Diagnostics.Debug.WriteLine("VideoPlayer: VideoSource [OldValue: {0}] [NewValue: {1}]", oldvalue, newvalue);
									}
									, coerceValue: (bindable, value) =>
									{
										//coerce value
										System.Diagnostics.Debug.WriteLine("VideoPlayer: VideoSource [Value: {0}]", value);
										return value;
									}
									, defaultValueCreator: (bindable) =>
									{
										//default constructor
										return default(string);
									}
#endif
				);

		/// <summary>
		/// Gets or sets the video source.
		/// </summary>
		/// <value>The video source.</value>
		public string VideoSource
		{
			get { return (string)GetValue(VideoSourceProperty); }
			set { SetValue(VideoSourceProperty, value); }
		}
		#endregion Property: VideoSource

		#region Property: VolumeLevel
		/// <summary>
		/// The volume level property
		/// </summary>
		public static readonly BindableProperty VolumeLevelProperty =
			BindableProperty.Create(propertyName: nameof(VolumeLevel), returnType: typeof(double),
				declaringType: typeof(VideoPlayer), defaultValue: default(double), defaultBindingMode: BindingMode.Default, defaultValueCreator: (o) => 100d);

		/// <summary>
		/// Gets or sets the volume level.
		/// </summary>
		/// <value>The volume level.</value>
		public double VolumeLevel
		{
			get { return (double)GetValue(VolumeLevelProperty); }
			set { SetValue(VolumeLevelProperty, value); }
		}
		#endregion Property: VolumeLevel

		#region Property: AutoPlay
		/// <summary>
		/// The automatic play property
		/// </summary>
		public static readonly BindableProperty AutoPlayProperty =
			BindableProperty.Create(propertyName: nameof(AutoPlay), returnType: typeof(bool),
				declaringType: typeof(VideoPlayer), defaultValue: default(bool), defaultBindingMode: BindingMode.Default, defaultValueCreator: (o) => false);

		/// <summary>
		/// Gets or sets a value indicating whether [automatic play].
		/// </summary>
		/// <value><c>true</c> if [automatic play]; otherwise, <c>false</c>.</value>
		public bool AutoPlay
		{
			get { return (bool)GetValue(AutoPlayProperty); }
			set { SetValue(AutoPlayProperty, value); }
		}
		#endregion Property: AutoPlay

		#region Property: AreControlsDisplayed
		/// <summary>
		/// The are controls displayed property
		/// </summary>
		public static readonly BindableProperty AreControlsDisplayedProperty =
			BindableProperty.Create(propertyName: nameof(AreControlsDisplayed), returnType: typeof(bool),
				declaringType: typeof(VideoPlayer), defaultValue: default(bool), defaultBindingMode: BindingMode.Default, defaultValueCreator: (o) => true);

		/// <summary>
		/// Gets or sets a value indicating whether [are controls displayed].
		/// </summary>
		/// <value><c>true</c> if [are controls displayed]; otherwise, <c>false</c>.</value>
		public bool AreControlsDisplayed
		{
			get { return (bool)GetValue(AreControlsDisplayedProperty); }
			set { SetValue(AreControlsDisplayedProperty, value); }
		}
		#endregion Property: AreControlsDisplayed

		#region Property: IsMuted
		/// <summary>
		/// The is muted property
		/// </summary>
		public static readonly BindableProperty IsMutedProperty =
			BindableProperty.Create(propertyName: nameof(IsMuted), returnType: typeof(bool),
				declaringType: typeof(VideoPlayer), defaultValue: default(bool), defaultBindingMode: BindingMode.Default, defaultValueCreator: (o) => false);

		/// <summary>
		/// Gets or sets a value indicating whether this instance is muted.
		/// </summary>
		/// <value><c>true</c> if this instance is muted; otherwise, <c>false</c>.</value>
		public bool IsMuted
		{
			get { return (bool)GetValue(IsMutedProperty); }
			set { SetValue(IsMutedProperty, value); }
		}
		#endregion Property: IsMuted

		#region Property: MediaState
		/// <summary>
		/// The media state property
		/// </summary>
		public static readonly BindableProperty MediaStateProperty =
			BindableProperty.Create(propertyName: nameof(MediaState), returnType: typeof(MediaState),
				declaringType: typeof(VideoPlayer), defaultValue: default(MediaState), defaultBindingMode: BindingMode.OneWayToSource);

		/// <summary>
		/// Gets or sets the state of the media.
		/// </summary>
		/// <value>The state of the media.</value>
		public MediaState MediaState
		{
			get { return (MediaState)GetValue(MediaStateProperty); }
			set { SetValue(MediaStateProperty, value); }
		}
		#endregion Property: MediaState

		#region Property: VideoScale
		/// <summary>
		/// The scale format of the video which is in most cases 16:9 (1.77) or 4:3 (1.33).
		/// </summary>
		public static readonly BindableProperty VideoScaleProperty =
			BindableProperty.Create(propertyName: nameof(VideoScale), returnType: typeof(double),
				declaringType: typeof(VideoPlayer), defaultValue: 1.77d, defaultBindingMode: BindingMode.Default);

		/// <summary>
		/// Gets or sets the video scale.
		/// </summary>
		/// <value>The video scale.</value>
		public double VideoScale
		{
			get { return (double)GetValue(VideoScaleProperty); }
			set { SetValue(VideoScaleProperty, value); }
		}
		#endregion Property: VideoScale

		#region Property: Position
		/// <summary>
		/// The position property
		/// </summary>
		public static readonly BindableProperty PositionProperty =
			BindableProperty.Create(propertyName: nameof(Position), returnType: typeof(TimeSpan),
				declaringType: typeof(VideoPlayer), defaultValue: new TimeSpan(), defaultBindingMode: BindingMode.OneWayToSource);

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>The position.</value>
		public TimeSpan Position
		{
			get { return (TimeSpan)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); Seek(value); }
		}
		#endregion Property: Position

		#region Property: Error Message
		/// <summary>
		/// The video source property
		/// </summary>
		public static readonly BindableProperty ErrorMessageProperty =
			BindableProperty.Create(propertyName: nameof(ErrorMessage),
									returnType: typeof(string),
									declaringType: typeof(VideoPlayer),
									defaultValue: default(string),
									defaultBindingMode: BindingMode.OneWayToSource
				);

		public string ErrorMessage
		{
			get { return (string)GetValue(ErrorMessageProperty); }
			set { SetValue(ErrorMessageProperty, value); }
		}
		#endregion Property: Error Message
		#endregion Properties

		#region Events Handlers
		/// <summary>
		/// Occurs when [media state changed].
		/// </summary>
		public event EventHandler<MediaProgressEventArgs> MediaStateChanged;
		/// <summary>
		/// Occurs when [media error].
		/// </summary>
		public event EventHandler<MediaErrorEventArgs> MediaError;
		#endregion Events Handlers

		#region Methods

		/// <summary>
		/// Plays this instance.
		/// </summary>
		public void Play()
		{
		    if (PlayCommand?.CanExecute(null) == true)
		    {
		        PlayCommand?.Execute(null);

				MediaState = MediaState.Playing;
		        OnMediaStateChanged();
		    }
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
		    if (StopCommand?.CanExecute(null) == true)
		    {
		        StopCommand?.Execute(null);

		        MediaState = MediaState.Stopped;
		        OnMediaStateChanged();
		    }
		}

		/// <summary>
		/// Pauses this instance.
		/// </summary>
		public void Pause()
		{
		    if (PauseCommand?.CanExecute(null) == true)
		    {
		        PauseCommand?.Execute(null);

		        MediaState = MediaState == MediaState.Paused ? MediaState.Playing : MediaState.Paused;

		        OnMediaStateChanged();
		    }
		}

		/// <summary>
		/// Seeks the specified time span.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		public void Seek(TimeSpan timeSpan)
		{
		    if (SeekCommand?.CanExecute(timeSpan) == true)
		    {
		        SeekCommand?.Execute(timeSpan);

		        MediaState = MediaState.Seeking;
		        OnMediaStateChanged();
		    }
		}

		/// <summary>
		/// Seeks the specified seconds.
		/// </summary>
		/// <param name="seconds">The seconds.</param>
		public void Seek(int seconds)
		{
			Seek(new TimeSpan(0, 0, 0, seconds));
		}

		/// <summary>
		/// Mutes the specified enabled.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		public void Mute(bool enabled)
		{
			if (MuteCommand?.CanExecute(enabled) == true)
			{
				MuteCommand?.Execute(enabled);
			}
		}
		#endregion Methods

		#region Events
		/// <summary>
		/// Called when [media loaded].
		/// </summary>
		internal void OnMediaLoaded()
		{
			MediaState = MediaState.Loaded;

			OnMediaStateChanged();
		}

		/// <summary>
		/// Called when [media started].
		/// </summary>
		internal void OnMediaStarted()
		{
			MediaState = MediaState.Playing;

			OnMediaStateChanged();
		}

		/// <summary>
		/// Called when [media completed].
		/// </summary>
		internal void OnMediaCompleted()
		{
			MediaState = MediaState.Finished;

			OnMediaStateChanged();
		}

		/// <summary>
		/// Called when [media error occurred].
		/// </summary>
		/// <param name="errorMessage">The error messaige.</param>
		/// <param name="ex">The ex.</param>
		internal void OnMediaErrorOccurred(string errorMessage, Exception ex = null)
		{
			MediaState = MediaState.Error;

			ErrorMessage = errorMessage;

			MediaError?.Invoke(this, new MediaErrorEventArgs { ErrorMessage = errorMessage, ErrorObject = ex, OriginalSource = VideoSource });

			OnMediaStateChanged();
		}

		/// <summary>
		/// Called when [media position changed].
		/// </summary>
		/// <param name="ts">The ts.</param>
		internal void OnMediaPositionChanged(TimeSpan ts)
		{
			var ms = MediaState;

			MediaState = MediaState.Seeking;
			OnMediaStateChanged();

			Position = ts;

			MediaState = ms;
			OnMediaStateChanged();
		}

		/// <summary>
		/// Called when [media state changed].
		/// </summary>
		internal void OnMediaStateChanged()
		{
			if (MediaState != MediaState.Error)
			{
				ErrorMessage = string.Empty;
			}

			MediaStateChanged?.Invoke(this, new MediaProgressEventArgs { State = MediaState });

			UdpdateCommands();
		}
		#endregion Events

		#region Private Methods

		/// <summary>
		/// Udpdates the commands.
		/// </summary>
		private void UdpdateCommands()
		{
			((Command)PlayCommand).ChangeCanExecute();
			((Command)PauseCommand).ChangeCanExecute();
			((Command)SeekCommand).ChangeCanExecute();
			((Command)StopCommand).ChangeCanExecute();
			((Command)MuteCommand).ChangeCanExecute();
		}
		#endregion Private Methods
	}

	#region Enums

	/// <summary>
	/// Enum MediaState
	/// </summary>
	public enum MediaState
	{
		/// <summary>
		/// The unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// The stopped
		/// </summary>
		Stopped,
		/// <summary>
		/// The starting
		/// </summary>
		Starting,
		/// <summary>
		/// The playing
		/// </summary>
		Playing,
		/// <summary>
		/// The seeking
		/// </summary>
		Seeking,
		/// <summary>
		/// The paused
		/// </summary>
		Paused,
		/// <summary>
		/// The loaded
		/// </summary>
		Loaded,
		/// <summary>
		/// The finished
		/// </summary>
		Finished,
		/// <summary>
		/// The error
		/// </summary>
		Error
	}
	#endregion Enums

	#region Event Arguments

	#endregion Event Arguments
}
