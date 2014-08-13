using System;
using Android.Graphics;
using Android.Views;
using CircleImageCropper.Util;

namespace CircleImageCropper.CropWindow.Pair
{
    public class Edge
    {

        // Private Constants ///////////////////////////////////////////////////////

        // Minimum distance in pixels that one edge can get to its opposing edge.
        // This is an arbitrary value that simply prevents the crop window from
        // becoming too small.
        // Minimum distance in pixels that one edge can get to its opposing edge.
        // This is an arbitrary value that simply prevents the crop window from
        // becoming too small.
        public const int MIN_CROP_LENGTH_PX = 40;
        // Member Variables ////////////////////////////////////////////////////////
        //private Edge edgeType;
        public float coordinate;

        public int edgeType;
        // Public Methods //////////////////////////////////////////////////////////

        public Edge(int edgeType)
        {
            this.edgeType = edgeType;
        }

        /**
 * Add the given number of pixels to the current coordinate position of this
 * Edge.
 * 
 * @param distance the number of pixels to add
 */
        public void offset(float distance)
        {
            coordinate += distance;
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

            switch (this.edgeType)
            {
                case EdgeType.LEFT:
                    coordinate = adjustLeft(x, imageRect, imageSnapRadius, aspectRatio);
                    break;
                case EdgeType.TOP:
                    coordinate = adjustTop(y, imageRect, imageSnapRadius, aspectRatio);
                    break;
                case EdgeType.RIGHT:
                    coordinate = adjustRight(x, imageRect, imageSnapRadius, aspectRatio);
                    break;
                case EdgeType.BOTTOM:
                    coordinate = adjustBottom(y, imageRect, imageSnapRadius, aspectRatio);
                    break;
            }
        }


