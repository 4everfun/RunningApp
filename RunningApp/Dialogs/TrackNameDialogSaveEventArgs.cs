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
    public class TrackNameDialogSaveEventArgs
    {
        public string TrackName { get; private set; }

        public TrackNameDialogSaveEventArgs(string TrackName)
        {
            this.TrackName = TrackName;
        }
    }
}