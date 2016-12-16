using System;
using Android.Content;
using Android.Util;
using Android.Views;

using Android.Graphics;

using Android.Locations;
using Android.OS;
using Android.Runtime;

using Android.Hardware;

using Kaart;

namespace RunningApp.Views
{
    public class MapView : View, ScaleGestureDetector.IOnScaleGestureListener, ILocationListener, ISensorEventListener
    {
        private Bitmap Map;
        private ScaleGestureDetector ScaleDetector;
        private Matrix CurrentMatrix = new Matrix();

        private const float LocationThreshold = 1;

        private float MapOffsetX;
        private float MapOffsetY;
        private float Scale;

        private bool FirstDraw = true;

        private float MinScale;
        private float MaxScale = 5.0f;

        private float LocationRadius = 25;

        private PointF CurrentRDLocation;
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
            this.SetBackgroundColor(Color.White);

            BitmapFactory.Options MapOptions = new BitmapFactory.Options();
            MapOptions.InScaled = false;
            Map = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Map, MapOptions);

            this.ScaleDetector = new ScaleGestureDetector(c, this);
            this.Touch += this.RegisterTouchEvent;

            LocationManager lm = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria crit = new Criteria();
            crit.Accuracy = Accuracy.Fine;
            string lp = lm.GetBestProvider(crit, true);
            lm.RequestLocationUpdates(lp, 0, 0, this);

