using System;
using Android.Graphics;
using Android.Views;

namespace CircleImageCropper.Util
{
    public class ImageViewUtil
    {
        /**
    * Gets the rectangular position of a Bitmap if it were placed inside a View
    * with scale type set to {@link ImageView#ScaleType #CENTER_INSIDE}.
    * 
    * @param bitmap the Bitmap
    * @param view the parent View of the Bitmap
    * @return the rectangular position of the Bitmap
    */

        public static Rect getBitmapRectCenterInside(Bitmap bitmap, View view)
        {
            int bitmapWidth = bitmap.Width;
            int bitmapHeight = bitmap.Height;
            int viewWidth = view.Width;
            int viewHeight = view.Height;

            return getBitmapRectCenterInsideHelper(bitmapWidth, bitmapHeight, viewWidth, viewHeight);
        }

        /**
         * Gets the rectangular position of a Bitmap if it were placed inside a View
         * with scale type set to {@link ImageView#ScaleType #CENTER_INSIDE}.
         * 
         * @param bitmapWidth the Bitmap's width
         * @param bitmapHeight the Bitmap's height
         * @param viewWidth the parent View's width
         * @param viewHeight the parent View's height
         * @return the rectangular position of the Bitmap
         */

        public static Rect getBitmapRectCenterInside(int bitmapWidth,
            int bitmapHeight,
            int viewWidth,
            int viewHeight)
        {
            return getBitmapRectCenterInsideHelper(bitmapWidth, bitmapHeight, viewWidth, viewHeight);
        }

        /**
         * Helper that does the work of the above functions. Gets the rectangular
         * position of a Bitmap if it were placed inside a View with scale type set
         * to {@link ImageView#ScaleType #CENTER_INSIDE}.
         * 
         * @param bitmapWidth the Bitmap's width
         * @param bitmapHeight the Bitmap's height
         * @param viewWidth the parent View's width
         * @param viewHeight the parent View's height
         * @return the rectangular position of the Bitmap
         */

        private static Rect getBitmapRectCenterInsideHelper(int bitmapWidth,
            int bitmapHeight,
            int viewWidth,
            int viewHeight)
        {
            double resultWidth;
            double resultHeight;
            int resultX;
            int resultY;

            double viewToBitmapWidthRatio = Double.PositiveInfinity;
            double viewToBitmapHeightRatio = Double.PositiveInfinity;

            // Checks if either width or height needs to be fixed
            if (viewWidth < bitmapWidth)
            {
                viewToBitmapWidthRatio = viewWidth/(double) bitmapWidth;
            }
            if (viewHeight < bitmapHeight)
            {
                viewToBitmapHeightRatio = viewHeight/(double) bitmapHeight;
            }

            // If either needs to be fixed, choose smallest ratio and calculate from
            // there
            if (viewToBitmapWidthRatio != Double.PositiveInfinity || viewToBitmapHeightRatio != Double.PositiveInfinity)
            {
                if (viewToBitmapWidthRatio <= viewToBitmapHeightRatio)
                {
                    resultWidth = viewWidth;
                    resultHeight = (bitmapHeight*resultWidth/bitmapWidth);
                }
                else
                {
                    resultHeight = viewHeight;
                    resultWidth = (bitmapWidth*resultHeight/bitmapHeight);
                }
            }
                // Otherwise, the picture is within frame layout bounds. Desired width
                // is simply picture size
            else
            {
                resultHeight = bitmapHeight;
                resultWidth = bitmapWidth;
            }

            // Calculate the position of the bitmap inside the ImageView.
            if (resultWidth == viewWidth)
            {
                resultX = 0;
                resultY = (int) Math.Round((viewHeight - resultHeight)/2);
            }
            else if (resultHeight == viewHeight)
            {
                resultX = (int) Math.Round((viewWidth - resultWidth)/2);
                resultY = 0;
            }
            else
            {
                resultX = (int) Math.Round((viewWidth - resultWidth)/2);
                resultY = (int) Math.Round((viewHeight - resultHeight)/2);
            }

            var result = new Rect(resultX,
                resultY,
                resultX + (int) Math.Ceiling(resultWidth),
                resultY + (int) Math.Ceiling(resultHeight));

            return result;
        }
    }
}