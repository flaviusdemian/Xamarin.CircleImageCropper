using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.CircleImageCropperSample.Cropper;
using Xamarin.CircleImageCropperSample.Cropwindow.Handle;
using Xamarin.CircleImageCropperSample.Cropwindow.Pair;
using Xamarin.CircleImageCropperSample.Util;
using Math = System.Math;

namespace Xamarin.CircleImageCropperSample.CropWindow
{
    public class CropOverlayView : View
    {
        // Private Constants ///////////////////////////////////////////////////////

        private static int SNAP_RADIUS_DP = 6;
        private static float DEFAULT_SHOW_GUIDELINES_LIMIT = 100;

        // Gets default values from PaintUtil, sets a bunch of values such that the
        // corners will draw correctly
        private static float DEFAULT_CORNER_THICKNESS_DP = PaintUtil.getCornerThickness();
        private static float DEFAULT_LINE_THICKNESS_DP = PaintUtil.getLineThickness();
        private static float DEFAULT_CORNER_OFFSET_DP = (DEFAULT_CORNER_THICKNESS_DP / 2) - (DEFAULT_LINE_THICKNESS_DP / 2);
        private static float DEFAULT_CORNER_EXTENSION_DP = DEFAULT_CORNER_THICKNESS_DP / 2
                                                                 + DEFAULT_CORNER_OFFSET_DP;
        private static float DEFAULT_CORNER_LENGTH_DP = 20;

        // mGuidelines enumerations
        private static int GUIDELINES_OFF = 0;
        private static int GUIDELINES_ON_TOUCH = 1;
        private static int GUIDELINES_ON = 2;

        // Member Variables ////////////////////////////////////////////////////////

        // The Paint used to draw the white rectangle around the crop area.
        private Paint mBorderPaint;

        // The Paint used to draw the guidelines within the crop area when pressed.
        private Paint mGuidelinePaint;

        // The Paint used to draw the corners of the Border
        private Paint mCornerPaint;

        // The Paint used to darken the surrounding areas outside the crop area.
        private Paint mBackgroundPaint;

        // The bounding box around the Bitmap that we are cropping.
        private Rect mBitmapRect;

        // The radius of the touch zone (in pixels) around a given Handle.
        private float mHandleRadius;

        // An EdgeType of the crop window will snap to the corresponding EdgeType of a
        // specified bounding box when the crop window EdgeType is less than or equal to
        // this distance (in pixels) away from the bounding box EdgeType.
        private float mSnapRadius;

        // Holds the x and y offset between the exact touch location and the exact
        // handle location that is activated. There may be an offset because we
        // allow for some leeway (specified by mHandleRadius) in activating a
        // handle. However, we want to maintain these offset values while the handle
        // is being dragged so that the handle doesn't jump.
        private Pair mTouchOffset;

        // The Handle that is currently pressed; null if no Handle is pressed.
        private Handle mPressedHandle;

        // Flag indicating if the crop area should always be a certain aspect ratio
        // (indicated by mTargetAspectRatio).
        private bool mFixAspectRatio = CropImageView.DEFAULT_FIXED_ASPECT_RATIO;

        // Floats to save the current aspect ratio of the image
        private static int mAspectRatioX = CropImageView.DEFAULT_ASPECT_RATIO_X;
        private static int mAspectRatioY = CropImageView.DEFAULT_ASPECT_RATIO_Y;

        // The aspect ratio that the crop area should maintain; this variable is
        // only used when mMaintainAspectRatio is true.
        private float mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;

        // Instance variables for customizable attributes
        private int mGuidelines;

        // Whether the Crop View has been initialized for the first time
        private bool initializedCropWindow = false;

        // Instance variables for the corner values
        private float mCornerExtension;
        private float mCornerOffset;
        private float mCornerLength;

        // Constructors ////////////////////////////////////////////////////////////

        public CropOverlayView(Context context)
            : base(context)
        {
            init(context);
        }

        public CropOverlayView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            init(context);
        }

        // View Methods ////////////////////////////////////////////////////////////

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {

            // Initialize the crop window here because we need the size of the view
            // to have been determined.
            initCropWindow(mBitmapRect);
        }

