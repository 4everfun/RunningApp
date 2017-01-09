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

namespace RunningApp.Tracker
{
    public class TrackUpdatedEventArgs : EventArgs
    {
        public Track Track { get; protected set; }

        public TrackUpdatedEventArgs(Track track)
        {
            this.Track = track;
        }
    }
}