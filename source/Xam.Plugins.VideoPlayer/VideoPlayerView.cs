using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// VideoPlayerView is a View which contains a MediaElement to play a video.
	/// </summary>
	public class VideoPlayerView : View
	{
		#region Commands
		// Play
		public static readonly BindableProperty PlayCommandProperty = BindableProperty.Create(nameof(PlayCommand), typeof(ICommand), typeof(VideoPlayerView), null);
		public ICommand PlayCommand { get { return (ICommand)GetValue(PlayCommandProperty); } internal set { SetValue(PlayCommandProperty, value); } }
		// Pause
		public static readonly BindableProperty PauseCommandProperty = BindableProperty.Create(nameof(PauseCommand), typeof(ICommand), typeof(VideoPlayerView), null);
		public ICommand PauseCommand { get { return (ICommand)GetValue(PauseCommandProperty); } internal set { SetValue(PauseCommandProperty, value); } }
		// Stop
		public static readonly BindableProperty StopCommandProperty = BindableProperty.Create(nameof(StopCommand), typeof(ICommand), typeof(VideoPlayerView), null);
		public ICommand StopCommand { get { return (ICommand)GetValue(StopCommandProperty); } internal set { SetValue(StopCommandProperty, value); } }
		// Seek
		public static readonly BindableProperty SeekCommandProperty = BindableProperty.Create(nameof(SeekCommand), typeof(ICommand), typeof(VideoPlayerView), null);
		public ICommand SeekCommand { get { return (ICommand)GetValue(SeekCommandProperty); } internal set { SetValue(SeekCommandProperty, value); } }
		// Mute
		public static readonly BindableProperty MuteCommandProperty = BindableProperty.Create(nameof(MuteCommand), typeof(ICommand), typeof(VideoPlayerView), null);
		public ICommand MuteCommand { get { return (ICommand) GetValue(MuteCommandProperty); } internal set {  SetValue(MuteCommandProperty, value); }}
		#endregion Commands

		#region Properties
		#region Property: VideoSource
		public static readonly BindableProperty VideoSourceProperty =
			BindableProperty.Create(propertyName: nameof(VideoSource),
									returnType: typeof(string),
									declaringType: typeof(VideoPlayerView),
									defaultValue: default(string),
									defaultBindingMode: BindingMode.Default
#if DEBUG
									, validateValue: (bindable, value) =>
									{
										// validate the new value
										System.Diagnostics.Debug.WriteLine("VideoPlayerView: VideoSource [Value: {0}]", value);

										return true;
									}
									, propertyChanged: (bindable, oldvalue, newvalue) =>
									{
										//property changed
										System.Diagnostics.Debug.WriteLine("VideoPlayerView: VideoSource [OldValue: {0}] [NewValue: {1}]", oldvalue, newvalue);
									}
									, propertyChanging: (bindable, oldvalue, newvalue) =>
									{
										//property changing
										System.Diagnostics.Debug.WriteLine("VideoPlayerView: VideoSource [OldValue: {0}] [NewValue: {1}]", oldvalue, newvalue);
									}
									, coerceValue: (bindable, value) =>
									{
										//coerce value
										System.Diagnostics.Debug.WriteLine("VideoPlayerView: VideoSource [Value: {0}]", value);
										return value;
									}
									, defaultValueCreator: (bindable) =>
									{
										//default constructor
										return default(string);
									}
#endif
				);

		public string VideoSource
		{
			get { return (string)GetValue(VideoSourceProperty); }
			set { SetValue(VideoSourceProperty, value); }
		}
		#endregion Property: VideoSource

		#region Property: VolumeLevel
		public static readonly BindableProperty VolumeLevelProperty =
			BindableProperty.Create(propertyName: nameof(VolumeLevel), returnType: typeof(double),
				declaringType: typeof(VideoPlayerView), defaultValue: default(double), defaultBindingMode: BindingMode.Default);

		public double VolumeLevel
		{
			get { return (double)GetValue(VolumeLevelProperty); }
			set { SetValue(VolumeLevelProperty, value); }
		}
		#endregion Property: VolumeLevel

		#region Property: AutoPlay
		public static readonly BindableProperty AutoPlayProperty =
			BindableProperty.Create(propertyName: nameof(AutoPlay), returnType: typeof(bool),
				declaringType: typeof(VideoPlayerView), defaultValue: default(bool), defaultBindingMode: BindingMode.Default);

		public bool AutoPlay
		{
			get { return (bool)GetValue(AutoPlayProperty); }
			set { SetValue(AutoPlayProperty, value); }
		}
		#endregion Property: AutoPlay

		#region Property: AreControlsDisplayed
		public static readonly BindableProperty AreControlsDisplayedProperty =
			BindableProperty.Create(propertyName: nameof(AreControlsDisplayed), returnType: typeof(bool),
				declaringType: typeof(VideoPlayerView), defaultValue: default(bool), defaultBindingMode: BindingMode.Default);

		public bool AreControlsDisplayed
		{
			get { return (bool)GetValue(AreControlsDisplayedProperty); }
			set { SetValue(AreControlsDisplayedProperty, value); }
		}
		#endregion Property: AreControlsDisplayed

		#region Property: IsMuted
		public static readonly BindableProperty IsMutedProperty =
			BindableProperty.Create(propertyName: nameof(IsMuted), returnType: typeof(bool),
				declaringType: typeof(VideoPlayerView), defaultValue: default(bool), defaultBindingMode: BindingMode.Default);

		public bool IsMuted
		{
			get { return (bool)GetValue(IsMutedProperty); }
			set { SetValue(IsMutedProperty, value); }
		}
		#endregion Property: IsMuted

		#region Property: MediaState
		public static readonly BindableProperty MediaStateProperty =
			BindableProperty.Create(propertyName: nameof(MediaState), returnType: typeof(MediaState),
				declaringType: typeof(VideoPlayerView), defaultValue: default(MediaState), defaultBindingMode: BindingMode.Default);

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
				declaringType: typeof(VideoPlayerView), defaultValue: 1.77d, defaultBindingMode: BindingMode.Default);

		public double VideoScale
		{
			get { return (double)GetValue(VideoScaleProperty); }
			set { SetValue(VideoScaleProperty, value); }
		}
		#endregion Property: VideoScale

		#region Property: Position
		public static readonly BindableProperty PositionProperty =
			BindableProperty.Create(propertyName: nameof(Position), returnType: typeof(TimeSpan),
				declaringType: typeof(VideoPlayerView), defaultValue: new TimeSpan(), defaultBindingMode: BindingMode.Default);

		public TimeSpan Position
		{
			get { return (TimeSpan)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); Seek(value); }
		}
		#endregion Property: Position

		#endregion Properties

		#region Events Handlers
		public event EventHandler<MediaProgressEventArgs> MediaStateChanged;
		public event EventHandler<MediaErrorEventArgs> MediaError;
		#endregion Events Handlers

		#region Methods

		public void Play()
		{
		    if (PlayCommand?.CanExecute(null) == true)
		    {
		        PlayCommand?.Execute(null);

		        MediaState = MediaState.Playing;
		        OnMediaStateChanged();
		    }
		}

		public void Stop()
		{
		    if (StopCommand?.CanExecute(null) == true)
		    {
		        StopCommand?.Execute(null);

		        MediaState = MediaState.Stopped;
		        OnMediaStateChanged();
		    }
		}

		public void Pause()
		{
		    if (PauseCommand?.CanExecute(null) == true)
		    {
		        PauseCommand?.Execute(null);

		        MediaState = MediaState == MediaState.Paused ? MediaState.Playing : MediaState.Paused;

		        OnMediaStateChanged();
		    }
		}

		public void Seek(TimeSpan timeSpan)
		{
		    if (SeekCommand?.CanExecute(timeSpan) == true)
		    {
		        SeekCommand?.Execute(timeSpan);

		        MediaState = MediaState.Seeking;
		        OnMediaStateChanged();
		    }
		}

		public void Seek(int seconds)
		{
			Seek(new TimeSpan(0, 0, 0, seconds));
		}

		public void Mute(bool enabled)
		{
			if (MuteCommand?.CanExecute(enabled) == true)
			{
				MuteCommand?.Execute(enabled);
			}
		}
		#endregion Methods

		#region Events
		internal void OnMediaLoaded()
		{
			MediaState = MediaState.Loaded;

			OnMediaStateChanged();
		}

		internal void OnMediaStarted()
		{
			MediaState = MediaState.Playing;

			OnMediaStateChanged();
		}

		internal void OnMediaCompleted()
		{
			MediaState = MediaState.Finished;

			OnMediaStateChanged();
		}

		internal void OnMediaErrorOccurred(string errorMessaige, Exception ex = null)
		{
			MediaState = MediaState.Error;

			MediaError?.    Invoke(this, new MediaErrorEventArgs { ErrorMessage = errorMessaige, ErrorObject = ex, OriginalSource = VideoSource });
		}

		internal void OnMediaStateChanged()
		{
			MediaStateChanged?.Invoke(this, new MediaProgressEventArgs { State = MediaState });

			((Command)PlayCommand).ChangeCanExecute();
			((Command)PauseCommand).ChangeCanExecute();
			((Command)SeekCommand).ChangeCanExecute();
			((Command)StopCommand).ChangeCanExecute();
			((Command)MuteCommand).ChangeCanExecute();
		}

		internal void OnMediaPositionChanged(TimeSpan ts)
	    {
	        Position = ts;

	    }
		#endregion Events
	}

	#region Enums

	public enum MediaState
	{
		Unknown,
		Stopped,
		Starting,
		Playing,
		Seeking,
		Paused,
		Loaded,
		Finished,
		Error
	}
	#endregion Enums

	#region Event Arguments

	#endregion Event Arguments
}
