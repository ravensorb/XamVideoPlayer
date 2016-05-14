using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xam.Plugins.VideoPlayer.Sample.ViewModels;
using Xam.Plugins.VideoPlayer.Sample.Views;
using Xamarin.Forms;

namespace Xam.Plugins.VideoPlayer.Sample
{
	public class App : Application
	{
		public App()
		{
			// The root page of your application
			try
			{
				MainPage = new Page1 {BindingContext = new VideoPlayerViewModel()};
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
