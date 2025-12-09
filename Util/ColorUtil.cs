using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Util {

  public static class ColorUtil {

    public static Color Lerp(Color a, Color b, double x) {
      x = Math.Min(1.0, Math.Max(0.0, x));
      return Color.FromArgb(
        (int)(a.A + ((b.A - a.A) * x)),
        (int)(a.R + ((b.R - a.R) * x)),
        (int)(a.G + ((b.G - a.G) * x)),
        (int)(a.B + ((b.B - a.B) * x))
      );
    }

  }

}
