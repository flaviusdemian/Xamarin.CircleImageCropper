using Android.Graphics;
using CircleImageCropper.CropWindow.Pair;
using CircleImageCropper.Util;

namespace CircleImageCropper.Cropwindow.Handle
{
    public class HorizontalHandleHelper : HandleHelper
    {
        // Member Variables ////////////////////////////////////////////////////////

        private readonly Edge mEdge;

        // Constructor /////////////////////////////////////////////////////////////

        public HorizontalHandleHelper(Edge edge) : base(edge, null)
        {
            mEdge = edge;
        }

        // HandleHelper Methods ////////////////////////////////////////////////////

        public override void UpdateCropWindow(float x,
            float y,
            float targetAspectRatio,
            Rect imageRect,
            float snapRadius)
        {
            // Adjust this Edge accordingly.
            mEdge.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio);

            float left = EdgeManager.LEFT.coordinate;
            float top = EdgeManager.TOP.coordinate;
            float right = EdgeManager.RIGHT.coordinate;
            float bottom = EdgeManager.BOTTOM.coordinate;

            // After this Edge is moved, our crop window is now out of proportion.
            float targetWidth = AspectRatioUtil.calculateWidth(top, bottom, targetAspectRatio);
            float currentWidth = right - left;

            // Adjust the crop window so that it maintains the given aspect ratio by
            // moving the adjacent edges symmetrically in or out.
            float difference = targetWidth - currentWidth;
            float halfDifference = difference/2;
            left -= halfDifference;
            right += halfDifference;

            EdgeManager.LEFT.coordinate = left;
            EdgeManager.RIGHT.coordinate = right;

            // Check if we have gone out of bounds on the sides, and fix.
            if (EdgeManager.LEFT.isOutsideMargin(imageRect, snapRadius) &&
                !mEdge.isNewRectangleOutOfBounds(EdgeManager.LEFT,
                    imageRect,
                    targetAspectRatio))
            {
                float offset = EdgeManager.LEFT.snapToRect(imageRect);
                EdgeManager.RIGHT.offset(-offset);
                mEdge.adjustCoordinate(targetAspectRatio);
            }
            if (EdgeManager.RIGHT.isOutsideMargin(imageRect, snapRadius) &&
                !mEdge.isNewRectangleOutOfBounds(EdgeManager.RIGHT,
                    imageRect,
                    targetAspectRatio))
            {
                float offset = EdgeManager.RIGHT.snapToRect(imageRect);
                EdgeManager.LEFT.offset(-offset);
                mEdge.adjustCoordinate(targetAspectRatio);
            }
        }
    }
}