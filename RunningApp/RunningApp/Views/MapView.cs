using System;
using Android.Content;
using Android.Util;
using Android.Views;

using Android.Graphics;

namespace RunningApp.Views
{
    public class MapView : View, ScaleGestureDetector.IOnScaleGestureListener
    {
        private Bitmap Map;
        private ScaleGestureDetector ScaleDetector;
        private Matrix CurrentMatrix = new Matrix();
        private Matrix PreviousMatrix = new Matrix();

        private PointF Position;
        private float CenterOffsetX;
        private float CenterOffsetY;
        private float AnchorX;
        private float AnchorY;
        private float XYRotation = 0;
        private float Scale;
        private float DefaultScale;

        private bool FirstDraw = true;
        private bool FirstMatrix = true;

        private float MinScale;
        private float MaxScale = 5.0f;

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
        }

        protected void SetScale(float NewScale)
        {
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
        }

        protected void SetAnchorPoint(PointF coords)
        {
            this.AnchorX = coords.X;
            this.AnchorY = coords.Y;
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

                this.CenterOffsetX = this.Map.Width / 2 * -1;
                this.CenterOffsetY = this.Map.Height / 2 * -1;
            }
        }

        protected override void OnDraw(Canvas c)
        {
            this.FirstDrawActions();

            this.CurrentMatrix = new Matrix();

            if (this.FirstDraw)
            {
                this.CurrentMatrix.PostTranslate(this.CenterOffsetX, this.CenterOffsetY);
            } else if (this.FirstMatrix)
            {
                this.CurrentMatrix.PostTranslate(this.CenterOffsetX - this.AnchorX - this.Width / 2, this.CenterOffsetY - this.AnchorY - this.Height / 2);
            } else
            {
                float[] values = new float[9];
                this.PreviousMatrix.GetValues(values);

                this.CurrentMatrix.PostTranslate(values[Matrix.MtransX] - this.AnchorX, values[Matrix.MtransY] - this.AnchorY);
            }

            this.CurrentMatrix.PostScale(this.Scale, this.Scale);

            this.CurrentMatrix.PostTranslate(this.Width / 2, this.Height / 2);

            c.DrawBitmap(this.Map, this.CurrentMatrix, new Paint());

            Paint t = new Paint();
            t.Color = Color.Black;
            c.DrawCircle(this.AnchorX, this.AnchorY, 50*this.Scale, t);

            this.FirstDraw = false;
        }

        protected void RegisterTouchEvent(object sender, TouchEventArgs ea)
        {
            this.ScaleDetector.OnTouchEvent(ea.Event);
        }

        // Scale
        public bool OnScale(ScaleGestureDetector detector)
        {
            this.SetAnchorPoint(new PointF(detector.FocusX, detector.FocusY));
            this.SetScale(this.Scale * detector.ScaleFactor);
            this.Invalidate();
            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            this.PreviousMatrix = this.CurrentMatrix;
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            this.FirstMatrix = false;
        }
    }
}