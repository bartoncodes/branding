using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Branding.App.Generators {

  public class GradientGenerator : IGenerator {
    private int Width { get; set; }
    private int Height { get; set; }
    private LinearGradientBrush Brush { get; set; }

    public GradientGenerator(int width, int height, Point p1, Point p2, Color c1, Color c2) {
      Width = width;
      Height = height;
      Brush = new LinearGradientBrush(p1, p2, c1, c2);
    }

    public Bitmap Generate() {
      var bmp = new Bitmap(Width, Height);
      using (var g = Graphics.FromImage(bmp)) {
        g.FillRectangle(Brush, 0, 0, Width, Height);
      }
      return bmp;
    }
  }

}
