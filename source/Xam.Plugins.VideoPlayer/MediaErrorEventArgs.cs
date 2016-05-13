using System;

namespace Xam.Plugins.VideoPlayer
{
    public class MediaErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
        public Exception ErrorObject { get; set; }
        public string OriginalSource { get; set; }
    }
}