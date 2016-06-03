// ***********************************************************************
// Assembly         : Xam.Plugins.VideoPlayer
// Author           : raven
// Created          : 06-03-2016
//
// Last Modified By : raven
// Last Modified On : 06-03-2016
// ***********************************************************************
// <copyright file="IVideoPlayer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Input;

namespace Xam.Plugins.VideoPlayer
{
	/// <summary>
	/// Interface IVideoPlayer
	/// </summary>
	internal interface IVideoPlayer
	{
		#region Commands
		/// <summary>
		/// Gets the play command.
		/// </summary>
		/// <value>The play command.</value>
		ICommand PlayCommand { get; }
		/// <summary>
		/// Gets the pause command.
		/// </summary>
		/// <value>The pause command.</value>
		ICommand PauseCommand { get; }
		/// <summary>
		/// Gets the stop command.
		/// </summary>
		/// <value>The stop command.</value>
		ICommand StopCommand { get; }
		/// <summary>
		/// Gets the seek command.
		/// </summary>
		/// <value>The seek command.</value>
		ICommand SeekCommand { get; }
		/// <summary>
		/// Gets the mute command.
		/// </summary>
		/// <value>The mute command.</value>
		ICommand MuteCommand { get; }
		#endregion Commands

		#region Properties
		/// <summary>
		/// Gets or sets the video source.
		/// </summary>
		/// <value>The video source.</value>
		string VideoSource { get; set; }
		/// <summary>
		/// Gets or sets the volume level.
		/// </summary>
		/// <value>The volume level.</value>
		double VolumeLevel { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [automatic play].
		/// </summary>
		/// <value><c>true</c> if [automatic play]; otherwise, <c>false</c>.</value>
		bool AutoPlay { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [are controls displayed].
		/// </summary>
		/// <value><c>true</c> if [are controls displayed]; otherwise, <c>false</c>.</value>
		bool AreControlsDisplayed { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is muted.
		/// </summary>
		/// <value><c>true</c> if this instance is muted; otherwise, <c>false</c>.</value>
		bool IsMuted { get; set; }
		/// <summary>
		/// Gets or sets the state of the media.
		/// </summary>
		/// <value>The state of the media.</value>
		MediaState MediaState { get; set; }
		/// <summary>
		/// Gets or sets the video scale.
		/// </summary>
		/// <value>The video scale.</value>
		double VideoScale { get; set; }
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>The position.</value>
		TimeSpan Position { get; set; }
		/// <summary>
		/// Gets or sets the error message.
		/// </summary>
		/// <value>The error message.</value>
		string ErrorMessage { get; set; }
		#endregion Properties

		#region Events
		/// <summary>
		/// Occurs when [media state changed].
		/// </summary>
		event EventHandler<MediaProgressEventArgs> MediaStateChanged;
		/// <summary>
		/// Occurs when [media error].
		/// </summary>
		event EventHandler<MediaErrorEventArgs> MediaError;
		#endregion Events

		#region Methods
		/// <summary>
		/// Plays this instance.
		/// </summary>
		void Play();
		/// <summary>
		/// Stops this instance.
		/// </summary>
		void Stop();
		/// <summary>
		/// Pauses this instance.
		/// </summary>
		void Pause();
		/// <summary>
		/// Seeks the specified time span.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		void Seek(TimeSpan timeSpan);
		/// <summary>
		/// Seeks the specified seconds.
		/// </summary>
		/// <param name="seconds">The seconds.</param>
		void Seek(int seconds);
		/// <summary>
		/// Mutes the specified enabled.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		void Mute(bool enabled);
		#endregion Methods
	}
}