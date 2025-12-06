using Branding.Filters;
using Branding.Generators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Branding.Brand {

  public class BrandRenderer {
    private BrandProfile Profile { get; set; }

    public BrandRenderer(BrandProfile profile) {
      Profile = profile;
    }

    public Bitmap Render() {
      var gradGen = new GradientGenerator(
        Profile.Width,
        Profile.Height,
        new Point(Profile.Width / 2, 0),
        new Point(Profile.Width / 2, Profile.Height),
        Color.FromArgb(255, 48, 48, 48),
        Color.FromArgb(255, 8, 8, 8)
      );
      var bmp = gradGen.Generate();

      var noiseColor = Color.FromArgb(8, 12, 24);
      var noiseFilter = new NoiseFilter(noiseColor, 8); // Block size should depend on total image size
      noiseFilter.Apply(bmp);

      var bgPixelator = new PixelateFilter(12, 12, InterpolationMode.Low, InterpolationMode.NearestNeighbor);
      bgPixelator.Apply(bmp);

      noiseFilter.Apply(bmp);


      var bartonFont = new Font("Consolas", 120);
      var bartonBrush = new SolidBrush(Color.FromArgb(64, 160, 160));
      var bartonTextGen = new TextGenerator(bartonFont, bartonBrush, "Barton", 4);
      var bartonBmp = bartonTextGen.Generate();

      var codesFont = new Font("ByteBounce", 240);
      var codesBrush = new SolidBrush(Color.FromArgb(112, 208, 208));
      var codesTextGen = new TextGenerator(codesFont, codesBrush, "Codes", 4);
      var codesBmp = codesTextGen.Generate();



      var bartonPixelator = new PixelateFilter(4, 6, InterpolationMode.Low, InterpolationMode.NearestNeighbor);
      bartonPixelator.Apply(bartonBmp);

      var bartonNoiseFilter = new NoiseFilter(Color.FromArgb(255, 64, 92, 128), 4);
      bartonNoiseFilter.Apply(bartonBmp);



      var codesPixelator = new PixelateFilter(4, 2, InterpolationMode.Low, InterpolationMode.NearestNeighbor);
      codesPixelator.Apply(codesBmp);

      var codesNoiseFilter = new NoiseFilter(Color.FromArgb(255, 92, 92, 92), 8);
      codesNoiseFilter.Apply(codesBmp);

      



      using (var g = Graphics.FromImage(bmp)) {
        var bartonTextTL = new Point(
          (Profile.Width / 2) - bartonBmp.Width,
          (Profile.Height / 2) - (bartonBmp.Height / 2)
        );
        var codesTextTL = new Point(
          (Profile.Width / 2),
          (Profile.Height / 2) - (codesBmp.Height / 2)
        );

        g.DrawImage(bartonBmp, bartonTextTL);
        g.DrawImage(codesBmp, codesTextTL);

      }

      return bmp;
    }

  }

}
