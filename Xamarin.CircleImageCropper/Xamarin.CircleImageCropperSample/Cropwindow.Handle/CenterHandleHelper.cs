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

namespace Xamarin.CircleImageCropperSample.Cropwindow.Handle
{
    public class CenterHandleHelper : HandleHelper
    {
         // Constructor /////////////////////////////////////////////////////////////

    public CenterHandleHelper() :  base(null, null)
    {
       
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    public override void updateCropWindow(float x,
                          float y,
                          Rect imageRect,
                          float snapRadius) {

        float left = EdgeType.LEFT.getCoordinate();
        float top = EdgeType.TOP.getCoordinate();
        float right = EdgeType.RIGHT.getCoordinate();
        float bottom = EdgeType.BOTTOM.getCoordinate();

         float currentCenterX = (left + right) / 2;
         float currentCenterY = (top + bottom) / 2;

         float offsetX = x - currentCenterX;
         float offsetY = y - currentCenterY;

        // Adjust the crop window.
        EdgeType.LEFT.offset(offsetX);
        EdgeType.TOP.offset(offsetY);
        EdgeType.RIGHT.offset(offsetX);
        EdgeType.BOTTOM.offset(offsetY);

        // Check if we have gone out of bounds on the sides, and fix.
        if (EdgeType.LEFT.isOutsideMargin(imageRect, snapRadius)) {
             float offset = EdgeType.LEFT.snapToRect(imageRect);
            EdgeType.RIGHT.offset(offset);
        } else if (EdgeType.RIGHT.isOutsideMargin(imageRect, snapRadius)) {
             float offset = EdgeType.RIGHT.snapToRect(imageRect);
            EdgeType.LEFT.offset(offset);
        }

        // Check if we have gone out of bounds on the top or bottom, and fix.
        if (EdgeType.TOP.isOutsideMargin(imageRect, snapRadius)) {
             float offset = EdgeType.TOP.snapToRect(imageRect);
            EdgeType.BOTTOM.offset(offset);
        } else if (EdgeType.BOTTOM.isOutsideMargin(imageRect, snapRadius)) {
             float offset = EdgeType.BOTTOM.snapToRect(imageRect);
            EdgeType.TOP.offset(offset);
        }
    }

    public void  updateCropWindow(float x,
                          float y,
                          float targetAspectRatio,
                          Rect imageRect,
                          float snapRadius) {

        updateCropWindow(x, y, imageRect, snapRadius);
    }
    }
}