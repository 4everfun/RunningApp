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
    public class TrackParcel : Java.Lang.Object, IParcelable
    {
        public Track Track { get; set; }

        [ExportField("CREATOR")]
        public static TrackParcelCreator InitializeCreator()
        {
            return new TrackParcelCreator();
        }

        public TrackParcel (Java.Lang.Object Track)
        {
            this.Track = (Track)Track;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteTypedObject(this.Track, ParcelableWriteFlags.None);
        }

        public int DescribeContents()
        {
            return 0;
        }
    }
}