using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xam.Plugins.VideoPlayer.Sample.ViewModels
{
	[PropertyChanged.ImplementPropertyChanged]
	public class VideoPlayerViewModel
	{
		public VideoPlayerViewModel()
		{
			Videos = new ObservableCollection<VideoItem>();

			Videos.Add(new VideoItem { Title = "Big Buck Bunny", PlaybackUrl = "http://download.openbricks.org/sample/H264/big_buck_bunny_1080p_H264_AAC_25fps_7200K.MP4" });
		}

		public ICollection<VideoItem> Videos { get; set; }
		public VideoItem SelectedVideo { get; set; }
	}

	[PropertyChanged.ImplementPropertyChanged]
	public class VideoItem
	{
		public string Title { get; set; }
		public string PlaybackUrl { get; set; }
	}
}
