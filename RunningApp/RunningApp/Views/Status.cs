using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using RunningApp.Tracker;

namespace RunningApp.Views
{
    public class Status : View
    {
        protected Tracker.Tracker tracker;

        public Status(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public Status(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            Timer t = new Timer();
            t.Interval = 1000;
            t.Elapsed += this.Update;
            t.Enabled = true;
        }

        public void SetTracker(Tracker.Tracker tracker)
        {
            this.tracker = tracker;
        }

        protected void Update(object sender, EventArgs ea)
        {
            this.Invalidate();
        }

        protected override void OnDraw(Canvas c)
        {
            base.OnDraw(c);

            Paint p = new Paint();
            p.Color = Color.White;
            p.TextSize = 40;

            c.DrawText("U loopt al: ", 10, 50, p);
            c.DrawText(this.tracker.GetTimeSpanTracking().ToString("hh:mm:ss"), 10, 100, p);

            p.TextAlign = Paint.Align.Center;

            c.DrawText("U heeft al ", this.Width / 2, 50, p);
            c.DrawText((int)this.tracker.GetTotalDistance() + "m gelopen", this.Width/2, 100, p);

            p.TextAlign = Paint.Align.Right;

            c.DrawText("Dat is gemiddeld", this.Width - 10, 50, p);
            c.DrawText((int)this.tracker.GetAvergageSpeed() + "km/h", this.Width - 10, 100, p);
        }
    }
}