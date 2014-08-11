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

        public EdgeAux primary;
        public EdgeAux secondary;

        // Constructor /////////////////////////////////////////////////////////////

        public EdgePair(EdgeAux edge1, EdgeAux edge2)
        {
            primary = edge1;
            secondary = edge2;
        }
    }
}