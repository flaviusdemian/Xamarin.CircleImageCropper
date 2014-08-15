using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.Lang;
using CircleImageCropper.Cropwindow.Handle;
using CircleImageCropper.CropWindow.Pair;
using CircleImageCropper.Util;
using Edge = CircleImageCropper.CropWindow.Pair.Edge;
using Exception = System.Exception;
using Math = System.Math;

namespace com.edmodo.cropper.cropwindow
{
    public class CropOverlayView : View
    {
        // Private Constants ///////////////////////////////////////////////////////

        private static int SNAP_RADIUS_DP = 6;
        private static float DEFAULT_SHOW_GUIDELINES_LIMIT = 100;

        // Gets default values from PaintUtil, sets a bunch of values such that the
        // corners will draw correctly
        private static readonly float DEFAULT_CORNER_THICKNESS_DP = PaintUtil.getCornerThickness();
        private static readonly float DEFAULT_LINE_THICKNESS_DP = PaintUtil.getLineThickness();

        private static readonly float DEFAULT_CORNER_OFFSET_DP = (DEFAULT_CORNER_THICKNESS_DP / 2) -
                                                                 (DEFAULT_LINE_THICKNESS_DP / 2);

        private static readonly float DEFAULT_CORNER_EXTENSION_DP = DEFAULT_CORNER_THICKNESS_DP / 2 +
                                                                    DEFAULT_CORNER_OFFSET_DP;

        private static float DEFAULT_CORNER_LENGTH_DP = 20;

        // mGuidelines enumerations
        private static int GUIDELINES_OFF = 0;
        private static int GUIDELINES_ON_TOUCH = 1;
        private static int GUIDELINES_ON = 2;

        // Member Variables ////////////////////////////////////////////////////////

        // The Paint used to draw the white rectangle around the crop area.

        // Floats to save the current aspect ratio of the image
        private static int mAspectRatioX = CropImageView.DEFAULT_ASPECT_RATIO_X;
        private static int mAspectRatioY = CropImageView.DEFAULT_ASPECT_RATIO_Y;

        // The aspect ratio that the crop area should maintain; this variable is
        // only used when mMaintainAspectRatio is true.

        // Whether the Crop View has been initialized for the first time
        private bool initializedCropWindow;
        private Paint mBackgroundPaint;

        // The bounding box around the Bitmap that we are cropping.
        private Rect mBitmapRect;
        private Paint mBorderPaint;

        // Instance variables for the corner values
        private float mCornerExtension;
        private float mCornerLength;
        private float mCornerOffset;
        private Paint mCornerPaint;
        private bool mFixAspectRatio = CropImageView.DEFAULT_FIXED_ASPECT_RATIO;
        private Paint mGuidelinePaint;
        private int mGuidelines;
        private float mHandleRadius;
        private Handle mPressedHandle;
        private float mSnapRadius;
        private float mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;
        private Android.Util.Pair mTouchOffset;

        // Constructors ////////////////////////////////////////////////////////////

        protected CropOverlayView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {}

        public CropOverlayView(Context context)
            : base(context)
        {}

