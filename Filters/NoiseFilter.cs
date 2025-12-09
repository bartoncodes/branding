using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Filters {

  public class NoiseFilter : IFilter {
    private Color Range { get; set; }
    private int Block { get; set; }

    public NoiseFilter(Color range, int block = 1) {
      Range = range;
      Block = block;
    }

    public void Apply(Bitmap bmp) {
      var r = new Random();

      var xBlocks = (int)Math.Ceiling((float)bmp.Width / (float)Block);
      var yBlocks = (int)Math.Ceiling((float)bmp.Height / (float)Block);
      using (var g = Graphics.FromImage(bmp)) {
        for (var by = 0; by < yBlocks; by++) {
          for (var bx = 0; bx < xBlocks; bx++) {
            var rf = r.NextDouble();
            var rr = (int)(Range.R * rf);
            var rg = (int)(Range.G * rf);
            var rb = (int)(Range.B * rf);

            // var rr = r.Next(Range.R);
            // var rg = r.Next(Range.G);
            // var rb = r.Next(Range.B);
            var x0 = bx * Block;
            var y0 = by * Block;
            var xn = Math.Min(x0 + Block, bmp.Width);
            var yn = Math.Min(y0 + Block, bmp.Height);
            for(var y = y0; y < yn; y++) {
              for(var x = x0; x < xn; x++) {
                var oldColor = bmp.GetPixel(x, y);
                var newColor = Color.FromArgb(oldColor.A,
                Math.Min(255, oldColor.R + rr),
                Math.Min(255, oldColor.G + rg),
                Math.Min(255, oldColor.B + rb)
                );
                bmp.SetPixel(x, y, newColor);
              }
            }
          }
        }
      }

    }

  }

}
