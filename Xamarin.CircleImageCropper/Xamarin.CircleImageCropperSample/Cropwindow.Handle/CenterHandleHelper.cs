using Android.Graphics;
using CircleImageCropper.CropWindow.Pair;

namespace CircleImageCropper.Cropwindow.Handle
{
    public class CenterHandleHelper : HandleHelper
    {
        // Constructor /////////////////////////////////////////////////////////////

        public CenterHandleHelper()
            : base(null, null)
        {
        }

        // HandleHelper Methods ////////////////////////////////////////////////////
        public override void UpdateCropWindow(float x,
            float y,
            Rect imageRect,
            float snapRadius)
        {
            float left = EdgeManager.LEFT.coordinate;
            float top = EdgeManager.TOP.coordinate;
            float right = EdgeManager.RIGHT.coordinate;
            float bottom = EdgeManager.BOTTOM.coordinate;

            float currentCenterX = (left + right)/2;
            float currentCenterY = (top + bottom)/2;

            float offsetX = x - currentCenterX;
            float offsetY = y - currentCenterY;

            // Adjust the crop window.
            EdgeManager.LEFT.offset(offsetX);
            EdgeManager.TOP.offset(offsetY);
            EdgeManager.RIGHT.offset(offsetX);
            EdgeManager.BOTTOM.offset(offsetY);

            // Check if we have gone out of bounds on the sides, and fix.
            if (EdgeManager.LEFT.isOutsideMargin(imageRect, snapRadius))
            {
                float offset = EdgeManager.LEFT.snapToRect(imageRect);
                EdgeManager.RIGHT.offset(offset);
            }
            else if (EdgeManager.RIGHT.isOutsideMargin(imageRect, snapRadius))
            {
                float offset = EdgeManager.RIGHT.snapToRect(imageRect);
                EdgeManager.LEFT.offset(offset);
            }

            // Check if we have gone out of bounds on the top or bottom, and fix.
            if (EdgeManager.TOP.isOutsideMargin(imageRect, snapRadius))
            {
                float offset = EdgeManager.TOP.snapToRect(imageRect);
                EdgeManager.BOTTOM.offset(offset);
            }
            else if (EdgeManager.BOTTOM.isOutsideMargin(imageRect, snapRadius))
            {
                float offset = EdgeManager.BOTTOM.snapToRect(imageRect);
                EdgeManager.TOP.offset(offset);
            }
        }

        public override void UpdateCropWindow(float x,
            float y,
            float targetAspectRatio,
            Rect imageRect,
            float snapRadius)
        {
            UpdateCropWindow(x, y, imageRect, snapRadius);
        }
    }
}