        public CropOverlayView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context);
        }

        public CropOverlayView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Init(context);
        }

        // View Methods ///////////////////////////////////////////////////////////

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            // Initialize the crop window here because we need the size of the view
            // to have been determined.
            InitCropWindow(mBitmapRect);
        }

        protected override void OnDraw(Canvas canvas)
        {
            try
            {
                base.OnDraw(canvas);

                // Draw translucent background for the cropped area.
                DrawBackground(canvas, mBitmapRect);

                if (ShowGuidelines())
                {
                    // Determines whether guidelines should be drawn or not
                    if (mGuidelines == GUIDELINES_ON)
                    {
                        DrawRuleOfThirdsGuidelines(canvas);
                    }
                    else if (mGuidelines == GUIDELINES_ON_TOUCH)
                    {
                        // Draw only when resizing
                        if (mPressedHandle != null)
                            DrawRuleOfThirdsGuidelines(canvas);
                    }
                    else if (mGuidelines == GUIDELINES_OFF)
                    {
                        // Do nothing
                    }
                }
                // Draw the circular border
                float cx = (EdgeManager.LEFT.coordinate + EdgeManager.RIGHT.coordinate) / 2;
                float cy = (EdgeManager.TOP.coordinate + EdgeManager.BOTTOM.coordinate) / 2;
                float radius = (EdgeManager.RIGHT.coordinate - EdgeManager.LEFT.coordinate) / 2;

                canvas.DrawCircle(cx, cy, radius, mBorderPaint);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            try
            {
                // If this View is not enabled, don't allow for touch interactions.
                if (!Enabled)
                {
                    return false;
                }

                switch (ev.Action)
                {
                    case MotionEventActions.Down:
                        OnActionDown(ev.GetX(), ev.GetY());
                        return true;

                    case MotionEventActions.Up:
                    case MotionEventActions.Cancel:
                        Parent.RequestDisallowInterceptTouchEvent(false);
                        OnActionUp();
                        return true;

                    case MotionEventActions.Move:
                        OnActionMove(ev.GetX(), ev.GetY());
                        Parent.RequestDisallowInterceptTouchEvent(true);
                        return true;

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }

        // Public Methods //////////////////////////////////////////////////////////

        /**
         * Informs the CropOverlayView of the image's position relative to the
         * ImageView. This is necessary to call in order to draw the crop window.
         * 
         * @param bitmapRect the image's bounding box
         */

        public void SetBitmapRect(Rect bitmapRect)
        {
            mBitmapRect = bitmapRect;
            InitCropWindow(mBitmapRect);
        }

        /**
         * Resets the crop overlay view.
         * 
         * @param bitmap the Bitmap to set
         */

        public void ResetCropOverlayView()
        {
            try
            {
                if (initializedCropWindow)
                {
                    InitCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets the guidelines for the CropOverlayView to be either on, off, or to
         * show when resizing the application.
         * 
         * @param guidelines Integer that signals whether the guidelines should be
         *            on, off, or only showing when resizing.
         */

        public void SetGuidelines(int guidelines)
        {
            try
            {
                if (guidelines < 0 || guidelines > 2)
                    throw new IllegalArgumentException("Guideline value must be set between 0 and 2. See documentation.");
                mGuidelines = guidelines;

                if (initializedCropWindow)
                {
                    InitCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets whether the aspect ratio is fixed or not; true fixes the aspect
         * ratio, while false allows it to be changed.
         * 
         * @param fixAspectRatio bool that signals whether the aspect ratio
         *            should be maintained.
         */

        public void SetFixedAspectRatio(bool fixAspectRatio)
        {
            try
            {
                mFixAspectRatio = fixAspectRatio;

                if (initializedCropWindow)
                {
                    InitCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets the X value of the aspect ratio; is defaulted to 1.
         * 
         * @param aspectRatioX int that specifies the new X value of the aspect
         *            ratio
         */

        public void SetAspectRatioX(int aspectRatioX)
        {
            try
            {
                if (aspectRatioX <= 0)
                    throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
                mAspectRatioX = aspectRatioX;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;

                if (initializedCropWindow)
                {
                    InitCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets the Y value of the aspect ratio; is defaulted to 1.
         * 
         * @param aspectRatioY int that specifies the new Y value of the aspect
         *            ratio
         */

        public void SetAspectRatioY(int aspectRatioY)
        {
            try
            {
                if (aspectRatioY <= 0)
                    throw new IllegalArgumentException(
                        "Cannot set aspect ratio value to a number less than or equal to 0.");
                mAspectRatioY = aspectRatioY;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;

                if (initializedCropWindow)
                {
                    InitCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets all initial values, but does not call InitCropWindow to reset the
         * views. Used once at the very start to initialize the attributes.
         * 
         * @param guidelines Integer that signals whether the guidelines should be
         *            on, off, or only showing when resizing.
         * @param fixAspectRatio bool that signals whether the aspect ratio
         *            should be maintained.
         * @param aspectRatioX float that specifies the new X value of the aspect
         *            ratio
         * @param aspectRatioY float that specifies the new Y value of the aspect
         *            ratio
         */

        public void SetInitialAttributeValues(int guidelines,
            bool fixAspectRatio,
            int aspectRatioX,
            int aspectRatioY)
        {
            try
            {
                if (guidelines < 0 || guidelines > 2)
                    throw new IllegalArgumentException("Guideline value must be set between 0 and 2. See documentation.");
                mGuidelines = guidelines;

                mFixAspectRatio = fixAspectRatio;

                if (aspectRatioX <= 0)
                    throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
                mAspectRatioX = aspectRatioX;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;

                if (aspectRatioY <= 0)
                    throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
                mAspectRatioY = aspectRatioY;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        // Private Methods /////////////////////////////////////////////////////////

        private void Init(Context context)
        {
            try
            {
                DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
                mHandleRadius = HandleUtil.getTargetRadius(context);
                mSnapRadius = TypedValue.ApplyDimension(ComplexUnitType.Dip, SNAP_RADIUS_DP, displayMetrics);
                mBorderPaint = PaintUtil.newBorderPaint(context);
                mGuidelinePaint = PaintUtil.newGuidelinePaint();
                mBackgroundPaint = PaintUtil.newBackgroundPaint(context);
                mCornerPaint = PaintUtil.newCornerPaint(context);
                // Sets the values for the corner sizes
                mCornerOffset = TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_CORNER_OFFSET_DP, displayMetrics);
                mCornerExtension = TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_CORNER_EXTENSION_DP, displayMetrics);
                mCornerLength = TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_CORNER_LENGTH_DP, displayMetrics);
                // Sets guidelines to default until specified otherwise
                mGuidelines = CropImageView.DEFAULT_GUIDELINES;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Set the initial crop window size and position. This is dependent on the
         * size and position of the image being cropped.
         * 
         * @param bitmapRect the bounding box around the image being cropped
         */

        private void InitCropWindow(Rect bitmapRect)
        {
            try
            {
                // Tells the attribute functions the crop window has already been
                // initialized
                if (initializedCropWindow == false)
                    initializedCropWindow = true;

                if (mFixAspectRatio)
                {
                    // If the image aspect ratio is wider than the crop aspect ratio,
                    // then the image height is the determining initial length. Else,
                    // vice-versa.
                    if (AspectRatioUtil.calculateAspectRatio(bitmapRect) > mTargetAspectRatio)
                    {
                        EdgeManager.TOP.coordinate = bitmapRect.Top;
                        EdgeManager.BOTTOM.coordinate = bitmapRect.Bottom;

                        float centerX = Width / 2f;

                        // Limits the aspect ratio to no less than 40 wide or 40 tall
                        float cropWidth = Math.Max(Edge.MIN_CROP_LENGTH_PX,
                            AspectRatioUtil.calculateWidth(EdgeManager.TOP.coordinate,
                                EdgeManager.BOTTOM.coordinate,
                                mTargetAspectRatio));

                        // Create new TargetAspectRatio if the original one does not fit
                        // the screen
                        if (cropWidth == Edge.MIN_CROP_LENGTH_PX)
                            mTargetAspectRatio = (Edge.MIN_CROP_LENGTH_PX) /
                                                 (EdgeManager.BOTTOM.coordinate - EdgeManager.TOP.coordinate);

                        float halfCropWidth = cropWidth / 2f;
                        EdgeManager.LEFT.coordinate = (centerX - halfCropWidth);
                        EdgeManager.RIGHT.coordinate = (centerX + halfCropWidth);
                    }
                    else
                    {
                        EdgeManager.LEFT.coordinate = bitmapRect.Left;
                        EdgeManager.RIGHT.coordinate = bitmapRect.Right;

                        float centerY = Height / 2f;

                        // Limits the aspect ratio to no less than 40 wide or 40 tall
                        float cropHeight = Math.Max(Edge.MIN_CROP_LENGTH_PX,
                            AspectRatioUtil.calculateHeight(EdgeManager.LEFT.coordinate,
                                EdgeManager.RIGHT.coordinate,
                                mTargetAspectRatio));

                        // Create new TargetAspectRatio if the original one does not fit
                        // the screen
                        if (cropHeight == Edge.MIN_CROP_LENGTH_PX)
                            mTargetAspectRatio = (EdgeManager.RIGHT.coordinate - EdgeManager.LEFT.coordinate) /
                                                 Edge.MIN_CROP_LENGTH_PX;

                        float halfCropHeight = cropHeight / 2f;
                        EdgeManager.TOP.coordinate = (centerY - halfCropHeight);
                        EdgeManager.BOTTOM.coordinate = (centerY + halfCropHeight);
                    }
                }
                else
                {
                    // ... do not fix aspect ratio...

                    // Initialize crop window to have 10% padding w/ respect to image.
                    float horizontalPadding = 0.1f * bitmapRect.Width();
                    float verticalPadding = 0.1f * bitmapRect.Height();

                    EdgeManager.LEFT.coordinate = (bitmapRect.Left + horizontalPadding);
                    EdgeManager.TOP.coordinate = (bitmapRect.Top + verticalPadding);
                    EdgeManager.RIGHT.coordinate = (bitmapRect.Right - horizontalPadding);
                    EdgeManager.BOTTOM.coordinate = (bitmapRect.Bottom - verticalPadding);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Indicates whether the crop window is small enough that the guidelines
         * should be shown. Public because this function is also used to determine
         * if the center handle should be focused.
         * 
         * @return bool Whether the guidelines should be shown or not
         */

        public static bool ShowGuidelines()
        {
            if ((Math.Abs(EdgeManager.LEFT.coordinate - EdgeManager.RIGHT.coordinate) < DEFAULT_SHOW_GUIDELINES_LIMIT)
                ||
                (Math.Abs(EdgeManager.TOP.coordinate - EdgeManager.BOTTOM.coordinate) < DEFAULT_SHOW_GUIDELINES_LIMIT))
                return false;
            return true;
        }

        private void DrawRuleOfThirdsGuidelines(Canvas canvas)
        {
            try
            {
                float left = EdgeManager.LEFT.coordinate;
                float top = EdgeManager.TOP.coordinate;
                float right = EdgeManager.RIGHT.coordinate;
                float bottom = EdgeManager.BOTTOM.coordinate;


                float cx = (left + right) / 2;
                float cy = (top + bottom) / 2;
                float radius = (right - left) / 2;

                var circleSelectionPath = new Path();
                circleSelectionPath.AddCircle(cx, cy, radius, Path.Direction.Cw);
                canvas.ClipPath(circleSelectionPath, Region.Op.Replace);


                // Draw vertical guidelines.
                float oneThirdCropWidth = Edge.getWidth() / 3;

                float x1 = left + oneThirdCropWidth;
                canvas.DrawLine(x1, top, x1, bottom, mGuidelinePaint);
                float x2 = right - oneThirdCropWidth;
                canvas.DrawLine(x2, top, x2, bottom, mGuidelinePaint);

                // Draw horizontal guidelines.
                float oneThirdCropHeight = Edge.getHeight() / 3;

                float y1 = top + oneThirdCropHeight;
                canvas.DrawLine(left, y1, right, y1, mGuidelinePaint);
                float y2 = bottom - oneThirdCropHeight;
                canvas.DrawLine(left, y2, right, y2, mGuidelinePaint);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void DrawBackground(Canvas canvas, Rect bitmapRect)
        {
            try
            {
                float left = EdgeManager.LEFT.coordinate;
                float top = EdgeManager.TOP.coordinate;
                float right = EdgeManager.RIGHT.coordinate;
                float bottom = EdgeManager.BOTTOM.coordinate;

                float cx = (left + right) / 2;
                float cy = (top + bottom) / 2;
                float radius = (right - left) / 2;


                var fullCanvasPath = new Path();
                fullCanvasPath.AddRect(bitmapRect.Left, bitmapRect.Top, bitmapRect.Right, bitmapRect.Bottom,
                    Path.Direction.Cw);

                var circleSelectionPath = new Path();
                circleSelectionPath.AddCircle(cx, cy, radius, Path.Direction.Ccw);

                canvas.ClipPath(fullCanvasPath);
                canvas.ClipPath(circleSelectionPath, Region.Op.Difference);

                //Draw semi-transparent background
                canvas.DrawRect(bitmapRect.Left, bitmapRect.Top, bitmapRect.Right, bitmapRect.Bottom, mBackgroundPaint);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void DrawCorners(Canvas canvas)
        {
            try
            {
                float left = EdgeManager.LEFT.coordinate;
                float top = EdgeManager.TOP.coordinate;
                float right = EdgeManager.RIGHT.coordinate;
                float bottom = EdgeManager.BOTTOM.coordinate;

                // Draws the corner lines

                // Top left
                canvas.DrawLine(left - mCornerOffset,
                    top - mCornerExtension,
                    left - mCornerOffset,
                    top + mCornerLength,
                    mCornerPaint);
                canvas.DrawLine(left, top - mCornerOffset, left + mCornerLength, top - mCornerOffset, mCornerPaint);

                // Top right
                canvas.DrawLine(right + mCornerOffset,
                    top - mCornerExtension,
                    right + mCornerOffset,
                    top + mCornerLength,
                    mCornerPaint);
                canvas.DrawLine(right, top - mCornerOffset, right - mCornerLength, top - mCornerOffset, mCornerPaint);

                // Bottom left
                canvas.DrawLine(left - mCornerOffset,
                    bottom + mCornerExtension,
                    left - mCornerOffset,
                    bottom - mCornerLength,
                    mCornerPaint);
                canvas.DrawLine(left,
                    bottom + mCornerOffset,
                    left + mCornerLength,
                    bottom + mCornerOffset,
                    mCornerPaint);

                // Bottom left
                canvas.DrawLine(right + mCornerOffset,
                    bottom + mCornerExtension,
                    right + mCornerOffset,
                    bottom - mCornerLength,
                    mCornerPaint);
                canvas.DrawLine(right,
                    bottom + mCornerOffset,
                    right - mCornerLength,
                    bottom + mCornerOffset,
                    mCornerPaint);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Handles a {@link MotionEvent#ACTION_DOWN} event.
         * 
         * @param x the x-coordinate of the down action
         * @param y the y-coordinate of the down action
         */

        private void OnActionDown(float x, float y)
        {
            float left = EdgeManager.LEFT.coordinate;
            float top = EdgeManager.TOP.coordinate;
            float right = EdgeManager.RIGHT.coordinate;
            float bottom = EdgeManager.BOTTOM.coordinate;

            mPressedHandle = HandleUtil.getPressedHandle(x, y, left, top, right, bottom, mHandleRadius);

            if (mPressedHandle == null)
                return;

            // Calculate the offset of the touch point from the precise location
            // of the handle. Save these values in a member variable since we want
            // to maintain this offset as we drag the handle.
            mTouchOffset = HandleUtil.getOffset(mPressedHandle, x, y, left, top, right, bottom);

            Invalidate();
        }

        /**
         * Handles a {@link MotionEvent#ACTION_UP} or
         * {@link MotionEvent#ACTION_CANCEL} event.
         */

        private void OnActionUp()
        {
            if (mPressedHandle == null)
                return;

            mPressedHandle = null;

            Invalidate();
        }

        /**
         * Handles a {@link MotionEvent#ACTION_MOVE} event.
         * 
         * @param x the x-coordinate of the move event
         * @param y the y-coordinate of the move event
         */

        private void OnActionMove(float x, float y)
        {
            try
            {
                if (mPressedHandle == null)
                    return;

                // Adjust the coordinates for the finger position's offset (i.e. the
                // distance from the initial touch to the precise handle location).
                // We want to maintain the initial touch's distance to the pressed
                // handle so that the crop window size does not "jump".
                //TODO: FIX
                x += (float)mTouchOffset.First;
                y += (float)mTouchOffset.Second;

                // Calculate the new crop window size/position.
                if (mFixAspectRatio)
                {
                    mPressedHandle.updateCropWindow(x, y, mTargetAspectRatio, mBitmapRect, mSnapRadius);
                }
                else
                {
                    mPressedHandle.updateCropWindow(x, y, mBitmapRect, mSnapRadius);
                }
                Invalidate();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}