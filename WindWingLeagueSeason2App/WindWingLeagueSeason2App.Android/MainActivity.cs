using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;

namespace WindWingLeagueSeason2App.Droid
{
    [Activity(Label = "WindWingLeagueSeason2App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            UpdateTheme();

            MessagingCenter.Subscribe<object>(this, "UpdateTheme", (sender) => UpdateTheme());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void UpdateTheme()
        {
            if (ThemeManager.darkMode)
            {
                Window.SetStatusBarColor(Android.Graphics.Color.Black);//.Argb(255, 0, 0, 0));
            }
            else
            {
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#3700B3"));
            }
        }
    }
}