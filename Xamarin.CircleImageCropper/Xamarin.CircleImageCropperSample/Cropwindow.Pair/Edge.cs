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
    public struct Edge
    {

        // Private Constants ///////////////////////////////////////////////////////

        // Minimum distance in pixels that one edge can get to its opposing edge.
        // This is an arbitrary value that simply prevents the crop window from
        // becoming too small.

        // Member Variables ////////////////////////////////////////////////////////
        //private Edge edgeType;
        private float mCoordinate;
        // Public Methods //////////////////////////////////////////////////////////
        private EdgeAux edgeAux;
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
            edgeAux.adjustCoordinate(x, y, imageRect, imageSnapRadius, aspectRatio);
        }

        /**
      * Gets the coordinate of the Edge
      * 
      * @return the Edge coordinate (x-coordinate for LEFT and RIGHT Edges and
      *         the y-coordinate for TOP and BOTTOM edges)
      */
        public float getCoordinate()
        {
            return edgeAux.getCoordinate();
        }

        /**
        * Sets the coordinate of the Edge. The coordinate will represent the
        * x-coordinate for LEFT and RIGHT Edges and the y-coordinate for TOP and
        * BOTTOM edges.
        * 
        * @param coordinate the position of the edge
        */
        public void setCoordinate(float coordinate)
        {
            mCoordinate = coordinate;
        }

        /**
         * Add the given number of pixels to the current coordinate position of this
         * Edge.
         * 
         * @param distance the number of pixels to add
         */
        public void offset(float distance)
        {
            mCoordinate += distance;
        }

        /**
         * Adjusts this Edge position such that the resulting window will have the
         * given aspect ratio.
         * 
         * @param aspectRatio the aspect ratio to achieve
         */
        public void adjustCoordinate(float aspectRatio)
        {
            edgeAux.adjustCoordinate(aspectRatio);
        }

        /**
         * Returns whether or not you can re-scale the image based on whether any edge would be out of bounds.
         * Checks all the edges for a possibility of jumping out of bounds.
         * 
         * @param Edge the Edge that is about to be expanded
         * @param imageRect the rectangle of the picture
         * @param aspectratio the desired aspectRatio of the picture.
         * 
         * @return whether or not the new image would be out of bounds.
         */
        public bool isNewRectangleOutOfBounds(Edge edge, Rect imageRect, float aspectRatio)
        {

            float offset = edge.snapOffset(imageRect);

            switch (this)
            {
                case LEFT:
                    if (edge.Equals(TOP))
                    {
                        float top = imageRect.Top;
                        float bottom = BOTTOM.getCoordinate() - offset;
                        float right = RIGHT.getCoordinate();
                        float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(BOTTOM))
                    {
                        float bottom = imageRect.Bottom;
                        float top = TOP.getCoordinate() - offset;
                        float right = RIGHT.getCoordinate();
                        float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);
                    }
                    break;

                case EdgeType.TOP:
                    if (edge.Equals(EdgeType.LEFT))
                    {
                        float left = imageRect.Left;
                        float right = EdgeType.RIGHT.getCoordinate() - offset;
                        float bottom = EdgeType.BOTTOM.getCoordinate();
                        float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeType.RIGHT))
                    {
                        float right = imageRect.Right;
                        float left = EdgeType.LEFT.getCoordinate() - offset;
                        float bottom = EdgeType.BOTTOM.getCoordinate();
                        float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);
                    }
                    break;

                case EdgeType.RIGHT:
                    if (edge.Equals(EdgeType.TOP))
                    {
                        float top = imageRect.Top;
                        float bottom = EdgeType.BOTTOM.getCoordinate() - offset;
                        float left = EdgeType.LEFT.getCoordinate();
                        float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeType.BOTTOM))
                    {
                        float bottom = imageRect.Bottom;
                        float top = EdgeType.TOP.getCoordinate() - offset;
                        float left = EdgeType.LEFT.getCoordinate();
                        float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);
                    }
                    break;


                case EdgeType.BOTTOM:
                    if (edge.Equals(EdgeType.LEFT))
                    {
                        float left = imageRect.Left;
                        float right = EdgeType.RIGHT.getCoordinate() - offset;
                        float top = EdgeType.TOP.getCoordinate();
                        float bottom = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeType.RIGHT))
                    {
                        float right = imageRect.Right;
                        float left = EdgeType.LEFT.getCoordinate() - offset;
                        float top = EdgeType.TOP.getCoordinate();
                        float bottom = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    break;
            }
            return true;
        }

        /**
         * Returns whether the new rectangle would be out of bounds.
         * 
         * @param top
         * @param left
         * @param bottom
         * @param right
         * @param imageRect the Image to be compared with.
         * @return whether it would be out of bounds
         */
        private bool isOutOfBounds(float top, float left, float bottom, float right, Rect imageRect)
        {
            return (top < imageRect.Top || left < imageRect.Left || bottom > imageRect.Bottom || right > imageRect.Right);
        }

        /**
         * Snap this Edge to the given image boundaries.
         * 
         * @param imageRect the bounding rectangle of the image to snap to
         * @return the amount (in pixels) that this coordinate was changed (i.e. the
         *         new coordinate minus the old coordinate value)
         */
        public float snapToRect(Rect imageRect)
        {

            float oldCoordinate = mCoordinate;

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    mCoordinate = imageRect.Left;
                    break;
                case EdgeType.TOP:
                    mCoordinate = imageRect.Top;
                    break;
                case EdgeType.RIGHT:
                    mCoordinate = imageRect.Right;
                    break;
                case EdgeType.BOTTOM:
                    mCoordinate = imageRect.Bottom;
                    break;
            }

            float offset = mCoordinate - oldCoordinate;
            return offset;
        }

        /**
         * Returns the potential snap offset of snaptoRect, without changing the coordinate.
         * 
         * @param imageRect the bounding rectangle of the image to snap to
         * @return the amount (in pixels) that this coordinate was changed (i.e. the
         *         new coordinate minus the old coordinate value)
         */
        public float snapOffset(Rect imageRect)
        {

            float oldCoordinate = mCoordinate;
            float newCoordinate = oldCoordinate;

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    newCoordinate = imageRect.Left;
                    break;
                case EdgeType.TOP:
                    newCoordinate = imageRect.Top;
                    break;
                case EdgeType.RIGHT:
                    newCoordinate = imageRect.Right;
                    break;
                case EdgeType.BOTTOM:
                    newCoordinate = imageRect.Bottom;
                    break;
            }

            float offset = newCoordinate - oldCoordinate;
            return offset;
        }

        /**
         * Snap this Edge to the given View boundaries.
         * 
         * @param view the View to snap to
         */
        public void snapToView(View view)
        {

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    mCoordinate = 0;
                    break;
                case EdgeType.TOP:
                    mCoordinate = 0;
                    break;
                case EdgeType.RIGHT:
                    mCoordinate = view.Width;
                    break;
                case EdgeType.BOTTOM:
                    mCoordinate = view.Height;
                    break;
            }
        }

        /**
         * Gets the current width of the crop window.
         */
        public static float getWidth()
        {
            return EdgeType.RIGHT.getCoordinate() - EdgeType.LEFT.getCoordinate();
        }

        /**
         * Gets the current height of the crop window.
         */
        public static float getHeight()
        {
            return EdgeType.BOTTOM.getCoordinate() - EdgeType.TOP.getCoordinate();
        }

        /**
         * Determines if this Edge is outside the inner margins of the given bounding
         * rectangle. The margins come inside the actual frame by SNAPRADIUS amount; 
         * therefore, determines if the point is outside the inner "margin" frame.
         * 
         */
        public bool isOutsideMargin(Rect rect, float margin)
        {

            bool result = false;

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    result = mCoordinate - rect.Left < margin;
                    break;
                case EdgeType.TOP:
                    result = mCoordinate - rect.Top < margin;
                    break;
                case EdgeType.RIGHT:
                    result = rect.Right - mCoordinate < margin;
                    break;
                case EdgeType.BOTTOM:
                    result = rect.Bottom - mCoordinate < margin;
                    break;
            }
            return result;
        }

        /**
         * Determines if this Edge is outside the image frame of the given bounding
         * rectangle.
         */
        public bool isOutsideFrame(Rect rect)
        {

            double margin = 0;
            bool result = false;

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    result = mCoordinate - rect.Left < margin;
                    break;
                case EdgeType.TOP:
                    result = mCoordinate - rect.Top < margin;
                    break;
                case EdgeType.RIGHT:
                    result = rect.Right - mCoordinate < margin;
                    break;
                case EdgeType.BOTTOM:
                    result = rect.Bottom - mCoordinate < margin;
                    break;
            }
            return result;
        }


        // Private Methods /////////////////////////////////////////////////////////

        /**
         * Get the resulting x-position of the left edge of the crop window given
         * the handle's position and the image's bounding box and snap radius.
         * 
         * @param x the x-position that the left edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual x-position of the left edge
         */
        //private static float adjustLeft(float x, Rect imageRect, float imageSnapRadius, float aspectRatio)
        //{

        //    float resultX = x;

        //    if (x - imageRect.Left < imageSnapRadius)
        //        resultX = imageRect.Left;

        //    else
        //    {
        //        // Select the minimum of the three possible values to use
        //        float resultXHoriz = float.PositiveInfinity;
        //        float resultXVert = float.PositiveInfinity;

        //        // Checks if the window is too small horizontally
        //        if (x >= EdgeType.RIGHT.getCoordinate() - EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultXHoriz = EdgeType.RIGHT.getCoordinate() - EdgeAux.MIN_CROP_LENGTH_PX;

        //        // Checks if the window is too small vertically
        //        if (((EdgeType.RIGHT.getCoordinate() - x) / aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultXVert = EdgeType.RIGHT.getCoordinate() - (EdgeAux.MIN_CROP_LENGTH_PX * aspectRatio);

        //        resultX = Math.Min(resultX, Math.Min(resultXHoriz, resultXVert));
        //    }
        //    return resultX;
        //}

        /**
         * Get the resulting x-position of the right edge of the crop window given
         * the handle's position and the image's bounding box and snap radius.
         * 
         * @param x the x-position that the right edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual x-position of the right edge
         */
        //private static float adjustRight(float x, Rect imageRect, float imageSnapRadius, float aspectRatio)
        //{

        //    float resultX = x;

        //    // If close to the edge
        //    if (imageRect.Right - x < imageSnapRadius)
        //        resultX = imageRect.Right;

        //    else
        //    {
        //        // Select the maximum of the three possible values to use
        //        float resultXHoriz = float.NegativeInfinity;
        //        float resultXVert = float.NegativeInfinity;

        //        // Checks if the window is too small horizontally
        //        if (x <= EdgeType.LEFT.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultXHoriz = EdgeType.LEFT.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX;

        //        // Checks if the window is too small vertically
        //        if (((x - EdgeType.LEFT.getCoordinate()) / aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
        //        {
        //            resultXVert = EdgeType.LEFT.getCoordinate() + (EdgeAux.MIN_CROP_LENGTH_PX * aspectRatio);
        //        }

        //        resultX = Math.Max(resultX, Math.Max(resultXHoriz, resultXVert));

        //    }

        //    return resultX;
        //}

        /**
         * Get the resulting y-position of the top edge of the crop window given the
         * handle's position and the image's bounding box and snap radius.
         * 
         * @param y the x-position that the top edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual y-position of the top edge
         */
        //private static float adjustTop(float y, Rect imageRect, float imageSnapRadius, float aspectRatio)
        //{

        //    float resultY = y;

        //    if (y - imageRect.Top < imageSnapRadius)
        //        resultY = imageRect.Top;

        //    else
        //    {
        //        // Select the minimum of the three possible values to use
        //        float resultYVert = float.PositiveInfinity;
        //        float resultYHoriz = float.PositiveInfinity;

        //        // Checks if the window is too small vertically
        //        if (y >= EdgeType.BOTTOM.getCoordinate() - EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultYHoriz = EdgeType.BOTTOM.getCoordinate() - EdgeAux.MIN_CROP_LENGTH_PX;

        //        // Checks if the window is too small horizontally
        //        if (((EdgeType.BOTTOM.getCoordinate() - y) * aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultYVert = EdgeType.BOTTOM.getCoordinate() - (EdgeAux.MIN_CROP_LENGTH_PX / aspectRatio);

        //        resultY = Math.Min(resultY, Math.Min(resultYHoriz, resultYVert));

        //    }

        //    return resultY;
        //}

        /**
         * Get the resulting y-position of the bottom edge of the crop window given
         * the handle's position and the image's bounding box and snap radius.
         * 
         * @param y the x-position that the bottom edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual y-position of the bottom edge
         */
        //private static float adjustBottom(float y, Rect imageRect, float imageSnapRadius, float aspectRatio)
        //{

        //    float resultY = y;

        //    if (imageRect.Bottom - y < imageSnapRadius)
        //        resultY = imageRect.Bottom;
        //    else
        //    {
        //        // Select the maximum of the three possible values to use
        //        float resultYVert = float.NegativeInfinity;
        //        float resultYHoriz = float.NegativeInfinity;

        //        // Checks if the window is too small vertically
        //        if (y <= EdgeType.TOP.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultYVert = EdgeType.TOP.getCoordinate() + EdgeAux.MIN_CROP_LENGTH_PX;

        //        // Checks if the window is too small horizontally
        //        if (((y - EdgeType.TOP.getCoordinate()) * aspectRatio) <= EdgeAux.MIN_CROP_LENGTH_PX)
        //            resultYHoriz = EdgeType.TOP.getCoordinate() + (EdgeAux.MIN_CROP_LENGTH_PX / aspectRatio);

        //        resultY = Math.Max(resultY, Math.Max(resultYHoriz, resultYVert));
        //    }

        //    return resultY;
        //}
    }
}