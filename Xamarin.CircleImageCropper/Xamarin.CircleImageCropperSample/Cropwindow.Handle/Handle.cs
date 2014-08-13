using Android.Graphics;

namespace CircleImageCropper.Cropwindow.Handle
{
    public class Handle
    {
        // Member Variables ////////////////////////////////////////////////////////

        private readonly HandleHelper mHelper;
        public int handleType;
        // Constructors ////////////////////////////////////////////////////////////

        public Handle(HandleHelper helper, int handleType)
        {
            mHelper = helper;
            this.handleType = handleType;
        }

        // Public Methods //////////////////////////////////////////////////////////

        public void updateCropWindow(float x,
            float y,
            Rect imageRect,
            float snapRadius)
        {
            mHelper.UpdateCropWindow(x, y, imageRect, snapRadius);
        }

        public void updateCropWindow(float x,
            float y,
            float targetAspectRatio,
            Rect imageRect,
            float snapRadius)
        {
            mHelper.UpdateCropWindow(x, y, targetAspectRatio, imageRect, snapRadius);
        }
    }
}