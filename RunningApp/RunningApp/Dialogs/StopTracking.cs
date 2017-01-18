using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RunningApp.Dialogs
{
    class StopTracking : DialogFragment
    {
        public static StopTracking NewInstance(Bundle bundle)
        {
            StopTracking fragment = new StopTracking();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.StopTracking, container, false);
            Button button = view.FindViewById<Button>(Resource.Id.CloseButton);
            button.Click += delegate {
                Dismiss();
                Toast.MakeText(Activity, "Dialog fragment dismissed!", ToastLength.Short).Show();
            };

            return view;
        }
    }
}