        protected void OnDraw(Canvas canvas)
        {

            base.OnDraw(canvas);

            // Draw translucent background for the cropped area.
            drawBackground(canvas, mBitmapRect);

            if (showGuidelines())
            {
                // Determines whether guidelines should be drawn or not
                if (mGuidelines == GUIDELINES_ON)
                {
                    drawRuleOfThirdsGuidelines(canvas);
                }
                else if (mGuidelines == GUIDELINES_ON_TOUCH)
                {
                    // Draw only when resizing
                    if (mPressedHandle != null)
                        drawRuleOfThirdsGuidelines(canvas);
                }
                else if (mGuidelines == GUIDELINES_OFF)
                {
                    // Do nothing
                }
            }


            // Draw the circular border
            float cx = (EdgeType.LEFT.getCoordinate() + EdgeType.RIGHT.getCoordinate()) / 2;
            float cy = (EdgeType.TOP.getCoordinate() + EdgeType.BOTTOM.getCoordinate()) / 2;
            float radius = (EdgeType.RIGHT.getCoordinate() - EdgeType.LEFT.getCoordinate()) / 2;

            canvas.DrawCircle(cx, cy, radius, mBorderPaint);
        }

        public bool OnTouchEvent(MotionEvent ev)
        {

            // If this View is not enabled, don't allow for touch interactions.
            if (!isEnabled())
            {
                return false;
            }

            switch (ev.Action)
            {

                case MotionEventActions.Down:
                    onActionDown(ev.GetX(), ev.GetY());
                    return true;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    Parent.RequestDisallowInterceptTouchEvent(false);
                    onActionUp();
                    return true;

                case MotionEventActions.Move:
                    onActionMove(ev.GetX(), ev.GetY());
                    Parent.RequestDisallowInterceptTouchEvent(true);
                    return true;

                default:
                    return false;
            }
        }

        // Public Methods //////////////////////////////////////////////////////////

        /**
         * Informs the CropOverlayView of the image's position relative to the
         * ImageView. This is necessary to call in order to draw the crop window.
         * 
         * @param bitmapRect the image's bounding box
         */
        public void setBitmapRect(Rect bitmapRect)
        {
            mBitmapRect = bitmapRect;
            initCropWindow(mBitmapRect);
        }

        /**
         * Resets the crop overlay view.
         * 
         * @param bitmap the Bitmap to set
         */
        public void resetCropOverlayView()
        {

            if (initializedCropWindow)
            {
                initCropWindow(mBitmapRect);
                Invalidate();
            }
        }

