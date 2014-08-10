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
using Xamarin.CircleImageCropperSample.Cropwindow.Pair;
using Xamarin.CircleImageCropperSample.Util;

namespace Xamarin.CircleImageCropperSample.Cropwindow.Handle
{
    public class VerticalHandleHelper : HandleHelper
    {
        // Member Variables ////////////////////////////////////////////////////////

        private EdgeType mEdgeType;

        // Constructor /////////////////////////////////////////////////////////////

        VerticalHandleHelper(EdgeType EdgeType)
            : base(null, EdgeType)
        {
            mEdgeType = EdgeType;
        }

        // HandleHelper Methods ////////////////////////////////////////////////////

        public override void updateCropWindow(float x,
                              float y,
                              float targetAspectRatio,
                              Rect imageRect,
                              float snapRadius)
        {

            // Adjust this EdgeType accordingly.
            mEdgeType.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio);

            float left = EdgeType.LEFT.getCoordinate();
            float top = EdgeType.TOP.getCoordinate();
            float right = EdgeType.RIGHT.getCoordinate();
            float bottom = EdgeType.BOTTOM.getCoordinate();

            // After this EdgeType is moved, our crop window is now out of proportion.
            float targetHeight = AspectRatioUtil.calculateHeight(left, right, targetAspectRatio);
            float currentHeight = bottom - top;

            // Adjust the crop window so that it maintains the given aspect ratio by
            // moving the adjacent EdgeTypes symmetrically in or out.
            float difference = targetHeight - currentHeight;
            float halfDifference = difference / 2;
            top -= halfDifference;
            bottom += halfDifference;

            EdgeType.TOP.setCoordinate(top);
            EdgeType.BOTTOM.setCoordinate(bottom);

            // Check if we have gone out of bounds on the top or bottom, and fix.
            if (EdgeType.TOP.isOutsideMargin(imageRect, snapRadius) && !mEdgeType.isNewRectangleOutOfBounds(EdgeType.TOP,
                                                                                                    imageRect,
                                                                                                    targetAspectRatio))
            {
                float offset = EdgeType.TOP.snapToRect(imageRect);
                EdgeType.BOTTOM.offset(-offset);
                mEdgeType.adjustCoordinate(targetAspectRatio);
            }
            if (EdgeType.BOTTOM.isOutsideMargin(imageRect, snapRadius) && !mEdgeType.isNewRectangleOutOfBounds(EdgeType.BOTTOM,
                                                                                                       imageRect,
                                                                                                       targetAspectRatio))
            {
                float offset = EdgeType.BOTTOM.snapToRect(imageRect);
                EdgeType.TOP.offset(-offset);
                mEdgeType.adjustCoordinate(targetAspectRatio);
            }
        }
    }
}