using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Xamarin.CircleImageCropperSample.Cropwindow.Pair;
using Xamarin.CircleImageCropperSample.Util;

namespace Xamarin.CircleImageCropperSample.Cropwindow.Handle
{
    public class HorizontalHandleHelper : HandleHelper
    {
          // Member Variables ////////////////////////////////////////////////////////

    private Edge mEdge;

    // Constructor /////////////////////////////////////////////////////////////

    public HorizontalHandleHelper(Edge edge) :  base(edge, null)
    {
        mEdge = edge;
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    public override void updateCropWindow(float x,
                          float y,
                          float targetAspectRatio,
                          Rect imageRect,
                          float snapRadius) {

        // Adjust this Edge accordingly.
        mEdge.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio);

        float left = EdgeType.LEFT.getCoordinate();
        float top = EdgeType.TOP.getCoordinate();
        float right = EdgeType.RIGHT.getCoordinate();
        float bottom = EdgeType.BOTTOM.getCoordinate();

        // After this Edge is moved, our crop window is now out of proportion.
         float targetWidth = AspectRatioUtil.calculateWidth(top, bottom, targetAspectRatio);
         float currentWidth = right - left;

        // Adjust the crop window so that it maintains the given aspect ratio by
        // moving the adjacent edges symmetrically in or out.
         float difference = targetWidth - currentWidth;
         float halfDifference = difference / 2;
        left -= halfDifference;
        right += halfDifference;

        EdgeType.LEFT.setCoordinate(left);
        EdgeType.RIGHT.setCoordinate(right);

        // Check if we have gone out of bounds on the sides, and fix.
        if (EdgeType.LEFT.isOutsideMargin(imageRect, snapRadius) && !mEdge.isNewRectangleOutOfBounds(EdgeType.LEFT,
                                                                                                 imageRect,
                                                                                                 targetAspectRatio)) {
             float offset = EdgeType.LEFT.snapToRect(imageRect);
            EdgeType.RIGHT.offset(-offset);
            mEdge.adjustCoordinate(targetAspectRatio);

        }
        if (EdgeType.RIGHT.isOutsideMargin(imageRect, snapRadius) && !mEdge.isNewRectangleOutOfBounds(EdgeType.RIGHT,
                                                                                                  imageRect,
                                                                                                  targetAspectRatio)) {
             float offset = EdgeType.RIGHT.snapToRect(imageRect);
            EdgeType.LEFT.offset(-offset);
            mEdge.adjustCoordinate(targetAspectRatio);
        }
    }
    }
}