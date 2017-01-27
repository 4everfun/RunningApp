using System;
using System.Timers;

using Android.App;
using Android.Widget;
using Android.OS;

using RunningApp.Views;
using RunningApp.Tracker;
using RunningApp.Exceptions;

using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;
using RunningApp.Fragments;

namespace RunningApp
{
    // Remove the ActionBar
    [Activity(Label = "RunningApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : AppCompatActivity
    {
        public Tracker.Tracker Tracker;
        protected DrawerLayout drawerLayout;
        protected ListView drawerList;
        protected FrameLayout content;

        protected const int TRACKER = 0;
        protected const int MYTRACKS = 1;
        protected const int ANALYSE = 2;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the content view to the main XML file 
            this.SetContentView(Resource.Layout.Main);

            string[] ListItems = new string[2];
            ListItems[0] = "Tracker";
            ListItems[1] = "Mijn Tracks";

            this.drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.drawerList = FindViewById<ListView>(Resource.Id.nav_view);
            this.drawerList.Adapter = new ArrayAdapter<String>(this, Resource.Layout.drawer_list_item, ListItems);
            this.drawerList.ItemClick += this.OpenItem;

            this.Tracker = new Tracker.Tracker(this);

            this.SwitchView(MainActivity.TRACKER);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SwitchView(int item)
        {
            Fragment f;
            switch (item)
            {
                case 0:
                    f = new TrackerFragment();
                    break;
                case 1:
                    f = new MyTracksFragment();
                    break;
                case 2:
                    f = new TrackerFragment();
                    break;
                default:
                    return;
            }
            FragmentTransaction ft = this.FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.content, f);
            ft.Commit();
        }

        private void OpenItem(object sender, AdapterView.ItemClickEventArgs ea)
        {
            switch (ea.Id)
            {
                case 0:
                    this.SwitchView(MainActivity.TRACKER);
                    break;
                case 1:
                    this.SwitchView(MainActivity.MYTRACKS);
                    break;
                default:
                    return;
            }
            this.drawerLayout.CloseDrawer(this.drawerList);
        }
    }
}

