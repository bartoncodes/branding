using Branding.App.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.App.Generators {

  public class BcLineGenerator : IGenerator {
    public int Width { get; init; }
    public int NumLayers { get; init; }
    public int MinHeight { get; init; }
    public int MaxHeight { get; init; }
    public Color StartColor { get; init; }
    public Color EndColor { get; init; }

    public Bitmap Generate() {
      var lineBmp = new Bitmap(Width, MaxHeight);
      using (var g = Graphics.FromImage(lineBmp)) {
        var blockX = 0;
        for(var i = 0; i < NumLayers; i++) {
          var blockFactor = (1.0 / NumLayers) * i;
          var blockFactorInverse = 1.0 - blockFactor;
          blockFactor = 1.0 - (blockFactorInverse * blockFactorInverse);
          var blockWidth = (int)(Width * blockFactor) - blockX;
          var blockHeight = (int)(MaxHeight + ((MinHeight - MaxHeight) * blockFactor));
          var blockColor = ColorUtil.Lerp(StartColor, EndColor, blockFactor);
          var blockBrush = new SolidBrush(blockColor);
          var blockY = (MaxHeight - blockHeight) / 2;
          g.FillRectangle(blockBrush, blockX, blockY, blockWidth, blockHeight);
          blockX += blockWidth;
        }
      }
      return lineBmp;
    }
  }

}
