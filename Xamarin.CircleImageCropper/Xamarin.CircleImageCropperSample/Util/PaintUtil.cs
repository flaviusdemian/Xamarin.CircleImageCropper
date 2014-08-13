using System;
using Android.Content;
using Android.Graphics;
using Android.Util;

namespace CircleImageCropper.Util
{
    public class PaintUtil
    {
        // Private Constants ///////////////////////////////////////////////////////

        private static int DEFAULT_CORNER_COLOR = Color.White;
        private static String SEMI_TRANSPARENT = "#AAFFFFFF";
        private static String DEFAULT_BACKGROUND_COLOR_ID = "#B0000000";
        private static float DEFAULT_LINE_THICKNESS_DP = 2;
        private static float DEFAULT_CORNER_THICKNESS_DP = 5;
        private static float DEFAULT_GUIDELINE_THICKNESS_PX = 1;

        // Public Methods //////////////////////////////////////////////////////////

        /**
         * Creates the Paint object for drawing the crop window border.
         * 
         * @param context the Context
         * @return new Paint object
         */

        public static Paint newBorderPaint(Context context)
        {
            // Set the line thickness for the crop window border.
            float lineThicknessPx = TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_LINE_THICKNESS_DP,
                context.Resources.DisplayMetrics);

            var borderPaint = new Paint();
            borderPaint.Color = Color.ParseColor(SEMI_TRANSPARENT);
            borderPaint.StrokeWidth = lineThicknessPx;
            borderPaint.SetStyle(Paint.Style.Stroke);
            borderPaint.AntiAlias = true;

            return borderPaint;
        }

        /**
         * Creates the Paint object for drawing the crop window guidelines.
         * 
         * @return the new Paint object
         */

        public static Paint newGuidelinePaint()
        {
            var paint = new Paint();
            paint.Color = Color.ParseColor(SEMI_TRANSPARENT);
            paint.StrokeWidth = DEFAULT_GUIDELINE_THICKNESS_PX;

            return paint;
        }

        /**
         * Creates the Paint object for drawing the translucent overlay outside the
         * crop window.
         * 
         * @param context the Context
         * @return the new Paint object
         */

        public static Paint newBackgroundPaint(Context context)
        {
            var paint = new Paint();
            paint.Color = Color.ParseColor(DEFAULT_BACKGROUND_COLOR_ID);

            return paint;
        }

        /**
         * Creates the Paint object for drawing the corners of the border
         * 
         * @param context the Context
         * @return the new Paint object
         */

        public static Paint newCornerPaint(Context context)
        {
            // Set the line thickness for the crop window border.
            float lineThicknessPx = TypedValue.ApplyDimension(ComplexUnitType.Dip,
                DEFAULT_CORNER_THICKNESS_DP,
                context.Resources.DisplayMetrics);

            var cornerPaint = new Paint();
            //TODO: FIX
            //cornerPaint.Color = DEFAULT_CORNER_COLOR;
            cornerPaint.StrokeWidth = lineThicknessPx;
            cornerPaint.SetStyle(Paint.Style.Stroke);

            return cornerPaint;
        }

        /**
         * Returns the value of the corner thickness
         * 
         * @return Float equivalent to the corner thickness
         */

        public static float getCornerThickness()
        {
            return DEFAULT_CORNER_THICKNESS_DP;
        }

        /**
         * Returns the value of the line thickness of the border
         * 
         * @return Float equivalent to the line thickness
         */

        public static float getLineThickness()
        {
            return DEFAULT_LINE_THICKNESS_DP;
        }
    }
}