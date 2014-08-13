using Android.Graphics;
using CircleImageCropper.CropWindow.Pair;
using CircleImageCropper.Util;

namespace CircleImageCropper.Cropwindow.Handle
{
    public abstract class HandleHelper
    {
        // Member Variables ////////////////////////////////////////////////////////

        private static float UNFIXED_ASPECT_RATIO_CONSTANT = 1;
        private readonly EdgePair mActiveEdges;
        private readonly Edge mHorizontalEdge;
        private readonly Edge mVerticalEdge;

        // Save the Pair object as a member variable to avoid having to instantiate
        // a new Object every time GetActiveEdges() is called.

        // Constructor /////////////////////////////////////////////////////////////

        /**
         * Constructor.
         * 
         * @param horizontalEdge the horizontal edge associated with this handle;
         *            may be null
         * @param verticalEdge the vertical edge associated with this handle; may be
         *            null
         */

        public HandleHelper(Edge horizontalEdge, Edge verticalEdge)
        {
            mHorizontalEdge = horizontalEdge;
            mVerticalEdge = verticalEdge;
            mActiveEdges = new EdgePair(mHorizontalEdge, mVerticalEdge);
        }

        // Package-Private Methods /////////////////////////////////////////////////

        /**
         * Updates the crop window by directly setting the Edge coordinates.
         * 
         * @param x the new x-coordinate of this handle
         * @param y the new y-coordinate of this handle
         * @param imageRect the bounding rectangle of the image
         * @param parentView the parent View containing the image
         * @param snapRadius the maximum distance (in pixels) at which the crop
         *            window should snap to the image
         */

        public virtual void UpdateCropWindow(float x,
            float y,
            Rect imageRect,
            float snapRadius)
        {
            EdgePair activeEdges = GetActiveEdges();
            Edge primaryEdge = activeEdges.primary;
            Edge secondaryEdge = activeEdges.secondary;

            if (primaryEdge != null)
                primaryEdge.adjustCoordinate(x, y, imageRect, snapRadius, UNFIXED_ASPECT_RATIO_CONSTANT);

            if (secondaryEdge != null)
                secondaryEdge.adjustCoordinate(x, y, imageRect, snapRadius, UNFIXED_ASPECT_RATIO_CONSTANT);
        }

        /**
         * Updates the crop window by directly setting the Edge coordinates; this
         * method maintains a given aspect ratio.
         * 
         * @param x the new x-coordinate of this handle
         * @param y the new y-coordinate of this handle
         * @param targetAspectRatio the aspect ratio to maintain
         * @param imageRect the bounding rectangle of the image
         * @param parentView the parent View containing the image
         * @param snapRadius the maximum distance (in pixels) at which the crop
         *            window should snap to the image
         */

        public abstract void UpdateCropWindow(float x,
            float y,
            float targetAspectRatio,
            Rect imageRect,
            float snapRadius);

        /**
         * Gets the Edges associated with this handle (i.e. the Edges that should be
         * moved when this handle is dragged). This is used when we are not
         * maintaining the aspect ratio.
         * 
         * @return the active edge as a pair (the pair may contain null values for
         *         the <code>primary</code>, <code>secondary</code> or both fields)
         */

        public EdgePair GetActiveEdges()
        {
            return mActiveEdges;
        }

        /**
         * Gets the Edges associated with this handle as an ordered Pair. The
         * <code>primary</code> Edge in the pair is the determining side. This
         * method is used when we need to maintain the aspect ratio.
         * 
         * @param x the x-coordinate of the touch point
         * @param y the y-coordinate of the touch point
         * @param targetAspectRatio the aspect ratio that we are maintaining
         * @return the active edges as an ordered pair
         */

        public EdgePair GetActiveEdges(float x, float y, float targetAspectRatio)
        {
            // Calculate the aspect ratio if this handle were dragged to the given
            // x-y coordinate.
            float potentialAspectRatio = GetAspectRatio(x, y);

            // If the touched point is wider than the aspect ratio, then x
            // is the determining side. Else, y is the determining side.
            if (potentialAspectRatio > targetAspectRatio)
            {
                mActiveEdges.primary = mVerticalEdge;
                mActiveEdges.secondary = mHorizontalEdge;
            }
            else
            {
                mActiveEdges.primary = mHorizontalEdge;
                mActiveEdges.secondary = mVerticalEdge;
            }
            return mActiveEdges;
        }

        // Private Methods /////////////////////////////////////////////////////////

        /**
         * Gets the aspect ratio of the resulting crop window if this handle were
         * dragged to the given point.
         * 
         * @param x the x-coordinate
         * @param y the y-coordinate
         * @return the aspect ratio
         */

        private float GetAspectRatio(float x, float y)
        {
            // Replace the active edge coordinate with the given touch coordinate.
            float left = (mVerticalEdge == EdgeManager.LEFT) ? x : EdgeManager.LEFT.coordinate;
            float top = (mHorizontalEdge == EdgeManager.TOP) ? y : EdgeManager.TOP.coordinate;
            float right = (mVerticalEdge == EdgeManager.RIGHT) ? x : EdgeManager.RIGHT.coordinate;
            float bottom = (mHorizontalEdge == EdgeManager.BOTTOM) ? y : EdgeManager.BOTTOM.coordinate;

            float aspectRatio = AspectRatioUtil.calculateAspectRatio(left, top, right, bottom);

            return aspectRatio;
        }
    }
}