using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CircleImageCropper;
using CircleImageCropper.CropWindow.Pair;
using CircleImageCropper.Util;
using com.edmodo.cropper.cropwindow;
using Edge = CircleImageCropper.CropWindow.Pair.Edge;
using Orientation = Android.Media.Orientation;

namespace com.edmodo.cropper
{
    public class CropImageView : FrameLayout
    {
        // Private Constants ///////////////////////////////////////////////////////
        private static readonly Rect EMPTY_RECT = new Rect();

        // Member Variables ////////////////////////////////////////////////////////

        // Sets the default image guidelines to show when resizing
        public static int DEFAULT_GUIDELINES = 1;
        public static bool DEFAULT_FIXED_ASPECT_RATIO = true;
        public static int DEFAULT_ASPECT_RATIO_X = 1;
        public static int DEFAULT_ASPECT_RATIO_Y = 1;

        private static int DEFAULT_IMAGE_RESOURCE = 0;

        private static String DEGREES_ROTATED = "DEGREES_ROTATED";

        private readonly bool mFixAspectRatio = DEFAULT_FIXED_ASPECT_RATIO;
        private readonly int mGuidelines = DEFAULT_GUIDELINES;
        private readonly int mImageResource = DEFAULT_IMAGE_RESOURCE;
        private int mAspectRatioX = DEFAULT_ASPECT_RATIO_X;
        private int mAspectRatioY = DEFAULT_ASPECT_RATIO_Y;
        private Bitmap mBitmap;
        private CropOverlayView mCropOverlayView;
        private int mDegreesRotated;
        private ImageView mImageView;
        private int mLayoutHeight;
        private int mLayoutWidth;

        // Constructors ////////////////////////////////////////////////////////////

        protected CropImageView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        public CropImageView(Context context)
            : base(context)
        {
            Init(context);
        }

        public CropImageView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init(context);
        }

        public CropImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

