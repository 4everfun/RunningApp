using Android.App;
using Android.Widget;
using Android.OS;

namespace RunningApp
{
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.SetContentView(Resource.Layout.Main);
        }
    }
}

