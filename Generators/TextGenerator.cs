using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Generators {

  public class TextGenerator : IGenerator {
    private Font Font { get; set; }
    private Brush Brush { get; set; }
    private string Text { get; set; }
    private int Padding { get; set; }

    public TextGenerator(Font font, Brush brush, string text, int padding) {
      Font = font;
      Brush = brush;
      Text = text;
      Padding = padding;
    }

    public Bitmap Generate() {
      var dumbBmp = new Bitmap(1, 1);
      SizeF textDims;
      using (var g = Graphics.FromImage(dumbBmp)) {
        textDims = g.MeasureString(Text, Font);
      }

      var bmpWidth = (int)textDims.Width + (Padding * 2);
      var bmpHeight = (int)textDims.Height + (Padding * 2);

      var bmp = new Bitmap(bmpWidth, bmpHeight);

      using (var g = Graphics.FromImage(bmp)) {
        g.DrawString(Text, Font, Brush, new PointF(Padding, Padding));
      }

      return bmp;
    }
  }

}
