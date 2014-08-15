using Android.Graphics;
using Java.Lang;

namespace CircleImageCropper.Util
{
    public class AspectRatioUtil
    {
        /**
    * Calculates the aspect ratio given a rectangle.
    */

        public static float calculateAspectRatio(float left, float top, float right, float bottom)
        {
            float width = right - left;
            float height = bottom - top;
            float aspectRatio = width / height;

            return aspectRatio;
        }

        /**
         * Calculates the aspect ratio given a rectangle.
         */

        public static float calculateAspectRatio(Rect rect)
        {
            try
            {
                float aspectRatio = rect.Width()/(float) rect.Height();

                return aspectRatio;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return 5;
        }

        /**
         * Calculates the x-coordinate of the left edge given the other sides of the
         * rectangle and an aspect ratio.
         */

        public static float calculateLeft(float top, float right, float bottom, float targetAspectRatio)
        {
            float height = bottom - top;
            // targetAspectRatio = width / height
            // width = targetAspectRatio * height
            // right - left = targetAspectRatio * height
            float left = right - (targetAspectRatio * height);

            return left;
        }

        /**
         * Calculates the y-coordinate of the top edge given the other sides of the
         * rectangle and an aspect ratio.
         */

        public static float calculateTop(float left, float right, float bottom, float targetAspectRatio)
        {
            float width = right - left;
            // targetAspectRatio = width / height
            // width = targetAspectRatio * height
            // height = width / targetAspectRatio
            // bottom - top = width / targetAspectRatio
            float top = bottom - (width / targetAspectRatio);

            return top;
        }

        /**
         * Calculates the x-coordinate of the right edge given the other sides of
         * the rectangle and an aspect ratio.
         */

        public static float calculateRight(float left, float top, float bottom, float targetAspectRatio)
        {
            float height = bottom - top;
            // targetAspectRatio = width / height
            // width = targetAspectRatio * height
            // right - left = targetAspectRatio * height
            float right = (targetAspectRatio * height) + left;

            return right;
        }

        /**
         * Calculates the y-coordinate of the bottom edge given the other sides of
         * the rectangle and an aspect ratio.
         */

        public static float calculateBottom(float left, float top, float right, float targetAspectRatio)
        {
            float width = right - left;
            // targetAspectRatio = width / height
            // width = targetAspectRatio * height
            // height = width / targetAspectRatio
            // bottom - top = width / targetAspectRatio
            float bottom = (width / targetAspectRatio) + top;

            return bottom;
        }

        /**
         * Calculates the width of a rectangle given the top and bottom edges and an
         * aspect ratio.
         */

        public static float calculateWidth(float top, float bottom, float targetAspectRatio)
        {
            float height = bottom - top;
            float width = targetAspectRatio * height;

            return width;
        }

        /**
         * Calculates the height of a rectangle given the left and right edges and
         * an aspect ratio.
         */

        public static float calculateHeight(float left, float right, float targetAspectRatio)
        {
            float width = right - left;
            float height = width / targetAspectRatio;

            return height;
        }
    }
}