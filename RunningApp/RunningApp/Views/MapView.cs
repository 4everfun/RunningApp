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

        private float CurrentTotalOffsetX;
        private float CurrentTotalOffsetY;

        private float CenterOffsetX;
        private float CenterOffsetY;
        private float PreviousCenterX;
        private float PreviousCenterY;
        private float OffsetX;
        private float OffsetY;
        private float Scale;

        private bool FirstDraw = true;

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
        }

        protected void SetOffset(float OffsetX, float OffsetY)
        {
            float ScaledOffsetX = OffsetX / this.Scale;
            float ScaledOffsetY = OffsetY / this.Scale;

            if (OffsetX != 0 || OffsetY != 0)
            {
                if (this.PreviousCenterX + ScaledOffsetX + this.Width / 2 / this.Scale > 0)
                {
                    this.PreviousCenterX = 0 - this.Width / 2 / this.Scale;
                    ScaledOffsetX = 0;
                }

                if (this.PreviousCenterY + ScaledOffsetY + this.Height / 2 / this.Scale > 0)
                {
                    this.PreviousCenterY = 0 - this.Height / 2 / this.Scale;
                    ScaledOffsetY = 0;
                }

                if (this.PreviousCenterX + ScaledOffsetX + this.Width / 2 / this.Scale - this.Width / this.Scale + this.Map.Width < 0)
                {
                    this.PreviousCenterX = 0 - this.Map.Width + this.Width / 2 / this.Scale;
                    ScaledOffsetX = 0;
                }

                if (this.PreviousCenterY + ScaledOffsetY + this.Height / 2 / this.Scale - this.Height / this.Scale + this.Map.Height < 0)
                {
                    this.PreviousCenterY = 0 - this.Map.Height + this.Height / 2 / this.Scale;
                    ScaledOffsetY = 0;
                }
            }

            this.OffsetX = ScaledOffsetX;
            this.OffsetY = ScaledOffsetY;
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

                this.PreviousCenterX = this.CenterOffsetX;
                this.PreviousCenterY = this.CenterOffsetY;
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
            
            this.CurrentMatrix.PostTranslate(this.PreviousCenterX + this.OffsetX, this.PreviousCenterY + this.OffsetY);
            this.CurrentMatrix.PostScale(this.Scale, this.Scale);
            this.CurrentMatrix.PostTranslate(this.Width / 2, this.Height / 2);

            this.CurrentTotalOffsetX = this.PreviousCenterX + this.OffsetX;
            this.CurrentTotalOffsetY = this.PreviousCenterY + this.OffsetY;

            c.DrawBitmap(this.Map, this.CurrentMatrix, new Paint());
        }

        protected float DragStartX;
        protected float DragStartY;
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
                        this.DragStartX = ea.Event.GetX();
                        this.DragStartY = ea.Event.GetY();
                        break;
                    case MotionEventActions.Move:
                        this.SetOffset(ea.Event.GetX() - this.DragStartX, ea.Event.GetY() - this.DragStartY);
                        break;
                    case MotionEventActions.Up:
                        this.PreviousCenterX = this.CurrentTotalOffsetX;
                        this.PreviousCenterY = this.CurrentTotalOffsetY;
                        this.SetOffset(0, 0);
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
    }
}