using App.Util;
using Branding.App.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.App.Generators {

  public class BinaryGenerator : IGenerator {
    public int Width { get; init; }
    public int Height { get; init; }
    public string FontName { get; init; }
    public float LowFontSize { get; init; }
    public float HighFontSize { get; init; }
    public Color LowColor { get; init; }
    public Color HighColor { get; init; }
    public int Seed { get; init; }

    public Bitmap Generate() {
      using(var dsp = new Disposer()) {
        var rand = new Random(Seed);
        var chars = new List<char>();

        var outBmp = new Bitmap(Width, Height); // output, don't dispose
        var g = dsp.Add(Graphics.FromImage(outBmp));
        var halfHeight = Height / 2;
        var halfHeightRecip = 1.0 / (double)halfHeight;

        var halfWidth = Width / 2;
        var halfWidthRecip = 1.0 / (double)halfWidth;

        var lineY = (int)(rand.NextDouble() * -60);
        while (lineY < Height) {
          var lineFactor = 1.0 - (Math.Abs(halfHeight - lineY) * halfHeightRecip);

          var fontSizeFactor = 0.75 + (rand.NextDouble() * 0.5);
          var fontSize = (LowFontSize + ((HighFontSize - LowFontSize) * lineFactor)) * fontSizeFactor;
          var font = dsp.Add(new Font(FontName, (int)fontSize));

          var chunkX = (int)(rand.NextDouble() * -160);
          var lineHeight = -1; // this will get set in the loop
          while (chunkX < Width) {
            var chunkFactor = 1.0 - (Math.Abs(halfWidth - chunkX) * halfWidthRecip);

            var colorMultiplier = (chunkFactor * lineFactor);
            var colorFactor = colorMultiplier + ((rand.NextDouble() * 0.4) - 0.2);
            var color = ColorUtil.Lerp(LowColor, HighColor, colorFactor);
            var brush = dsp.Add(new SolidBrush(color));

            int chunkLength = 4 + rand.Next(9);
            int chunkBits = rand.Next();
            chars.Clear();
            for (var i = 0; i < chunkLength; i++)
              chars.Add((chunkBits >> i & 1) == 1 ? '1' : '0');
            var chunkText = string.Concat(chars);

            var chunkSize = g.MeasureString(chunkText, font);
            g.DrawString(chunkText, font, brush, chunkX, lineY);
            chunkX += (int)chunkSize.Width;
            if (lineHeight == -1)
              lineHeight = (int)chunkSize.Height;
          }

          lineY += lineHeight;
        }
        
        return outBmp;
      }

    }

  }

}
