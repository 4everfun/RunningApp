using Android.App;
using Android.Widget;
using Android.OS;
using System;

namespace RunningApp
{
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        Views.MapView Map;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //this.SetContentView(new Views.MapView(this));

            this.SetContentView(Resource.Layout.Main);

            Map = FindViewById<Views.MapView>(Resource.Id.mapView);

            FindViewById<Button>(Resource.Id.centerButton).Click += this.CenterMap;
        }

        private void CenterMap(object sender, EventArgs e)
        {
            Map.CenterMap();
        }
    }
}

