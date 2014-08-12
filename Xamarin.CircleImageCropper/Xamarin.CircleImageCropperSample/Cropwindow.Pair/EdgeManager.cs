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
public class EdgeManager 
{
    // Public Methods //////////////////////////////////////////////////////////
    public static Edge LEFT, TOP, RIGHT, BOTTOM;
    
    static EdgeManager()
    {
      LEFT = new Edge(EdgeType.LEFT);
      TOP = new Edge(EdgeType.TOP);
      RIGHT = new Edge(EdgeType.RIGHT);
      BOTTOM = new Edge(EdgeType.BOTTOM);
    }
}
}