using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.CircleImageCropperSample.Util;

namespace Xamarin.CircleImageCropperSample.Cropwindow.Pair
{
    public class EdgeAux
    {
        private float mCoordinate;
        public static int MIN_CROP_LENGTH_PX = 40;

        public void setCoordinate(float coordinate)
        {
            mCoordinate = coordinate;
        }


        public void offset(float distance)
        {
            mCoordinate += distance;
        }

        public float getCoordinate()
        {
            return mCoordinate;
        }

        /**
        * Sets the Edge to the given x-y coordinate but also adjusting for snapping
        * to the image bounds and parent view border constraints.
        * 
        * @param x the x-coordinate
        * @param y the y-coordinate
        * @param imageRect the bounding rectangle of the image
        * @param imageSnapRadius the radius (in pixels) at which the edge should
        *            snap to the image
        */
        public void adjustCoordinate(float x, float y, Rect imageRect, float imageSnapRadius, float aspectRatio)
        {
            if (this == EdgeType.LEFT)
            {
                mCoordinate = adjustLeft(x, imageRect, imageSnapRadius, aspectRatio);
            }
            else if (this == EdgeType.TOP)
            {
                mCoordinate = adjustTop(y, imageRect, imageSnapRadius, aspectRatio);
            }
            else if (this == EdgeType.RIGHT)
            {
                mCoordinate = adjustRight(x, imageRect, imageSnapRadius, aspectRatio);
            }
            else if (this == EdgeType.BOTTOM)
            {
                mCoordinate = adjustBottom(y, imageRect, imageSnapRadius, aspectRatio);
            }
        }

        public void adjustCoordinate(float aspectRatio)
        {

            float left = EdgeType.LEFT.getCoordinate();
            float top = EdgeType.TOP.getCoordinate();
            float right = EdgeType.RIGHT.getCoordinate();
            float bottom = EdgeType.BOTTOM.getCoordinate();

            if (this == EdgeType.LEFT)
            {
                mCoordinate = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
            }
            else if (this == EdgeType.TOP)
            {
                mCoordinate = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
            }
            else if (this == EdgeType.RIGHT)
            {
                mCoordinate = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
            }
            else if (this == EdgeType.BOTTOM)
            {
                mCoordinate = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
            }
        }

        private static float adjustLeft(float x, Rect imageRect, float imageSnapRadius, float aspectRatio)
        {

            float resultX = x;

            if (x - imageRect.Left < imageSnapRadius)
                resultX = imageRect.Left;

            else
            {
                // Select the minimum of the three possible values to use
                float resultXHoriz = Single.PositiveInfinity;
                float resultXVert = Single.PositiveInfinity;

                // Checks if the window is too small horizontally
                if (x >= EdgeType.RIGHT.getCoordinate() - MIN_CROP_LENGTH_PX)
                    resultXHoriz = EdgeType.RIGHT.getCoordinate() - MIN_CROP_LENGTH_PX;

                // Checks if the window is too small vertically
                if (((EdgeType.RIGHT.getCoordinate() - x) / aspectRatio) <= MIN_CROP_LENGTH_PX)
                    resultXVert = EdgeType.RIGHT.getCoordinate() - (MIN_CROP_LENGTH_PX * aspectRatio);

                resultX = Math.Min(resultX, Math.Min(resultXHoriz, resultXVert));
            }
            return resultX;
        }

        private static float adjustRight(float x, Rect imageRect, float imageSnapRadius, float aspectRatio)
        {

            float resultX = x;

            // If close to the edge
            if (imageRect.Right - x < imageSnapRadius)
                resultX = imageRect.Right;

            else
            {
                // Select the maximum of the three possible values to use
                float resultXHoriz = float.NegativeInfinity;
                float resultXVert = float.NegativeInfinity;

                // Checks if the window is too small horizontally
                if (x <= EdgeType.LEFT.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX)
                    resultXHoriz = EdgeType.LEFT.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX;

                // Checks if the window is too small vertically
                if (((x - EdgeType.LEFT.getCoordinate()) / aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
                {
                    resultXVert = EdgeType.LEFT.getCoordinate() + (EdgeAux.MIN_CROP_LENGTH_PX * aspectRatio);
                }

                resultX = Math.Max(resultX, Math.Max(resultXHoriz, resultXVert));

            }

            return resultX;
        }

        private static float adjustTop(float y, Rect imageRect, float imageSnapRadius, float aspectRatio)
        {

            float resultY = y;

            if (y - imageRect.Top < imageSnapRadius)
                resultY = imageRect.Top;

            else
            {
                // Select the minimum of the three possible values to use
                float resultYVert = float.PositiveInfinity;
                float resultYHoriz = float.PositiveInfinity;

                // Checks if the window is too small vertically
                if (y >= EdgeType.BOTTOM.getCoordinate() - EdgeAux.MIN_CROP_LENGTH_PX)
                    resultYHoriz = EdgeType.BOTTOM.getCoordinate() - EdgeAux.MIN_CROP_LENGTH_PX;

                // Checks if the window is too small horizontally
                if (((EdgeType.BOTTOM.getCoordinate() - y) * aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
                    resultYVert = EdgeType.BOTTOM.getCoordinate() - (EdgeAux.MIN_CROP_LENGTH_PX / aspectRatio);

                resultY = Math.Min(resultY, Math.Min(resultYHoriz, resultYVert));

            }

            return resultY;
        }

        private static float adjustBottom(float y, Rect imageRect, float imageSnapRadius, float aspectRatio)
        {

            float resultY = y;

            if (imageRect.Bottom - y < imageSnapRadius)
                resultY = imageRect.Bottom;
            else
            {
                // Select the maximum of the three possible values to use
                float resultYVert = float.NegativeInfinity;
                float resultYHoriz = float.NegativeInfinity;

                // Checks if the window is too small vertically
                if (y <= EdgeType.TOP.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX)
                    resultYVert = EdgeType.TOP.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX;

                // Checks if the window is too small horizontally
                if (((y - EdgeType.TOP.getCoordinate()) * aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
                    resultYHoriz = EdgeType.TOP.getCoordinate() + (EdgeAux.MIN_CROP_LENGTH_PX / aspectRatio);

                resultY = Math.Max(resultY, Math.Max(resultYHoriz, resultYVert));
            }

            return resultY;
        }
    }
}