            TypedArray ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.CropImageView, 0, 0);
            try
            {
                mGuidelines = ta.GetInteger(Resource.Styleable.CropImageView_guidelines, DEFAULT_GUIDELINES);
                mFixAspectRatio = ta.GetBoolean(Resource.Styleable.CropImageView_fixAspectRatio, DEFAULT_FIXED_ASPECT_RATIO);
                mAspectRatioX = ta.GetInteger(Resource.Styleable.CropImageView_aspectRatioX, DEFAULT_ASPECT_RATIO_X);
                mAspectRatioY = ta.GetInteger(Resource.Styleable.CropImageView_aspectRatioY, DEFAULT_ASPECT_RATIO_Y);
                mImageResource = ta.GetResourceId(Resource.Styleable.CropImageView_imageResource, DEFAULT_IMAGE_RESOURCE);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                ta.Recycle();
            }

            Init(context);
        }

        // View Methods ////////////////////////////////////////////////////////////

        protected override IParcelable OnSaveInstanceState()
        {
            var bundle = new Bundle();
            try
            {
                bundle.PutParcelable("instanceState", base.OnSaveInstanceState());
                bundle.PutInt(DEGREES_ROTATED, mDegreesRotated);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable parcelable)
        {
            try
            {
                if (parcelable is Bundle)
                {
                    var bundle = (Bundle)parcelable;

                    // Fixes the rotation of the image when orientation changes.
                    mDegreesRotated = bundle.GetInt(DEGREES_ROTATED);
                    int tempDegrees = mDegreesRotated;
                    RotateImage(mDegreesRotated);
                    mDegreesRotated = tempDegrees;
                    //TODO: THIS SHOULD WORK, FIX
                    base.OnRestoreInstanceState(bundle.GetParcelable("instanceState").JavaCast<IParcelable>());
                }
                else
                {
                    base.OnRestoreInstanceState(parcelable);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            try
            {
                if (mBitmap != null)
                {
                    Rect bitmapRect = ImageViewUtil.getBitmapRectCenterInside(mBitmap, this);
                    mCropOverlayView.SetBitmapRect(bitmapRect);
                }
                else
                {
                    mCropOverlayView.SetBitmapRect(EMPTY_RECT);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            try
            {
                var widthMode = (int)MeasureSpec.GetMode(widthMeasureSpec);
                int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
                var heightMode = (int)MeasureSpec.GetMode(heightMeasureSpec);
                int heightSize = MeasureSpec.GetSize(heightMeasureSpec);

                if (mBitmap != null)
                {
                    base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

                    // Bypasses a baffling bug when used within a ScrollView, where
                    // heightSize is set to 0.
                    if (heightSize == 0)
                        heightSize = mBitmap.Height;

                    int desiredWidth;
                    int desiredHeight;

                    double viewToBitmapWidthRatio = Double.PositiveInfinity;
                    double viewToBitmapHeightRatio = Double.PositiveInfinity;

                    // Checks if either width or height needs to be fixed
                    if (widthSize < mBitmap.Width)
                    {
                        viewToBitmapWidthRatio = widthSize / (double)mBitmap.Width;
                    }
                    if (heightSize < mBitmap.Height)
                    {
                        viewToBitmapHeightRatio = heightSize / (double)mBitmap.Height;
                    }

                    // If either needs to be fixed, choose smallest ratio and calculate
                    // from there
                    if (viewToBitmapWidthRatio != Double.PositiveInfinity ||
                        viewToBitmapHeightRatio != Double.PositiveInfinity)
                    {
                        if (viewToBitmapWidthRatio <= viewToBitmapHeightRatio)
                        {
                            desiredWidth = widthSize;
                            desiredHeight = (int)(mBitmap.Height * viewToBitmapWidthRatio);
                        }
                        else
                        {
                            desiredHeight = heightSize;
                            desiredWidth = (int)(mBitmap.Width * viewToBitmapHeightRatio);
                        }
                    }

                        // Otherwise, the picture is within frame layout bounds. Desired
                    // width is
                    // simply picture size
                    else
                    {
                        desiredWidth = mBitmap.Width;
                        desiredHeight = mBitmap.Height;
                    }

                    int width = GetOnMeasureSpec(widthMode, widthSize, desiredWidth);
                    int height = GetOnMeasureSpec(heightMode, heightSize, desiredHeight);

                    mLayoutWidth = width;
                    mLayoutHeight = height;

                    Rect bitmapRect = ImageViewUtil.getBitmapRectCenterInside(mBitmap.Width,
                        mBitmap.Height,
                        mLayoutWidth,
                        mLayoutHeight);
                    mCropOverlayView.SetBitmapRect(bitmapRect);

                    // MUST CALL THIS
                    SetMeasuredDimension(mLayoutWidth, mLayoutHeight);
                }
                else
                {
                    mCropOverlayView.SetBitmapRect(EMPTY_RECT);
                    SetMeasuredDimension(widthSize, heightSize);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            try
            {
                base.OnLayout(changed, l, t, r, b);

                if (mLayoutWidth > 0 && mLayoutHeight > 0)
                {
                    // Gets original parameters, and creates the new parameters
                    ViewGroup.LayoutParams origparams = LayoutParameters;
                    origparams.Width = mLayoutWidth;
                    origparams.Height = mLayoutHeight;
                    LayoutParameters = origparams;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        // Public Methods //////////////////////////////////////////////////////////

        /**
         * Returns the integer of the imageResource
         * 
         * @param int the image resource id
         */

        public int getImageResource()
        {
            return mImageResource;
        }

        /**
         * Sets a Bitmap as the content of the CropImageView.
         * 
         * @param bitmap the Bitmap to set
         */

        public void SetImageBitmap(Bitmap bitmap)
        {
            try
            {
                mBitmap = bitmap;
                mImageView.SetImageBitmap(mBitmap);

                if (mCropOverlayView != null)
                {
                    mCropOverlayView.ResetCropOverlayView();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets a Bitmap and initializes the image rotation according to the EXIT data.
         * <p>
         * The EXIF can be retrieved by doing the following:
         * <code>ExifInterface exif = new ExifInterface(path);</code>
         * 
         * @param bitmap the original bitmap to set; if null, this
         * @param exif the EXIF information about this bitmap; may be null
         */

        public void SetImageBitmap(Bitmap bitmap, ExifInterface exif)
        {
            try
            {
                if (bitmap == null)
                {
                    return;
                }

                if (exif == null)
                {
                    SetImageBitmap(bitmap);
                    return;
                }

                var matrix = new Matrix();
                int orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, 1);
                int rotate = -1;
                //TODO CHECK THIS FIX
                switch (orientation)
                {
                    case (int)Orientation.Rotate270:
                        rotate = 270;
                        break;
                    case (int)Orientation.Rotate180:
                        rotate = 180;
                        break;
                    case (int)Orientation.Rotate90:
                        rotate = 90;
                        break;
                }

                if (rotate == -1)
                {
                    SetImageBitmap(bitmap);
                }
                else
                {
                    matrix.PostRotate(rotate);
                    Bitmap rotatedBitmap = Bitmap.CreateBitmap(bitmap,
                        0,
                        0,
                        bitmap.Width,
                        bitmap.Height,
                        matrix,
                        true);
                    SetImageBitmap(rotatedBitmap);
                    bitmap.Recycle();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets a Drawable as the content of the CropImageView.
         * 
         * @param resId the drawable resource ID to set
         */

        public void SetImageResource(int resId)
        {
            if (resId != 0)
            {
                Bitmap bitmap = BitmapFactory.DecodeResource(Resources, resId);
                SetImageBitmap(bitmap);
            }
        }

        /**
         * Gets the cropped image based on the current crop window.
         * 
         * @return a new Bitmap representing the cropped image
         */

        public Bitmap GetCroppedImage()
        {
            try
            {
                Rect displayedImageRect = ImageViewUtil.getBitmapRectCenterInside(mBitmap, mImageView);

                // Get the scale factor between the actual Bitmap dimensions and the
                // displayed dimensions for width.
                float actualImageWidth = mBitmap.Width;
                float displayedImageWidth = displayedImageRect.Width();
                float scaleFactorWidth = actualImageWidth / displayedImageWidth;

                // Get the scale factor between the actual Bitmap dimensions and the
                // displayed dimensions for height.
                float actualImageHeight = mBitmap.Height;
                float displayedImageHeight = displayedImageRect.Height();
                float scaleFactorHeight = actualImageHeight / displayedImageHeight;

                // Get crop window position relative to the displayed image.
                float cropWindowX = EdgeManager.LEFT.coordinate - displayedImageRect.Left;
                float cropWindowY = EdgeManager.TOP.coordinate - displayedImageRect.Top;
                float cropWindowWidth = Edge.getWidth();
                float cropWindowHeight = Edge.getHeight();

                // Scale the crop window position to the actual size of the Bitmap.
                float actualCropX = cropWindowX * scaleFactorWidth;
                float actualCropY = cropWindowY * scaleFactorHeight;
                float actualCropWidth = cropWindowWidth * scaleFactorWidth;
                float actualCropHeight = cropWindowHeight * scaleFactorHeight;

                // Crop the subset from the original Bitmap.
                Bitmap croppedBitmap = Bitmap.CreateBitmap(mBitmap,
                    (int)actualCropX,
                    (int)actualCropY,
                    (int)actualCropWidth,
                    (int)actualCropHeight);

                return croppedBitmap;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return null;
        }

        /**
         * Gets the cropped circle image based on the current crop selection.
         * 
         * @return a new Circular Bitmap representing the cropped image
         */

        public Bitmap GetCroppedCircleImage()
        {
            try
            {
                Bitmap bitmap = GetCroppedImage();
                Bitmap output = Bitmap.CreateBitmap(bitmap.Width,
                    bitmap.Height, Bitmap.Config.Argb8888);
                var canvas = new Canvas(output);
                //TODO: FIX THIS
                //int color = 0xff424242;
                var paint = new Paint();
                var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

                paint.AntiAlias = true;
                canvas.DrawARGB(0, 0, 0, 0);
                //TODO: FIX THIS
                //paint.Color = color;
                canvas.DrawCircle(bitmap.Width / 2, bitmap.Height / 2,
                    bitmap.Width / 2, paint);
                paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                canvas.DrawBitmap(bitmap, rect, rect, paint);
                //Bitmap _bmp = Bitmap.createScaledBitmap(output, 60, 60, false);
                //return _bmp;
                return output;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return null;
        }

        /**
         * Gets the crop window's position relative to the source Bitmap (not the image
         * displayed in the CropImageView).
         * 
         * @return a RectF instance containing cropped area boundaries of the source Bitmap
         */

        public RectF GetActualCropRect()
        {
            try
            {
                Rect displayedImageRect = ImageViewUtil.getBitmapRectCenterInside(mBitmap, mImageView);

                // Get the scale factor between the actual Bitmap dimensions and the
                // displayed dimensions for width.
                float actualImageWidth = mBitmap.Width;
                float displayedImageWidth = displayedImageRect.Width();
                float scaleFactorWidth = actualImageWidth / displayedImageWidth;

                // Get the scale factor between the actual Bitmap dimensions and the
                // displayed dimensions for height.
                float actualImageHeight = mBitmap.Height;
                float displayedImageHeight = displayedImageRect.Height();
                float scaleFactorHeight = actualImageHeight / displayedImageHeight;

                // Get crop window position relative to the displayed image.
                float displayedCropLeft = EdgeManager.LEFT.coordinate - displayedImageRect.Left;
                float displayedCropTop = EdgeManager.TOP.coordinate - displayedImageRect.Top;
                float displayedCropWidth = Edge.getWidth();
                float displayedCropHeight = Edge.getHeight();

                // Scale the crop window position to the actual size of the Bitmap.
                float actualCropLeft = displayedCropLeft * scaleFactorWidth;
                float actualCropTop = displayedCropTop * scaleFactorHeight;
                float actualCropRight = actualCropLeft + displayedCropWidth * scaleFactorWidth;
                float actualCropBottom = actualCropTop + displayedCropHeight * scaleFactorHeight;

                // Correct for floating point errors. Crop rect boundaries should not
                // exceed the source Bitmap bounds.
                actualCropLeft = Math.Max(0f, actualCropLeft);
                actualCropTop = Math.Max(0f, actualCropTop);
                actualCropRight = Math.Min(mBitmap.Width, actualCropRight);
                actualCropBottom = Math.Min(mBitmap.Height, actualCropBottom);

                var actualCropRect = new RectF(actualCropLeft,
                    actualCropTop,
                    actualCropRight,
                    actualCropBottom);

                return actualCropRect;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return null;
        }

        /**
         * Sets whether the aspect ratio is fixed or not; true fixes the aspect ratio, while
         * false allows it to be changed.
         * 
         * @param fixAspectRatio bool that signals whether the aspect ratio should be
         *            maintained.
         */

        public void SetFixedAspectRatio(bool fixAspectRatio)
        {
            try
            {
                mCropOverlayView.SetFixedAspectRatio(fixAspectRatio);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets the guidelines for the CropOverlayView to be either on, off, or to show when
         * resizing the application.
         * 
         * @param guidelines Integer that signals whether the guidelines should be on, off, or
         *            only showing when resizing.
         */

        public void SetGuidelines(int guidelines)
        {
            try
            {
                mCropOverlayView.SetGuidelines(guidelines);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Sets the both the X and Y values of the aspectRatio.
         * 
         * @param aspectRatioX int that specifies the new X value of the aspect ratio
         * @param aspectRatioX int that specifies the new Y value of the aspect ratio
         */

        public void SetAspectRatio(int aspectRatioX, int aspectRatioY)
        {
            try
            {
                mAspectRatioX = aspectRatioX;
                mCropOverlayView.SetAspectRatioX(mAspectRatioX);

                mAspectRatioY = aspectRatioY;
                mCropOverlayView.SetAspectRatioY(mAspectRatioY);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Rotates image by the specified number of degrees clockwise. Cycles from 0 to 360
         * degrees.
         * 
         * @param degrees Integer specifying the number of degrees to rotate.
         */

        public void RotateImage(int degrees)
        {
            var matrix = new Matrix();
            matrix.PostRotate(degrees);
            mBitmap = Bitmap.CreateBitmap(mBitmap, 0, 0, mBitmap.Width, mBitmap.Height, matrix, true);
            SetImageBitmap(mBitmap);

            mDegreesRotated += degrees;
            mDegreesRotated = mDegreesRotated % 360;
        }

        // Private Methods /////////////////////////////////////////////////////////

        private void Init(Context context)
        {
            try
            {
                //LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                LayoutInflater inflater = LayoutInflater.From(context);
                View v = inflater.Inflate(Resource.Layout.crop_image_view, this, true);
                if (v != null)
                {
                    mImageView = v.FindViewById<ImageView>(Resource.Id.ImageView_image);
                    if (mImageView != null)
                    {
                        SetImageResource(mImageResource);
                    }
                    mCropOverlayView = v.FindViewById<CropOverlayView>(Resource.Id.CropOverlayView);
                    if (mCropOverlayView != null)
                    {
                        mCropOverlayView.SetInitialAttributeValues(mGuidelines, mFixAspectRatio, mAspectRatioX, mAspectRatioY);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /**
         * Determines the specs for the onMeasure function. Calculates the width or height
         * depending on the mode.
         * 
         * @param measureSpecMode The mode of the measured width or height.
         * @param measureSpecSize The size of the measured width or height.
         * @param desiredSize The desired size of the measured width or height.
         * @return The  size of the width or height.
         */

        private static int GetOnMeasureSpec(int measureSpecMode, int measureSpecSize, int desiredSize)
        {
            // Measure Width
            int spec;
            if (measureSpecMode == (int)MeasureSpecMode.Exactly)
            {
                // Must be this size
                spec = measureSpecSize;
            }
            else if (measureSpecMode == (int)MeasureSpecMode.AtMost)
            {
                // Can't be bigger than...; match_parent value
                spec = Math.Min(desiredSize, measureSpecSize);
            }
            else
            {
                // Be whatever you want; wrap_content
                spec = desiredSize;
            }

            return spec;
        }
    }
}