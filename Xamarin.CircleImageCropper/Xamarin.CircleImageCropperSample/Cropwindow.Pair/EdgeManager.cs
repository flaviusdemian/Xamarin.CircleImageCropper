namespace CircleImageCropper.CropWindow.Pair
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