            SensorManager sm = (SensorManager)c.GetSystemService(Context.SensorService);
            sm.RegisterListener(this, sm.GetDefaultSensor(SensorType.Orientation), SensorDelay.Ui);
        }

        protected PointF RD2Bitmap(PointF Location)
        {
            PointF BitmapLocation = new PointF();

            BitmapLocation.X = (Location.X - 136000) * 0.4f;
            BitmapLocation.Y = (458000 - Location.Y) * 0.4f;

            return BitmapLocation;
        }

        protected void SetScale(float NewScale)
        {
            float PreviousScale = this.Scale;
            if (NewScale < this.MaxScale)
            {
                this.Scale = NewScale;
                if (NewScale > this.MinScale)
                {
                    this.Scale = NewScale;
                } else
                {
                    this.Scale = this.MinScale;
                }
            } else
            {
                this.Scale = this.MaxScale;
            }
            this.UpdateMapOffset();
        }

        protected void SetOffset(float OffsetX, float OffsetY)
        {
            float ScaledOffsetX = OffsetX / this.Scale;
            float ScaledOffsetY = OffsetY / this.Scale;

            this.SetMapOffset(this.MapOffsetX + ScaledOffsetX, this.MapOffsetY + ScaledOffsetY);
        }

        private void SetMapOffset(float x, float y)
        {
            if (x + this.Width / 2 / this.Scale > 0)
            {
                x = 0 - this.Width / 2 / this.Scale;
            }

            if (y + this.Height / 2 / this.Scale > 0)
            {
                y = 0 - this.Height / 2 / this.Scale;
            }

            if (x + this.Width / 2 / this.Scale - this.Width / this.Scale + this.Map.Width < 0)
            {
                x = 0 - this.Map.Width + this.Width / 2 / this.Scale;
            }

            if (y + this.Height / 2 / this.Scale - this.Height / this.Scale + this.Map.Height < 0)
            {
                y = 0 - this.Map.Height + this.Height / 2 / this.Scale;
            }

            this.MapOffsetX = x;
            this.MapOffsetY = y;
        }

        private void UpdateMapOffset()
        {
            this.SetMapOffset(this.MapOffsetX, this.MapOffsetY);
        }

        private void SetLocation(PointF Location)
        {
            PointF BitmapLocation = this.RD2Bitmap(Location);
            float ScaledX = (this.Map.Width / 2 - BitmapLocation.X) * this.Scale;
            float ScaledY = (this.Map.Height / 2 - BitmapLocation.Y) * this.Scale;

            Console.WriteLine(ScaledX);
            Console.WriteLine(ScaledY);

            this.SetMapOffset(ScaledX, ScaledY);
        }

        public void CenterMap()
        {
            if (this.CurrentRDLocation == null) return;
            this.SetLocation(this.CurrentRDLocation);
        }

        private void FirstDrawActions()
        {
            if (this.FirstDraw)
            {
                this.MinScale = Math.Min(
                    this.Map.Height / this.Height,
                    this.Map.Width / this.Width
                );
                this.Scale = this.MinScale * 2;

                this.MapOffsetX = this.Map.Width / 2 * -1;
                this.MapOffsetY = this.Map.Height / 2 * -1;
            }
            this.FirstDraw = false;
        }

        protected override void OnDraw(Canvas c)
        {
            Paint t = new Paint();
            t.Color = Color.Black;
            t.StrokeWidth = 50;

            this.FirstDrawActions();

            this.CurrentMatrix = new Matrix();
            
            this.CurrentMatrix.PostTranslate(this.MapOffsetX, this.MapOffsetY);
            this.CurrentMatrix.PostScale(this.Scale, this.Scale);
            this.CurrentMatrix.PostTranslate(this.Width / 2, this.Height / 2);

            c.DrawBitmap(this.Map, this.CurrentMatrix, new Paint());
            this.DrawLocation(c);
        }

        public void DrawLocation(Canvas c)
        {
            if (this.CurrentRDLocation == null) return;

            float x = this.RD2Bitmap(this.CurrentRDLocation).X * this.Scale + this.MapOffsetX * this.Scale + this.Width / 2;
            float y = this.RD2Bitmap(this.CurrentRDLocation).Y * this.Scale + this.MapOffsetY * this.Scale + this.Height / 2;

            Paint p = new Paint();
            p.Color = Color.Blue;

            c.DrawCircle(x, y, this.LocationRadius, p);

            float TriangleWidth = this.LocationRadius * 2;
            float TriangleHeight = this.LocationRadius * 1.4f;

            float TriangleOffset = TriangleHeight * 0.2f;

            float TriangleBottomLeftX = x - TriangleWidth / 2;
            float TriangleBottomRightX = x + TriangleWidth / 2;
            float TriangleTopX = x;

            float TrianlgeBottomY = y + this.LocationRadius + TriangleOffset;
            float TriangleTopY = TrianlgeBottomY + TriangleHeight;

            Path path = new Path();
            // left
            path.MoveTo(TriangleBottomLeftX, TrianlgeBottomY);
            path.LineTo(TriangleTopX, TriangleTopY);
            path.LineTo(TriangleBottomRightX, TrianlgeBottomY);
            path.LineTo(TriangleBottomLeftX, TrianlgeBottomY);
            path.Close();

            Paint z = new Paint();
            z.SetStyle(Paint.Style.Fill);
            z.Color = Color.Red;

            Matrix m = new Matrix();
            RectF b = new RectF();
            path.ComputeBounds(b, true);

            b.Top -= TriangleOffset * 2 + TriangleHeight + this.LocationRadius * 2;

            float HeightWidthDifference = b.Height() - b.Width();
            b.Left -= HeightWidthDifference / 2;
            b.Right += HeightWidthDifference / 2;

            //m.PostRotate(this.CurrentLocationRotation, TriangleWidth / 2, this.LocationRadius + TriangleHeight + TriangleOffset);
            m.PostRotate(this.CurrentLocationRotation, b.CenterX(), b.CenterY());

            path.Transform(m);

            c.DrawPath(path, z);
        }

        protected float PreviousDragX;
        protected float PreviousDragY;
        protected bool IsScaling = false;

        protected int PreviousPointerCount = 0;
        protected bool skip = false;

        protected void RegisterTouchEvent(object sender, TouchEventArgs ea)
        {
            // Scale
            this.ScaleDetector.OnTouchEvent(ea.Event);

            // Drag
            if (ea.Event.PointerCount == 1 && !this.IsScaling && !this.skip)
            {
                switch (ea.Event.Action)
                {
                    case MotionEventActions.Down:
                        this.PreviousDragX = ea.Event.GetX();
                        this.PreviousDragY = ea.Event.GetY();
                        break;
                    case MotionEventActions.Move:
                        this.SetOffset(ea.Event.GetX() - this.PreviousDragX, ea.Event.GetY() - this.PreviousDragY);

                        this.PreviousDragX = ea.Event.GetX();
                        this.PreviousDragY = ea.Event.GetY();
                        break;
                }
            }

            if (this.PreviousPointerCount == 2 && ea.Event.PointerCount == 1)
            {
                this.IsScaling = false;
                this.skip = true;
            }
            if (this.skip && ea.Event.Action == MotionEventActions.Up) this.skip = false;

            this.PreviousPointerCount = ea.Event.PointerCount;

            this.Invalidate();
        }

        // Scale
        public bool OnScale(ScaleGestureDetector detector)
        {
            this.SetScale(this.Scale * detector.ScaleFactor);
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

        public void OnLocationChanged(Location location)
        {
            PointF PreviousRDLocation = this.CurrentRDLocation;

            this.CurrentRDLocation = Projectie.Geo2RD(location);

            if (PreviousRDLocation == null)
            {
                this.Invalidate();
                return;
            }

            if (Math.Max(Math.Abs(PreviousRDLocation.X - this.CurrentRDLocation.X), Math.Abs(PreviousRDLocation.Y - this.CurrentRDLocation.Y)) > MapView.LocationThreshold) {
                this.Invalidate();
            }
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

        public void OnSensorChanged(SensorEvent e)
        {
            this.CurrentLocationRotation = e.Values[0];
            this.Invalidate();
        }
    }
}