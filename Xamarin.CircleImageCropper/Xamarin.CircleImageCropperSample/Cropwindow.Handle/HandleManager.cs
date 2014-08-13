using CircleImageCropper.CropWindow.Pair;

namespace CircleImageCropper.Cropwindow.Handle
{
    public class HandleManager
    {
        public static Handle
            TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT, LEFT, TOP, RIGHT, BOTTOM, CENTER;

        static HandleManager()
        {
            TOP_LEFT = new Handle(new CornerHandleHelper(EdgeManager.TOP, EdgeManager.LEFT), HandleType.TOP_LEFT);
            TOP_RIGHT = new Handle(new CornerHandleHelper(EdgeManager.TOP, EdgeManager.RIGHT), HandleType.TOP_RIGHT);
            BOTTOM_LEFT = new Handle(new CornerHandleHelper(EdgeManager.BOTTOM, EdgeManager.LEFT),
                HandleType.BOTTOM_LEFT);
            BOTTOM_RIGHT = new Handle(new CornerHandleHelper(EdgeManager.BOTTOM, EdgeManager.RIGHT),
                HandleType.BOTTOM_RIGHT);
            LEFT = new Handle(new VerticalHandleHelper(EdgeManager.LEFT), HandleType.LEFT);
            TOP = new Handle(new HorizontalHandleHelper(EdgeManager.TOP), HandleType.TOP);
            RIGHT = new Handle(new VerticalHandleHelper(EdgeManager.RIGHT), HandleType.RIGHT);
            BOTTOM = new Handle(new HorizontalHandleHelper(EdgeManager.BOTTOM), HandleType.BOTTOM);
            CENTER = new Handle(new CenterHandleHelper(), HandleType.CENTER);
        }
    }
}