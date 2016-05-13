using System;

namespace Xam.Plugins.VideoPlayer
{
    public class MediaProgressEventArgs : EventArgs
    {
        public MediaState State { get; set; }
    }
}