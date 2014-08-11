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
    public class HandleType
    {
        public static HandleHelper TOP_LEFT = new CornerHandleHelper(EdgeType.TOP, EdgeType.LEFT);
        public static HandleHelper TOP_RIGHT = new CornerHandleHelper(EdgeType.TOP, EdgeType.RIGHT);
        public static HandleHelper BOTTOM_LEFT = new CornerHandleHelper(EdgeType.BOTTOM, EdgeType.LEFT);
        public static HandleHelper BOTTOM_RIGHT = new CornerHandleHelper(EdgeType.BOTTOM, EdgeType.RIGHT);
        public static HandleHelper LEFT = new VerticalHandleHelper(EdgeType.LEFT);
        public static HandleHelper TOP = new HorizontalHandleHelper(EdgeType.TOP);
        public static HandleHelper RIGHT = new VerticalHandleHelper(EdgeType.RIGHT);
        public static HandleHelper BOTTOM = new HorizontalHandleHelper(EdgeType.BOTTOM);
        public static HandleHelper CENTER = new CenterHandleHelper();
    }
}