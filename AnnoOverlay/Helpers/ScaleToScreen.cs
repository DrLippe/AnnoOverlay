using System.Windows;

namespace AnnoOverlay
{
    public static class Scale
    {
        public static double ToScreenHeight(double relativeSize)
        {
            return SystemParameters.PrimaryScreenHeight / 1080 * relativeSize;
        }
    }
}
