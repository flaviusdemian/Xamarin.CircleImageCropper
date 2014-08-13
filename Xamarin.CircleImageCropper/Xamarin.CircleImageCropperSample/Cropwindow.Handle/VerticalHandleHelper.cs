using Android.Graphics;
using CircleImageCropper.CropWindow.Pair;
using CircleImageCropper.Util;

namespace CircleImageCropper.Cropwindow.Handle
{
    public class VerticalHandleHelper : HandleHelper
    {
        // Member Variables ////////////////////////////////////////////////////////

        private readonly Edge mEdgeType;

        // Constructor /////////////////////////////////////////////////////////////

        public VerticalHandleHelper(Edge edge)
            : base(null, edge)
        {
            mEdgeType = edge;
        }

        // HandleHelper Methods ////////////////////////////////////////////////////

        public override void UpdateCropWindow(float x,
            float y,
            float targetAspectRatio,
            Rect imageRect,
            float snapRadius)
        {
            // Adjust this EdgeType accordingly.
            mEdgeType.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio);

            float left = EdgeManager.LEFT.coordinate;
            float top = EdgeManager.TOP.coordinate;
            float right = EdgeManager.RIGHT.coordinate;
            float bottom = EdgeManager.BOTTOM.coordinate;

            // After this EdgeType is moved, our crop window is now out of proportion.
            float targetHeight = AspectRatioUtil.calculateHeight(left, right, targetAspectRatio);
            float currentHeight = bottom - top;

            // Adjust the crop window so that it maintains the given aspect ratio by
            // moving the adjacent EdgeTypes symmetrically in or out.
            float difference = targetHeight - currentHeight;
            float halfDifference = difference/2;
            top -= halfDifference;
            bottom += halfDifference;

            EdgeManager.TOP.coordinate = top;
            EdgeManager.BOTTOM.coordinate = bottom;

            // Check if we have gone out of bounds on the top or bottom, and fix.
            if (EdgeManager.TOP.isOutsideMargin(imageRect, snapRadius) &&
                !mEdgeType.isNewRectangleOutOfBounds(EdgeManager.TOP,
                    imageRect,
                    targetAspectRatio))
            {
                float offset = EdgeManager.TOP.snapToRect(imageRect);
                EdgeManager.BOTTOM.offset(-offset);
                mEdgeType.adjustCoordinate(targetAspectRatio);
            }
            if (EdgeManager.BOTTOM.isOutsideMargin(imageRect, snapRadius) &&
                !mEdgeType.isNewRectangleOutOfBounds(EdgeManager.BOTTOM,
                    imageRect,
                    targetAspectRatio))
            {
                float offset = EdgeManager.BOTTOM.snapToRect(imageRect);
                EdgeManager.TOP.offset(-offset);
                mEdgeType.adjustCoordinate(targetAspectRatio);
            }
        }
    }
}