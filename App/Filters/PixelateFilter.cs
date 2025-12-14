using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Brand.App.Filters {

  public class PixelateFilter : IFilter {
    private int PixWidth { get; set; }
    private int PixHeight { get; set; }
    private InterpolationMode ShrinkMode { get; set; }
    private InterpolationMode GrowMode { get; set; }

    public PixelateFilter(
      int pixWidth,
      int pixHeight,
      InterpolationMode shrinkMode = InterpolationMode.Default,
      InterpolationMode growMode = InterpolationMode.Default
    ) {
      PixWidth = pixWidth;
      PixHeight = pixHeight;
      ShrinkMode = shrinkMode;
      GrowMode = growMode;
    }

    public void Apply(Bitmap bmp) {
      int smallWidth = (int)((float)bmp.Width / (float)PixWidth);
      int smallHeight = (int)((float)bmp.Height / (float)PixHeight);
      var smallBmp = new Bitmap(smallWidth, smallHeight);

      using(var g = Graphics.FromImage(smallBmp)) {
        g.InterpolationMode = ShrinkMode;
        g.DrawImage(bmp, new Rectangle(0, 0, smallWidth, smallHeight));
      }

      using(var g = Graphics.FromImage(bmp)) {
        g.Clear(Color.FromArgb(0, 0, 0, 0));
        g.InterpolationMode = GrowMode;
        g.DrawImage(smallBmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
      }
    }

  }

}