        /**
            * Adjusts this Edge position such that the resulting window will have the
            * given aspect ratio.
            * 
            * @param aspectRatio the aspect ratio to achieve
            */
        public void adjustCoordinate(float aspectRatio)
        {
            float left = EdgeManager.LEFT.coordinate;
            float top = EdgeManager.TOP.coordinate;
            float right = EdgeManager.RIGHT.coordinate;
            float bottom = EdgeManager.BOTTOM.coordinate;

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    coordinate = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                    break;
                case EdgeType.TOP:
                    coordinate = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                    break;
                case EdgeType.RIGHT:
                    coordinate = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                    break;
                case EdgeType.BOTTOM:
                    coordinate = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
                    break;
            }
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

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    if (edge.Equals(EdgeManager.TOP))
                    {
                        float top = imageRect.Top;
                        float bottom = EdgeManager.BOTTOM.coordinate - offset;
                        float right = EdgeManager.RIGHT.coordinate;
                        float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeManager.BOTTOM))
                    {
                        float bottom = imageRect.Bottom;
                        float top = EdgeManager.TOP.coordinate - offset;
                        float right = EdgeManager.RIGHT.coordinate;
                        float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);
                    }
                    break;

                case EdgeType.TOP:
                    if (edge.Equals(EdgeManager.LEFT))
                    {
                        float left = imageRect.Left;
                        float right = EdgeManager.RIGHT.coordinate - offset;
                        float bottom = EdgeManager.BOTTOM.coordinate;
                        float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeManager.RIGHT))
                    {
                        float right = imageRect.Right;
                        float left = EdgeManager.LEFT.coordinate - offset;
                        float bottom = EdgeManager.BOTTOM.coordinate;
                        float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);
                    }
                    break;

                case EdgeType.RIGHT:
                    if (edge.Equals(EdgeManager.TOP))
                    {
                        float top = imageRect.Top;
                        float bottom = EdgeManager.BOTTOM.coordinate - offset;
                        float left = EdgeManager.LEFT.coordinate;
                        float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeManager.BOTTOM))
                    {
                        float bottom = imageRect.Bottom;
                        float top = EdgeManager.TOP.coordinate - offset;
                        float left = EdgeManager.LEFT.coordinate;
                        float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);
                    }
                    break;


                case EdgeType.BOTTOM:
                    if (edge.Equals(EdgeManager.LEFT))
                    {
                        float left = imageRect.Left;
                        float right = EdgeManager.RIGHT.coordinate - offset;
                        float top = EdgeManager.TOP.coordinate;
                        float bottom = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);

                        return isOutOfBounds(top, left, bottom, right, imageRect);

                    }
                    else if (edge.Equals(EdgeManager.RIGHT))
                    {
                        float right = imageRect.Right;
                        float left = EdgeManager.LEFT.coordinate - offset;
                        float top = EdgeManager.TOP.coordinate;
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

            float oldCoordinate = coordinate;

            switch (edgeType)
            {
                case EdgeType.LEFT:
                    coordinate = imageRect.Left;
                    break;
                case EdgeType.TOP:
                    coordinate = imageRect.Top;
                    break;
                case EdgeType.RIGHT:
                    coordinate = imageRect.Right;
                    break;
                case EdgeType.BOTTOM:
                    coordinate = imageRect.Bottom;
                    break;
            }

            float offset = coordinate - oldCoordinate;
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

            float oldCoordinate = coordinate;
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
                    coordinate = 0;
                    break;
                case EdgeType.TOP:
                    coordinate = 0;
                    break;
                case EdgeType.RIGHT:
                    coordinate = view.Width;
                    break;
                case EdgeType.BOTTOM:
                    coordinate = view.Height;
                    break;
            }
        }

        /**
         * Gets the current width of the crop window.
         */
        public static float getWidth()
        {
            return EdgeManager.RIGHT.coordinate - EdgeManager.LEFT.coordinate;
        }

        /**
         * Gets the current height of the crop window.
         */
        public static float getHeight()
        {
            return EdgeManager.BOTTOM.coordinate - EdgeManager.TOP.coordinate;
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
                    result = coordinate - rect.Left < margin;
                    break;
                case EdgeType.TOP:
                    result = coordinate - rect.Top < margin;
                    break;
                case EdgeType.RIGHT:
                    result = rect.Right - coordinate < margin;
                    break;
                case EdgeType.BOTTOM:
                    result = rect.Bottom - coordinate < margin;
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
                    result = coordinate - rect.Left < margin;
                    break;
                case EdgeType.TOP:
                    result = coordinate - rect.Top < margin;
                    break;
                case EdgeType.RIGHT:
                    result = rect.Right - coordinate < margin;
                    break;
                case EdgeType.BOTTOM:
                    result = rect.Bottom - coordinate < margin;
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
        private static float adjustLeft(float x, Rect imageRect, float imageSnapRadius, float aspectRatio)
        {

            float resultX = x;

            if (x - imageRect.Left < imageSnapRadius)
                resultX = imageRect.Left;

            else
            {
                // Select the minimum of the three possible values to use
                float resultXHoriz = float.PositiveInfinity;
                float resultXVert = float.PositiveInfinity;

                // Checks if the window is too small horizontally
                if (x >= EdgeManager.RIGHT.coordinate - MIN_CROP_LENGTH_PX)
                    resultXHoriz = EdgeManager.RIGHT.coordinate - MIN_CROP_LENGTH_PX;

                // Checks if the window is too small vertically
                if (((EdgeManager.RIGHT.coordinate - x) / aspectRatio) <= MIN_CROP_LENGTH_PX)
                    resultXVert = EdgeManager.RIGHT.coordinate - (MIN_CROP_LENGTH_PX * aspectRatio);

                resultX = Math.Min(resultX, Math.Min(resultXHoriz, resultXVert));
            }
            return resultX;
        }

        /**
         * Get the resulting x-position of the right edge of the crop window given
         * the handle's position and the image's bounding box and snap radius.
         * 
         * @param x the x-position that the right edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual x-position of the right edge
         */
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
                if (x <= EdgeManager.LEFT.coordinate + MIN_CROP_LENGTH_PX)
                    resultXHoriz = EdgeManager.LEFT.coordinate + MIN_CROP_LENGTH_PX;

                // Checks if the window is too small vertically
                if (((x - EdgeManager.LEFT.coordinate) / aspectRatio) <= MIN_CROP_LENGTH_PX)
                {
                    resultXVert = EdgeManager.LEFT.coordinate + (MIN_CROP_LENGTH_PX * aspectRatio);
                }

                resultX = Math.Max(resultX, Math.Max(resultXHoriz, resultXVert));

            }

            return resultX;
        }

        /**
         * Get the resulting y-position of the top edge of the crop window given the
         * handle's position and the image's bounding box and snap radius.
         * 
         * @param y the x-position that the top edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual y-position of the top edge
         */
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
                if (y >= EdgeManager.BOTTOM.coordinate - MIN_CROP_LENGTH_PX)
                    resultYHoriz = EdgeManager.BOTTOM.coordinate - MIN_CROP_LENGTH_PX;

                // Checks if the window is too small horizontally
                if (((EdgeManager.BOTTOM.coordinate - y) * aspectRatio) <= MIN_CROP_LENGTH_PX)
                    resultYVert = EdgeManager.BOTTOM.coordinate - (MIN_CROP_LENGTH_PX / aspectRatio);

                resultY = Math.Min(resultY, Math.Min(resultYHoriz, resultYVert));

            }

            return resultY;
        }

        /**
         * Get the resulting y-position of the bottom edge of the crop window given
         * the handle's position and the image's bounding box and snap radius.
         * 
         * @param y the x-position that the bottom edge is dragged to
         * @param imageRect the bounding box of the image that is being cropped
         * @param imageSnapRadius the snap distance to the image edge (in pixels)
         * @return the actual y-position of the bottom edge
         */
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
                if (y <= EdgeManager.TOP.coordinate + MIN_CROP_LENGTH_PX)
                    resultYVert = EdgeManager.TOP.coordinate + MIN_CROP_LENGTH_PX;

                // Checks if the window is too small horizontally
                if (((y - EdgeManager.TOP.coordinate) * aspectRatio) <= MIN_CROP_LENGTH_PX)
                    resultYHoriz = EdgeManager.TOP.coordinate + (MIN_CROP_LENGTH_PX / aspectRatio);

                resultY = Math.Max(resultY, Math.Max(resultYHoriz, resultYVert));
            }

            return resultY;
        }
    }
}