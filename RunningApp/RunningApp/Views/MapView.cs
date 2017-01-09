using System;
using System.Diagnostics;

using Android.Content;
using Android.Util;
using Android.Views;

using Android.Graphics;

using Android.Locations;
using Android.OS;
using Android.Runtime;

using Android.Hardware;

using Kaart;
using RunningApp.Exceptions;
using RunningApp.Tracker;
using System.Collections.Generic;

namespace RunningApp.Views
{
    public class MapView : View, ScaleGestureDetector.IOnScaleGestureListener, ILocationListener, ISensorEventListener
    {
        /// <summary>
        /// In this variable the Map bitmap is stored
        /// </summary>
        private Bitmap Map;

        private const int MapNorthRD = 458000;
        private const int MapEastRD = 142000;
        private const int MapSouthRD = 453000;
        private const int MapWestRD = 136000;

        private const float BitmapScale = 0.4f;

        private ScaleGestureDetector ScaleDetector;

        /// <summary>
        /// This is the Matrix currently used to transform the map
        /// </summary>
        private Matrix CurrentMatrix = new Matrix();

        /// <summary>
        /// The X offset of the map, calculated from the center of the unscaled bitmap from (0, 0)
        /// </summary>
        private float MapOffsetX;

        /// <summary>
        /// The Y offset of the map, calculated from the center of the unscaled bitmap from (0, 0)
        /// </summary>
        private float MapOffsetY;

        /// <summary>
        /// The current scale used to scale the map
        /// </summary>
        private float MapScale;

        /// <summary>
        /// The lower limit for the scale
        /// </summary>
        private float MinScale;

        /// <summary>
        /// The upper limit for the scale
        /// </summary>
        private float MaxScale = 4f;

        /// <summary>
        /// The radius for the location indicator
        /// </summary>
        private float LocationRadius = 25f;

        /// <summary>
        /// The variable where the current RD location of the device can be saved
        /// </summary>
        private PointF CurrentRDLocation;

        private Stopwatch timer = new Stopwatch();

        private Tracker.Tracker tracker;
        private Track Track;

        /// <summary>
        /// The current rotation of the device
        /// </summary>
        private float CurrentLocationRotation;


        public MapView(Context context) :
            base(context)
        {
            Initialize(context);
        }

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
            // Set the background color the white. This is actually redundant, whilst the map can't be zoomed out more than the size of the view. So this background color won't show. We've kept it in as a safeguard.
            this.SetBackgroundColor(Color.White);

            BitmapFactory.Options MapOptions = new BitmapFactory.Options();
            // Disable scaling due to memory limitations
            MapOptions.InScaled = false;
            // Load the bitmap of the map
            this.Map = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Map, MapOptions);

            // Initialize the scale and pinch gestures
            this.ScaleDetector = new ScaleGestureDetector(c, this);
            this.Touch += this.RegisterTouchEvent;

            // Initialize the location listener. Use Fine Accuarcy and receive updates as often as possible.
            LocationManager lm = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria crit = new Criteria();
            crit.Accuracy = Accuracy.Fine;
            string lp = lm.GetBestProvider(crit, true);
            lm.RequestLocationUpdates(lp, 0, 0.5f, this);

            // Initialize the sensor manager for the orientation
            SensorManager sm = (SensorManager)c.GetSystemService(Context.SensorService);
            sm.RegisterListener(this, sm.GetDefaultSensor(SensorType.Orientation), SensorDelay.Ui);

            // Set the default map offset to the center of the map
            this.SetLocation(new PointF((MapView.MapEastRD + MapView.MapWestRD) / 2, (MapView.MapNorthRD + MapView.MapSouthRD) / 2));

