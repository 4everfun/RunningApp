using System;
using System.Collections.Generic;
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
using Android.Locations;

using Kaart;

namespace RunningApp.Views
{
    public class MapView : View, ILocationListener, ScaleGestureDetector.IOnScaleGestureListener
    {
        private Bitmap Map;
        private PointF Position;
        private float CenterXOffset = 0;
        private float CenterYOffset = 0;
        private float XYRotation = 0;
        private float Scale = 0;

        public MapView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public MapView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context c)
        {
            BitmapFactory.Options MapOptions = new BitmapFactory.Options();
            MapOptions.InScaled = false;

            Map = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Map, MapOptions);

            this.Scale = Math.Min(this.Map.Width / this.Width, this.Map.Height / this.Height);

            LocationManager LocationManager = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria LocationCriteria = new Criteria();
            LocationCriteria.Accuracy = Accuracy.High;
            string LocationProvider = LocationManager.GetBestProvider(LocationCriteria, true);
            LocationManager.RequestLocationUpdates(LocationProvider, 0, 0, this);
        }

        // Location

        public void OnLocationChanged(Location Location)
        {
            this.Position = Projectie.Geo2RD(Location);

            this.Invalidate();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        // Scale

        public bool OnScale(ScaleGestureDetector detector)
        {
            this.Scale = detector.ScaleFactor;
            throw new NotImplementedException();
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            throw new NotImplementedException();
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            throw new NotImplementedException();
        }

        protected override void OnDraw(Canvas c)
        {
            Matrix Mat = new Matrix();

            float XOffset = this.Map.Width / 2 + this.CenterXOffset;
            float YOffset = this.Map.Height/ 2 + this.CenterYOffset;

            this.DrawMap(c, Mat);
            this.DrawCurrentLocation(c, Mat);

            Mat.PostRotate(this.XYRotation);
            Mat.PostTranslate(XOffset, YOffset);

            // Center the map on the screen
            Mat.PostScale(this.Scale, this.Scale);
            Mat.PostTranslate(this.Width / 2, this.Height / 2);
        }

        private void DrawMap(Canvas c, Matrix m)
        {
            c.DrawBitmap(this.Map, m, new Paint());
        }

        private void DrawCurrentLocation(Canvas c, Matrix m)
        {
            //c.DrawCircle;
        }
    }
}