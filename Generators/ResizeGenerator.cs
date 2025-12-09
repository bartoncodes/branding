using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Generators {

  public class ResizeGenerator : IGenerator {
    private Bitmap Source { get; init; }
    private double XScale { get; init; }
    private double YScale { get; init; }

    public ResizeGenerator(Bitmap source, double xScale = 1.0, double yScale = 1.0) {
      Source = source;
      XScale = xScale;
      YScale = yScale;
    }

    public Bitmap Generate() {
      var outRect = new Rectangle(0, 0,
        (int)Math.Floor(Source.Width * XScale),
        (int)Math.Floor(Source.Height * YScale)
      );
      var outBmp = new Bitmap(outRect.Width, outRect.Height);
      using (var g = Graphics.FromImage(outBmp)) {
        g.DrawImage(Source, outRect);
      }
      return outBmp;
    }

  }

}