        /**
         * Sets the guidelines for the CropOverlayView to be either on, off, or to
         * show when resizing the application.
         * 
         * @param guidelines Integer that signals whether the guidelines should be
         *            on, off, or only showing when resizing.
         */
        public void setGuidelines(int guidelines)
        {
            if (guidelines < 0 || guidelines > 2)
                throw new IllegalArgumentException("Guideline value must be set between 0 and 2. See documentation.");
            else
            {
                mGuidelines = guidelines;

                if (initializedCropWindow)
                {
                    initCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
        }

        /**
         * Sets whether the aspect ratio is fixed or not; true fixes the aspect
         * ratio, while false allows it to be changed.
         * 
         * @param fixAspectRatio bool that signals whether the aspect ratio
         *            should be maintained.
         */
        public void setFixedAspectRatio(bool fixAspectRatio)
        {
            mFixAspectRatio = fixAspectRatio;

            if (initializedCropWindow)
            {
                initCropWindow(mBitmapRect);
                Invalidate();
            }
        }

        /**
         * Sets the X value of the aspect ratio; is defaulted to 1.
         * 
         * @param aspectRatioX int that specifies the new X value of the aspect
         *            ratio
         */
        public void setAspectRatioX(int aspectRatioX)
        {
            if (aspectRatioX <= 0)
                throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
            else
            {
                mAspectRatioX = aspectRatioX;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;

                if (initializedCropWindow)
                {
                    initCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
        }

        /**
         * Sets the Y value of the aspect ratio; is defaulted to 1.
         * 
         * @param aspectRatioY int that specifies the new Y value of the aspect
         *            ratio
         */
        public void setAspectRatioY(int aspectRatioY)
        {
            if (aspectRatioY <= 0)
                throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
            else
            {
                mAspectRatioY = aspectRatioY;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;

                if (initializedCropWindow)
                {
                    initCropWindow(mBitmapRect);
                    Invalidate();
                }
            }
        }

        /**
         * Sets all initial values, but does not call initCropWindow to reset the
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
        public void setInitialAttributeValues(int guidelines,
                                              bool fixAspectRatio,
                                              int aspectRatioX,
                                              int aspectRatioY)
        {
            if (guidelines < 0 || guidelines > 2)
                throw new IllegalArgumentException("Guideline value must be set between 0 and 2. See documentation.");
            else
                mGuidelines = guidelines;

            mFixAspectRatio = fixAspectRatio;

            if (aspectRatioX <= 0)
                throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
            else
            {
                mAspectRatioX = aspectRatioX;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;
            }

            if (aspectRatioY <= 0)
                throw new IllegalArgumentException("Cannot set aspect ratio value to a number less than or equal to 0.");
            else
            {
                mAspectRatioY = aspectRatioY;
                mTargetAspectRatio = ((float)mAspectRatioX) / mAspectRatioY;
            }

        }

        // Private Methods /////////////////////////////////////////////////////////

        private void init(Context context)
        {

            DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;

            mHandleRadius = HandleUtil.getTargetRadius(context);

            mSnapRadius = TypedValue.ApplyDimension(TypedValue.COMPLEX_UNIT_DIP,
                                                    SNAP_RADIUS_DP,
                                                    displayMetrics);

            mBorderPaint = PaintUtil.newBorderPaint(context);
            mGuidelinePaint = PaintUtil.newGuidelinePaint();
            mBackgroundPaint = PaintUtil.newBackgroundPaint(context);
            mCornerPaint = PaintUtil.newCornerPaint(context);

            // Sets the values for the corner sizes
            mCornerOffset = TypedValue.ApplyDimension(TypedValue.COMPLEX_UNIT_DIP,
                                                      DEFAULT_CORNER_OFFSET_DP,
                                                      displayMetrics);
            mCornerExtension = TypedValue.ApplyDimension(TypedValue.COMPLEX_UNIT_DIP,
                                                         DEFAULT_CORNER_EXTENSION_DP,
                                                         displayMetrics);
            mCornerLength = TypedValue.ApplyDimension(TypedValue.COMPLEX_UNIT_DIP,
                                                      DEFAULT_CORNER_LENGTH_DP,
                                                      displayMetrics);

            // Sets guidelines to default until specified otherwise
            mGuidelines = CropImageView.DEFAULT_GUIDELINES;
        }

        /**
         * Set the initial crop window size and position. This is dependent on the
         * size and position of the image being cropped.
         * 
         * @param bitmapRect the bounding box around the image being cropped
         */
        private void initCropWindow(Rect bitmapRect)
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

                    EdgeType.TOP.setCoordinate(bitmapRect.Top);
                    EdgeType.BOTTOM.setCoordinate(bitmapRect.Bottom);

                    float centerX = Width / 2f;

                    // Limits the aspect ratio to no less than 40 wide or 40 tall
                    float cropWidth = Math.Max(Xamarin.CircleImageCropperSample.Cropwindow.Pair.Edge.MIN_CROP_LENGTH_PX,
                                                    AspectRatioUtil.calculateWidth(EdgeType.TOP.getCoordinate(),
                                                                                   EdgeType.BOTTOM.getCoordinate(),
                                                                                   mTargetAspectRatio));

                    // Create new TargetAspectRatio if the original one does not fit
                    // the screen
                    if (cropWidth == Cropwindow.Pair.Edge.MIN_CROP_LENGTH_PX)
                        mTargetAspectRatio = (Xamarin.CircleImageCropperSample.Cropwindow.Pair.Edge.MIN_CROP_LENGTH_PX) / (EdgeType.BOTTOM.getCoordinate() - EdgeType.TOP.getCoordinate());

                    float halfCropWidth = cropWidth / 2f;
                    EdgeType.LEFT.setCoordinate(centerX - halfCropWidth);
                    EdgeType.RIGHT.setCoordinate(centerX + halfCropWidth);

                }
                else
                {

                    EdgeType.LEFT.setCoordinate(bitmapRect.left);
                    EdgeType.RIGHT.setCoordinate(bitmapRect.right);

                    float centerY = getHeight() / 2f;

                    // Limits the aspect ratio to no less than 40 wide or 40 tall
                    float cropHeight = Math.max(EdgeType.MIN_CROP_LENGTH_PX,
                                                     AspectRatioUtil.calculateHeight(EdgeType.LEFT.getCoordinate(),
                                                                                     EdgeType.RIGHT.getCoordinate(),
                                                                                     mTargetAspectRatio));

                    // Create new TargetAspectRatio if the original one does not fit
                    // the screen
                    if (cropHeight == EdgeType.MIN_CROP_LENGTH_PX)
                        mTargetAspectRatio = (EdgeType.RIGHT.getCoordinate() - EdgeType.LEFT.getCoordinate()) / EdgeType.MIN_CROP_LENGTH_PX;

                    float halfCropHeight = cropHeight / 2f;
                    EdgeType.TOP.setCoordinate(centerY - halfCropHeight);
                    EdgeType.BOTTOM.setCoordinate(centerY + halfCropHeight);
                }

            }
            else
            { // ... do not fix aspect ratio...

                // Initialize crop window to have 10% padding w/ respect to image.
                float horizontalPadding = 0.1f * bitmapRect.Width();
                float verticalPadding = 0.1f * bitmapRect.Height();

                EdgeType.LEFT.setCoordinate(bitmapRect.Left + horizontalPadding);
                EdgeType.TOP.setCoordinate(bitmapRect.Top + verticalPadding);
                EdgeType.RIGHT.setCoordinate(bitmapRect.Right - horizontalPadding);
                EdgeType.BOTTOM.setCoordinate(bitmapRect.Bottom - verticalPadding);
            }
        }

        /**
         * Indicates whether the crop window is small enough that the guidelines
         * should be shown. Public because this function is also used to determine
         * if the center handle should be focused.
         * 
         * @return bool Whether the guidelines should be shown or not
         */
        public static bool showGuidelines()
        {
            if ((Math.Abs(EdgeType.LEFT.getCoordinate() - EdgeType.RIGHT.getCoordinate()) < DEFAULT_SHOW_GUIDELINES_LIMIT)
                || (Math.Abs(EdgeType.TOP.getCoordinate() - EdgeType.BOTTOM.getCoordinate()) < DEFAULT_SHOW_GUIDELINES_LIMIT))
                return false;
            else
                return true;
        }

        private void drawRuleOfThirdsGuidelines(Canvas canvas)
        {


            float left = EdgeType.LEFT.getCoordinate();
            float top = EdgeType.TOP.getCoordinate();
            float right = EdgeType.RIGHT.getCoordinate();
            float bottom = EdgeType.BOTTOM.getCoordinate();


            float cx = (left + right) / 2;
            float cy = (top + bottom) / 2;
            float radius = (right - left) / 2;

            Path circleSelectionPath = new Path();
            circleSelectionPath.AddCircle(cx, cy, radius, Path.Direction.Cw);
            canvas.ClipPath(circleSelectionPath, Region.Op.Replace);


            // Draw vertical guidelines.
            float oneThirdCropWidth = EdgeType.getWidth() / 3;

            float x1 = left + oneThirdCropWidth;
            canvas.DrawLine(x1, top, x1, bottom, mGuidelinePaint);
            float x2 = right - oneThirdCropWidth;
            canvas.DrawLine(x2, top, x2, bottom, mGuidelinePaint);

            // Draw horizontal guidelines.
            float oneThirdCropHeight = EdgeType.getHeight() / 3;

            float y1 = top + oneThirdCropHeight;
            canvas.DrawLine(left, y1, right, y1, mGuidelinePaint);
            float y2 = bottom - oneThirdCropHeight;
            canvas.DrawLine(left, y2, right, y2, mGuidelinePaint);
        }

        private void drawBackground(Canvas canvas, Rect bitmapRect)
        {

            float left = EdgeType.LEFT.getCoordinate();
            float top = EdgeType.TOP.getCoordinate();
            float right = EdgeType.RIGHT.getCoordinate();
            float bottom = EdgeType.BOTTOM.getCoordinate();

            float cx = (left + right) / 2;
            float cy = (top + bottom) / 2;
            float radius = (right - left) / 2;


            Path fullCanvasPath = new Path();
            fullCanvasPath.AddRect(bitmapRect.Left, bitmapRect.Top, bitmapRect.Right, bitmapRect.Bottom, Path.Direction.Cw);

            Path circleSelectionPath = new Path();
            circleSelectionPath.AddCircle(cx, cy, radius, Path.Direction.Ccw);

            canvas.ClipPath(fullCanvasPath);
            canvas.ClipPath(circleSelectionPath, Region.Op.Difference);

            //Draw semi-transparent background
            canvas.DrawRect(bitmapRect.Left, bitmapRect.Top, bitmapRect.Right, bitmapRect.Bottom, mBackgroundPaint);

        }

        private void drawCorners(Canvas canvas)
        {

            float left = EdgeType.LEFT.getCoordinate();
            float top = EdgeType.TOP.getCoordinate();
            float right = EdgeType.RIGHT.getCoordinate();
            float bottom = EdgeType.BOTTOM.getCoordinate();

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

        /**
         * Handles a {@link MotionEvent#ACTION_DOWN} event.
         * 
         * @param x the x-coordinate of the down action
         * @param y the y-coordinate of the down action
         */
        private void onActionDown(float x, float y)
        {

            float left = EdgeType.LEFT.getCoordinate();
            float top = EdgeType.TOP.getCoordinate();
            float right = EdgeType.RIGHT.getCoordinate();
            float bottom = EdgeType.BOTTOM.getCoordinate();

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
        private void onActionUp()
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
        private void onActionMove(float x, float y)
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
    }
}