using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xam.Plugins.VideoPlayer.Sample;

namespace Xam.Plugins.VideoPlayer.Sample.Droid
{
	[Activity(Label = "Xam.Plugins.VideoPlayer.Sample", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// subscribe to app wide unhandled exceptions so that we can log them.
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => { ExceptionHandler(args.ExceptionObject as Exception);};
			AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) => { ExceptionHandler(args.Exception); };

			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App());
		}

		/// <summary>
		/// When app-wide unhandled exceptions are hit, this will handle them. Be aware however, that typically
		/// android will be destroying the process, so there's not a lot you can do on the android side of things,
		/// but your xamarin code should still be able to work. so if you have a custom err logging manager or
		/// something, you can call that here. You _won't_ be able to call Android.Util.Log, because Dalvik
		/// will destroy the java side of the process.
		/// </summary>
		protected void ExceptionHandler(Exception e)
		{
			// log won't be available, because dalvik is destroying the process
			//Log.Debug (logTag, "MyHandler caught : " + e.Message);
			// instead, your err handling code shoudl be run:
			Console.WriteLine("========= MyHandler caught : " + e.Message);
		}
	}
}

