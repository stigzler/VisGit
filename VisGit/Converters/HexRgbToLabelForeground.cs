using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VisGit.Converters
{
    internal class HexRgbToLabelForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string colorString = "#FF" + value;
            Color color = (Color)ColorConverter.ConvertFromString(colorString);

            SolidColorBrush brush = new SolidColorBrush(color);

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Color ChangeLightness(Color color, float coef)
        {
            return Color.FromArgb(Byte.MaxValue, (byte)(color.R * coef), (byte)(color.G * coef), (byte)(color.B * coef));
        }

        private const int MinLightness = 1;
        private const int MaxLightness = 10;
        private const float MinLightnessCoef = 1f;
        private const float MaxLightnessCoef = 0.4f;

        public static Color ChangeLightness(Color color, int lightness)
        {
            if (lightness < MinLightness)
                lightness = MinLightness;
            else if (lightness > MaxLightness)
                lightness = MaxLightness;

            float coef = MinLightnessCoef +
              (
                (lightness - MinLightness) *
                  ((MaxLightnessCoef - MinLightnessCoef) / (MaxLightness - MinLightness))
              );

            return Color.FromArgb(color.A, (byte)(color.R * coef), (byte)(color.G * coef),
                (byte)(color.B * coef));
        }
    }
}