            // Set the current Location to the last know location
            try
            {
                this.CurrentRDLocation = Projectie.Geo2RD(lm.GetLastKnownLocation(lp));
                this.CenterMapToCurrentLocation();
            }
            catch (Exception) {}
        }

        public void SetTracker(Tracker.Tracker tracker)
        {
            this.tracker = tracker;
            this.tracker.TrackUpdated += this.UpdateTrackFromTracker;
        }

        public void UpdateTrackFromTracker(object sender, TrackUpdatedEventArgs tuea)
        {
            this.Track = tuea.Track;
            this.Invalidate();
        }

        /// <summary>
        /// Check if the given RD location is on the map
        /// </summary>
        /// <param name="Location">The RD location to check</param>
        /// <returns>True if on map, false if off map</returns>
        private bool IsOnMap(PointF Location)
        {
            if (Location.X < MapView.MapWestRD || Location.X > MapView.MapEastRD || Location.Y < MapView.MapSouthRD || Location.Y > MapView.MapNorthRD) return false;
            return true;
        }

        /// <summary>
        /// Convert RD coordiantes in Bitmap coordinates (scale 1km = 400px)
        /// </summary>
        /// <param name="Location">The RD location to convert into Bitmap coordinates</param>
        /// <returns>Bitmap coordinates</returns>
        protected PointF RD2Bitmap(PointF Location)
        {
            if (!this.IsOnMap(Location)) throw new NotOnMapException();

            PointF BitmapLocation = new PointF();

            // The X location on the Bitmap is the RD location minus the RD location at the start of the bitmap. Then we need to scale it down with the convertion factor of the mapscale
            BitmapLocation.X = (Location.X - MapView.MapWestRD) * MapView.BitmapScale;

            // The Y location on the Bitmap is the RD location at the end of the bitmap minus the RD location. This is because the Y axis is reversed, because this specific piece of map is located at the Northern Hemisphere. Then we need to scale it down with the convertion factor of the mapscale
            BitmapLocation.Y = (MapView.MapNorthRD - Location.Y) * MapView.BitmapScale;

            return BitmapLocation;
        }

        /// <summary>
        /// Set the scale of the map
        /// </summary>
        /// <param name="NewScale">The new scale for the map</param>
        protected void SetScale(float NewScale)
        {
            // Set the scale of the map with consideration of the upper and lower limit.
            this.MapScale = (NewScale < this.MinScale) ? this.MinScale : (NewScale > this.MaxScale) ? this.MaxScale : NewScale;

            // Update the MapOffset (in this way we'll never get white borders around the bitmap)
            this.UpdateMapOffset();
        }

        /// <summary>
        /// Add an offset to the current map offset. This new offset has to be scaled to the current mapscale.
        /// </summary>
        /// <param name="OffsetX">The additional X offset of the scaled Bitmap</param>
        /// <param name="OffsetY">The additional Y offset of the scaled Bitmap</param>
        protected void SetOffset(float OffsetX, float OffsetY)
        {
            // Convert screenpixels to bitmappixels
            float ScaledOffsetX = OffsetX / this.MapScale;
            float ScaledOffsetY = OffsetY / this.MapScale;

            // Set the new map offset
            this.SetMapOffset(this.MapOffsetX + ScaledOffsetX, this.MapOffsetY + ScaledOffsetY);
        }

        /// <summary>
        /// Override the current (bitmap) offset of the map to the given new (bitmap) offset. This new offset cannot be scaled to the current scale. This method checks if the X or Y offset is out of bounds.
        /// </summary>
        /// <param name="OffsetX">The new X offset of the Bitmap</param>
        /// <param name="OffsetY">The new Y offset of the Bitmap</param>
        private void SetMapOffset(float x, float y)
        {
            // Check if the X or the Y is out of bounds. If this is the case, overwrite the value
            if (x + this.Width / 2 / this.MapScale > 0)
            {
                x = 0 - this.Width / 2 / this.MapScale;
            } else if (x + this.Width / 2 / this.MapScale - this.Width / this.MapScale + this.Map.Width < 0)
            {
                x = 0 - this.Map.Width + this.Width / 2 / this.MapScale;
            }

            if (y + this.Height / 2 / this.MapScale > 0)
            {
                y = 0 - this.Height / 2 / this.MapScale;
            } else if (y + this.Height / 2 / this.MapScale - this.Height / this.MapScale + this.Map.Height < 0)
            {
                y = 0 - this.Map.Height + this.Height / 2 / this.MapScale;
            }

            // Set the new MapOffset
            this.MapOffsetX = x;
            this.MapOffsetY = y;
        }

        /// <summary>
        /// Update the current map offset with the current (new) scale
        /// </summary>
        private void UpdateMapOffset()
        {
            this.SetMapOffset(this.MapOffsetX, this.MapOffsetY);
        }

        /// <summary>
        /// Center the map to a given RD Location.
        /// </summary>
        /// <param name="Location">A RD location to center the map to</param>
        private void SetLocation(PointF Location)
        {
            // Convert the RD Location to bitmap pixels
            try
            {
                PointF BitmapLocation = this.RD2Bitmap(Location);

                // Set the new map offset
                this.SetMapOffset(BitmapLocation.X * -1, BitmapLocation.Y * -1);
            } catch (Exception) { throw; }
        }

        /// <summary>
        /// Center the map to the current RD Location if available.
        /// </summary>
        public void CenterMapToCurrentLocation()
        {
            // If there is no current RD location, throw an exception
            if (this.CurrentRDLocation == null) throw new NoLocationException();

            // Else set the current map location to the current RD Location
            try
            {
                this.SetLocation(this.CurrentRDLocation);
            }
            catch (Exception) { throw; }
        }

        public void CheckCurrentLocation()
        {
            // If there is no current RD location, throw an exception
            if (this.CurrentRDLocation == null) throw new NoLocationException();

            if (!this.IsOnMap(this.CurrentRDLocation)) throw new NotOnMapException();
        }

        protected override void OnLayout(bool changed, int left, int right, int top, int bottom)
        {
            if (changed)
            {
                // Set the minimum scale so the bitmap cannot be scaled smaller then the smallest dimension of the screen
                this.MinScale = Math.Max(
                    (float)this.Height / this.Map.Height,
                    (float)this.Width / this.Map.Width
                );
                // Set the default map scale to twice the minimum scale
                this.MapScale = this.MinScale * 2;
            }
        }

        protected override void OnDraw(Canvas c)
        {
            this.CurrentMatrix = new Matrix();
            
            // Move the center position on the bitmap to the top left corner
            this.CurrentMatrix.PostTranslate(this.MapOffsetX, this.MapOffsetY);
            // Then scale
            this.CurrentMatrix.PostScale(this.MapScale, this.MapScale);
            // And center on the screen
            this.CurrentMatrix.PostTranslate(this.Width / 2, this.Height / 2);

            c.DrawBitmap(this.Map, this.CurrentMatrix, new Paint());
            this.DrawLocation(c);
            this.DrawOrientation(c);
            this.DrawTrack(c);
        }

        /// <summary>
        /// Draw the current location on the canvas
        /// </summary>
        /// <param name="c">The canvas</param>
        protected void DrawLocation(Canvas c)
        {
            // If there is no current location, do nothing
            if (this.CurrentRDLocation == null) return;
            // If the current location is out of bound, don't draw
            if (!this.IsOnMap(this.CurrentRDLocation)) return;

            // TODO: Use the current Matrix if possible

            // Determine the position of the CurrentLocation on the screen
            float x = (this.RD2Bitmap(this.CurrentRDLocation).X + this.MapOffsetX) * this.MapScale + this.Width / 2;
            float y = (this.RD2Bitmap(this.CurrentRDLocation).Y + this.MapOffsetY) * this.MapScale + this.Height / 2;

            Paint InnerCirclePaint = new Paint();
            InnerCirclePaint.Color = new Color(68, 94, 224);

            Paint OuterCirclePaint = new Paint();
            OuterCirclePaint.Color = Color.White;
            
            // Paint the CurrentLocation
            c.DrawCircle(x, y, this.LocationRadius, OuterCirclePaint);
            c.DrawCircle(x, y, this.LocationRadius - this.LocationRadius * 0.15f, InnerCirclePaint);
        }

        /// <summary>
        /// Draw the current orientation on the canvas
        /// </summary>
        /// <param name="c">The canvas</param>
        protected void DrawOrientation(Canvas c)
        {
            // TODO: If no orientation is available, don't draw

            // If the current location is out of bound, don't draw
            if (!this.IsOnMap(this.CurrentRDLocation)) return;

            // Determine the position of the CurrentLocation on the screen
            float x = (this.RD2Bitmap(this.CurrentRDLocation).X + this.MapOffsetX) * this.MapScale + this.Width / 2;
            float y = (this.RD2Bitmap(this.CurrentRDLocation).Y + this.MapOffsetY) * this.MapScale + this.Height / 2;

            // Calculate all the orientation triangle dimensions
            float TriangleWidth = this.LocationRadius * 1.8f;
            float TriangleHeight = this.LocationRadius * 1f;

            float TriangleOffset = TriangleHeight * 0.2f;

            float TriangleBottomLeftX = x - TriangleWidth / 2;
            float TriangleBottomRightX = x + TriangleWidth / 2;
            float TriangleTopX = x;

            float TriangleBottomY = y + this.LocationRadius + TriangleOffset;
            float TriangleTopY = TriangleBottomY + TriangleHeight;

            Path path = new Path();
            path.MoveTo(TriangleBottomLeftX, TriangleBottomY);
            path.LineTo(TriangleTopX, TriangleTopY);
            path.LineTo(TriangleBottomRightX, TriangleBottomY);
            path.LineTo(TriangleBottomLeftX, TriangleBottomY);
            path.Close();

            Paint z = new Paint();
            z.SetStyle(Paint.Style.Fill);
            z.Color = new Color(68, 94, 224);

            // Rotate the triangle the right way
            Matrix m = new Matrix();
            RectF b = new RectF();
            path.ComputeBounds(b, true);

            b.Top -= TriangleOffset * 2 + TriangleHeight + this.LocationRadius * 2;

            float HeightWidthDifference = b.Height() - b.Width();
            b.Left -= HeightWidthDifference / 2;
            b.Right += HeightWidthDifference / 2;

            // Draw orientation
            m.PostRotate(this.CurrentLocationRotation -180, b.CenterX(), b.CenterY());
            Console.WriteLine(this.CurrentLocationRotation);
            path.Transform(m);

            c.DrawPath(path, z);
        }

        private List<PointF> PointsToRD(List<Location> List)
        {
            List<PointF> ret = new List<PointF>();
            foreach(Location loc in List)
            {
                ret.Add(Projectie.Geo2RD(loc));
            }
            return ret;
        }

        private void DrawTrack(Canvas c)
        {
            if (this.Track == null) return;
            if (this.Track.CountSegments() <= 0) return;

            foreach(Segment segment in this.Track.GetSegments())
            {
                if (segment.CountPoints() <= 0) continue;

                try
                {
                    Path path = new Path();

                    List<PointF> points = PointsToRD(segment.GetPoints());

                    float FirstX = (this.RD2Bitmap(points[0]).X + this.MapOffsetX) * this.MapScale + this.Width / 2;
                    float FirstY = (this.RD2Bitmap(points[0]).Y + this.MapOffsetY) * this.MapScale + this.Height / 2;

                    path.MoveTo(FirstX, FirstY);

                    foreach (PointF TrackPoint in points)
                    {
                        float x = (this.RD2Bitmap(TrackPoint).X + this.MapOffsetX) * this.MapScale + this.Width / 2;
                        float y = (this.RD2Bitmap(TrackPoint).Y + this.MapOffsetY) * this.MapScale + this.Height / 2;

                        path.LineTo(x, y);
                    }

                    Paint paint = new Paint();
                    paint.Color = Color.Red;
                    paint.SetStyle(Paint.Style.Stroke);
                    paint.StrokeWidth = 10;

                    c.DrawPath(path, paint);
                }
                catch (NotOnMapException) { }
            }
        }

        /// <summary>
        /// The previous finger position
        /// </summary>
        protected PointF PreviousFingerPosition;

        /// <summary>
        /// True if the scale gesture is haping
        /// </summary>
        protected bool IsScaling = false;

        protected void RegisterTouchEvent(object sender, TouchEventArgs ea)
        {
            // Scale
            this.ScaleDetector.OnTouchEvent(ea.Event);

            // Drag
            if (ea.Event.PointerCount == 1 && !this.IsScaling)
            {
                if (ea.Event.Action == MotionEventActions.Move)
                {
                    this.SetOffset(ea.Event.GetX() - this.PreviousFingerPosition.X, ea.Event.GetY() - this.PreviousFingerPosition.Y);
                    this.PreviousFingerPosition = new PointF(ea.Event.GetX(), ea.Event.GetY());
                }
                this.PreviousFingerPosition = new PointF(ea.Event.GetX(), ea.Event.GetY());
            }

            if (this.IsScaling && ea.Event.PointerCount == 1 && ea.Event.Action == MotionEventActions.Up) this.IsScaling = false;

            // Redraw the view
            this.Invalidate();
        }

        // Scale
        public bool OnScale(ScaleGestureDetector detector)
        {
            this.SetScale(this.MapScale * detector.ScaleFactor);
            this.IsScaling = true;

            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
        }

        // Location
        public void OnLocationChanged(Location location)
        {
            this.CurrentRDLocation = Projectie.Geo2RD(location);
            this.Invalidate();
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {

        }

        // Orientation
        public void OnSensorChanged(SensorEvent e)
        {
            this.CurrentLocationRotation = e.Values[0];
            this.Invalidate();
        }
    }
}