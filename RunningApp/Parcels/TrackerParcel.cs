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

using RunningApp.Tracker;
using RunningApp.Parcels.Creators;
using Java.Interop;

namespace RunningApp.Parcels
{
    public class TrackerParcel : Java.Lang.Object, IParcelable
    {
        public Tracker.Tracker Tracker { get; set; }

        [ExportField("CREATOR")]
        public static TrackerParcelCreator InitializeCreator()
        {
            return new TrackerParcelCreator();
        }

        public TrackerParcel (Java.Lang.Object Tracker)
        {
            this.Tracker = (Tracker.Tracker)Tracker;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteTypedObject(this.Tracker, ParcelableWriteFlags.None);
        }

        public int DescribeContents()
        {
            return 0;
        }
    }
}