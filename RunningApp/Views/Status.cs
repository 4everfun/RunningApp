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

            int startX = 50;

            if (this.tracker.GetOrInstantiateTrackModel().ID != null)
            {
                c.DrawText("U bekijkt nu: \"" + this.tracker.GetOrInstantiateTrackModel().Name + "\"", 10, startX, p);
                startX += 50;
            }

            c.DrawText("U loopt al: ", 10, startX, p);
            c.DrawText(this.tracker.GetTimeSpanTracking().ToString(@"hh\:mm\:ss"), 10, startX + 50, p);

            p.TextAlign = Paint.Align.Center;

            c.DrawText("U heeft al ", this.Width / 2, startX, p);
            c.DrawText((int)this.tracker.track.GetTotalRunningDistance() + "m gelopen", this.Width/2, startX + 50, p);

            p.TextAlign = Paint.Align.Right;

            c.DrawText("Dat is gemiddeld", this.Width - 10, startX, p);
            c.DrawText((int)this.tracker.track.GetAvergageSpeed() + "km/h", this.Width - 10, startX + 50, p);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int Height = 130;

            if (this.tracker.GetOrInstantiateTrackModel().ID != null) Height += 50;

            this.SetMeasuredDimension(widthMeasureSpec, Height);
        }
    }
}