using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDColor = System.Drawing.Color;
using SWMColor = System.Windows.Media.Color;

namespace VisGitCore.Helpers
{
    public static class ColorExt
    {
        public static SWMColor ToMediaColor(this SDColor color) => SWMColor.FromArgb(color.A, color.R, color.G, color.B);

        public static SDColor ToDrawingColor(this SWMColor color) => SDColor.FromArgb(color.A, color.R, color.G, color.B);
    }
}