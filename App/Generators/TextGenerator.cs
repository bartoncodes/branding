using App.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.App.Generators {

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
      using(var dsp = new Disposer()) {
        var dumbBmp = dsp.Add(new Bitmap(1, 1));
        SizeF textDims;
        var dumbG = dsp.Add(Graphics.FromImage(dumbBmp));
        textDims = dumbG.MeasureString(Text, Font);

        var bmpWidth = (int)textDims.Width + (Padding * 2);
        var bmpHeight = (int)textDims.Height + (Padding * 2);

        var bmp = new Bitmap(bmpWidth, bmpHeight); // out bmp, do not dispose
        var bmpG = dsp.Add(Graphics.FromImage(bmp));
        bmpG.DrawString(Text, Font, Brush, new PointF(Padding, Padding));
        return bmp;
      }
    }
  }

}
