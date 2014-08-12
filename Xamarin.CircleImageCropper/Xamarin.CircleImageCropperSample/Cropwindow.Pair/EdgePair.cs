
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

namespace Xamarin.CircleImageCropperSample.Cropwindow.Pair
{
    public class EdgePair
    {
        // Member Variables ////////////////////////////////////////////////////////

        public Edge primary;
        public Edge secondary;

        // Constructor /////////////////////////////////////////////////////////////

        public EdgePair(Edge edge1, Edge edge2)
        {
            primary = edge1;
            secondary = edge2;
        }
    }
}