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
    public class Handle
    {

        // Member Variables ////////////////////////////////////////////////////////

        private HandleHelper mHelper;

        // Constructors ////////////////////////////////////////////////////////////

        Handle(HandleHelper helper)
        {
            mHelper = helper;
        }

        // Public Methods //////////////////////////////////////////////////////////

        public void updateCropWindow(float x,
                                     float y,
                                     Rect imageRect,
                                     float snapRadius)
        {

            mHelper.updateCropWindow(x, y, imageRect, snapRadius);
        }

        public void updateCropWindow(float x,
                                     float y,
                                     float targetAspectRatio,
                                     Rect imageRect,
                                     float snapRadius)
        {

            mHelper.updateCropWindow(x, y, targetAspectRatio, imageRect, snapRadius);
        }
    }
}