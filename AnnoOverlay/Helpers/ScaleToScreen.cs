using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
