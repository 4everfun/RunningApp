using Android.App;
using Android.Widget;
using Android.OS;

namespace RunningApp
{
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //this.SetContentView(new Views.MapView(this));

            this.SetContentView(Resource.Layout.Main);
        }
    }
}

