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
using Android.Text;

namespace RunningApp.Dialogs
{
    class TrackNameDialog : DialogFragment
    {
        public delegate void TrackNameDialogSaveEventHandler(TrackNameDialog sender, TrackNameDialogSaveEventArgs e);
        public event TrackNameDialogSaveEventHandler TrackNameDialogSave;

        private Button btnClose, btnSave;

        public static TrackNameDialog NewInstance(Bundle bundle)
        {
            TrackNameDialog fragment = new TrackNameDialog();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.Dialog.SetTitle("Track Opslaan");

            View view = inflater.Inflate(Resource.Layout.TrackNameDialog, container, false);

            this.btnClose = view.FindViewById<Button>(Resource.Id.btnClose);
            this.btnClose.Click += delegate {
                this.Dismiss();
            };

            this.btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            this.btnSave.Click += (s, ea) => this.OnTrackNameDialogSave(new TrackNameDialogSaveEventArgs(view.FindViewById<EditText>(Resource.Id.etTrackName).Text));

            return view;
        }

        protected virtual void OnTrackNameDialogSave(TrackNameDialogSaveEventArgs e)
        {
            TrackNameDialogSave?.Invoke(this, e);
        }
    }
}