using App.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Brand.App.Filters {

  public class DarkHaloFilter : IFilter {
    public int DarkAlpha { get; init; }
    public double JaggedFactor { get; init; }
    public double HaloLayers { get; init; }
    public double HaloPadding { get; init; }
    public int MaxLineHeight { get; init; }

    public void Apply(Bitmap bmp) {
      using(var dsp = new Disposer()) {
        var rand = new Random();

        var halfHeight = bmp.Height / 2;
        var halfHeightRecip = 1.0 / (double)halfHeight;
        var halfWidth = bmp.Width / 2;

        var darkBrush = dsp.Add(new SolidBrush(Color.FromArgb(DarkAlpha, 0, 0, 0)));

        var lhMin = (int)(MaxLineHeight * 0.50);
        var lhRand = MaxLineHeight - lhMin;

        var paddingFactor = 1.0 + HaloPadding;
        var paddingWidth = (int)(bmp.Width * HaloPadding);

        var g = dsp.Add(Graphics.FromImage(bmp));
        var lineY = 0;
        while(lineY < bmp.Height) {
          var lineHeight = rand.Next(lhRand) + lhMin;
          var lineMidY = lineY + (lineHeight / 2);

          var lineFactor = (Math.Abs(halfHeight - lineMidY) * halfHeightRecip) * paddingFactor;
          lineFactor *= lineFactor;

          var baseEdgeX = (int)(halfWidth * lineFactor) + paddingWidth;
          
          for(int i = 0; i < HaloLayers; i++) {
            var edgeFactor = lineFactor + (rand.NextDouble() * JaggedFactor * 2) - JaggedFactor;
            edgeFactor = Math.Pow(edgeFactor, (i + 1));
            baseEdgeX = (int)(baseEdgeX * edgeFactor);
            var paddedEdgeX = baseEdgeX;
            var darkLeftEdgeX = Math.Min(baseEdgeX, halfWidth);
            var darkRightEdgeX = (halfWidth - darkLeftEdgeX) + halfWidth;
            g.FillRectangle(darkBrush, 0, lineY, darkLeftEdgeX, lineHeight);
            g.FillRectangle(darkBrush, darkRightEdgeX, lineY, baseEdgeX, lineHeight);
          }

          lineY += lineHeight;
        }
      }

    }

  }

}
