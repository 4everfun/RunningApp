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
    class StopTrackingDialog : DialogFragment
    {
        public delegate void EventHandler(StopTrackingDialog sender, EventArgs e);

        public event EventHandler OnSaveClick;
        public event EventHandler OnShareClick;
        public event EventHandler OnDeleteClick;

        private Button btnClose, btnSave, btnDelete, btnShare;

        public static StopTrackingDialog NewInstance(Bundle bundle)
        {
            StopTrackingDialog fragment = new StopTrackingDialog();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.StopTrackingDialog, container, false);

            this.btnClose = view.FindViewById<Button>(Resource.Id.btnClose);
            this.btnClose.Click += delegate {
                this.Dismiss();
            };

            this.btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            this.btnSave.Click += delegate (object sender, EventArgs ea) {
                this.OnSaveClick(this, ea);
            };

            this.btnDelete = view.FindViewById<Button>(Resource.Id.btnDelete);
            this.btnDelete.Click += delegate (object sender, EventArgs ea) {
                this.OnDeleteClick(this, ea);
            };

            this.btnShare = view.FindViewById<Button>(Resource.Id.btnShare);
            this.btnShare.Click += delegate (object sender, EventArgs ea) {
                this.OnShareClick(this, ea);
            };

            return view;
        }

        public void BtnSaveClick (object sender, EventArgs ea)
        {
            OnSaveClick(this, ea);
        }
    }
}