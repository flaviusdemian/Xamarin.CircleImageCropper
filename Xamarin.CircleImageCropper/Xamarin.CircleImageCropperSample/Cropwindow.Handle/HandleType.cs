using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.CircleImageCropperSample.Cropwindow.Pair;

namespace Xamarin.CircleImageCropperSample.Cropwindow.Handle
{
    public enum HandleType
    {
        TOP_LEFT = new CornerHandleHelper(EdgeType.TOP, EdgeType.LEFT),
        TOP_RIGHT = new CornerHandleHelper(EdgeType.TOP, EdgeType.RIGHT),
        BOTTOM_LEFT = new CornerHandleHelper(EdgeType.BOTTOM, EdgeType.LEFT),
        BOTTOM_RIGHT = new CornerHandleHelper(EdgeType.BOTTOM, EdgeType.RIGHT),
        LEFT = new VerticalHandleHelper(EdgeType.LEFT),
        TOP = new HorizontalHandleHelper(EdgeType.TOP),
        RIGHT = new VerticalHandleHelper(EdgeType.RIGHT),
        BOTTOM = new HorizontalHandleHelper(EdgeType.BOTTOM),
        CENTER = new CenterHandleHelper()
    }
}