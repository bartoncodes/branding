using Branding.Filters;
using Branding.Generators;
using Branding.Util;
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

      var fullBmp = new Bitmap(Profile.Width, Profile.Height);
      using(var g = Graphics.FromImage(fullBmp)) {
        g.Clear(Color.Black);
      }

      // Background Noise

      var noiseColor = ColorUtil.Lerp(Profile.Colors.Grit, Color.Black, 0.9);

      for(var i = 3; i <= 8; i++) {
        var blockSize = (int)Math.Pow(2, i);
        var noiseFilter = new NoiseFilter(noiseColor, blockSize); // Block size should depend on total image size
        noiseFilter.Apply(fullBmp);
      }

      // Binary Stuff

      var binaryGen = new BinaryGenerator() {
        Width = fullBmp.Width,
        Height = fullBmp.Height,
        FontName = "ByteBounce",
        LowFontSize = 30f, // Profile.Height * 0.025f,
        HighFontSize = 60f, // Profile.Height * 0.05f,
        LowColor = ColorUtil.Lerp(Profile.Colors.Grit, Color.Black, 0.75),
        HighColor = ColorUtil.Lerp(Profile.Colors.Texture, Color.Black, 0.50),
        Seed = 0
      };
      var binaryBmp = binaryGen.Generate();
      using(var g = Graphics.FromImage(fullBmp)) {
        g.DrawImage(binaryBmp, 0, 0);
      }


      // Dark Halo Stuff

      var darkHaloFilter = new DarkHaloFilter() {
        DarkAlpha = 128,
        JaggedFactor = 0.15,
        HaloLayers = 3,
        HaloPadding = 0.15,
        MaxLineHeight = Math.Max(10, (int)(Profile.Height / 64))
      };



      // Text Stuff

      var bartonFontSize = (int)(Profile.AreaOfInterest.Height * 0.50);
      var bartonFont = new Font("ByteBounce", bartonFontSize);
      var bartonBrush = new SolidBrush(Profile.Colors.Base);
      var bartonTextGen = new TextGenerator(bartonFont, bartonBrush, "Barton", 4);
      var bartonBmp = bartonTextGen.Generate();

      var resizeGen = new ResizeGenerator(bartonBmp, 0.75, 1.0);
      bartonBmp = resizeGen.Generate();

      var codesFontHeight = (int)(Profile.AreaOfInterest.Height * 0.65);
      var codesFont = new Font("ByteBounce", codesFontHeight);
      var codesBrush = new SolidBrush(Profile.Colors.Strong);
      var codesTextGen = new TextGenerator(codesFont, codesBrush, "Codes", 4);
      var codesBmp = codesTextGen.Generate();


      // Text Effects

      var bartonPixelator = new PixelateFilter(4, 6, InterpolationMode.Low, InterpolationMode.NearestNeighbor);
      bartonPixelator.Apply(bartonBmp);

      var bartonNoiseColor = ColorUtil.Lerp(ColorUtil.Lerp(Profile.Colors.Base, Profile.Colors.Highlight, 0.33), Color.Black, 0.50);
      var bartonNoiseFilter = new NoiseFilter(bartonNoiseColor, 4);
      bartonNoiseFilter.Apply(bartonBmp);


      var codesPixelator = new PixelateFilter(4, 2, InterpolationMode.Low, InterpolationMode.NearestNeighbor);
      codesPixelator.Apply(codesBmp);

      var codesNoiseColor = ColorUtil.Lerp(ColorUtil.Lerp(Profile.Colors.Strong, Profile.Colors.Highlight, 0.50), Color.Black, 0.50);
      var codesNoiseFilter = new NoiseFilter(codesNoiseColor, 8);
      codesNoiseFilter.Apply(codesBmp);


      // Line Stuff
      var linesExtraWidth = bartonBmp.Width / 2;
      var linesMinHeight = (int)(bartonBmp.Height * 0.16);
      var linesMaxHeight = (int)(bartonBmp.Height * 0.16);
      var linesNumLayers = 5;

      var lineNoiseColor = ColorUtil.Lerp(Profile.Colors.Texture, Color.Black, 0.80);
      // var lineNoiseColor = Profile.Colors.Texture;
      var lineNoise = new NoiseFilter(lineNoiseColor, 2);
      var linePixelator = new PixelateFilter(4, 4, InterpolationMode.NearestNeighbor, InterpolationMode.Low);

      var bartonLineGen = new BcLineGenerator() {
        Width = bartonBmp.Width + linesExtraWidth,
        NumLayers = linesNumLayers,
        MinHeight = linesMinHeight,
        MaxHeight = linesMaxHeight,
        StartColor = Profile.Colors.Base,
        EndColor = ColorUtil.Lerp(Profile.Colors.Base, Color.Black, 0.80)
      };
      var bartonLineBmp = bartonLineGen.Generate();
      bartonLineBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
      lineNoise.Apply(bartonLineBmp);
      linePixelator.Apply(bartonLineBmp);

      var codesLineGen = new BcLineGenerator() {
        Width = codesBmp.Width + linesExtraWidth,
        NumLayers = linesNumLayers,
        MinHeight = linesMinHeight,
        MaxHeight = linesMaxHeight,
        StartColor = Profile.Colors.Strong,
        EndColor = ColorUtil.Lerp(Profile.Colors.Strong, Color.Black, 0.80)
      };
      var codesLineBmp = codesLineGen.Generate();
      lineNoise.Apply(codesLineBmp);
      linePixelator.Apply(codesLineBmp);

      // Layout

      using (var g = Graphics.FromImage(fullBmp)) {
        var bumpHeight = (int)(bartonBmp.Height * 0.12);
        var pinchWidth = (int)(bartonBmp.Width * 0.05);
        var lineSepWidth = (int)(bartonBmp.Width * 0.10);
        var textWidth = bartonBmp.Width + codesBmp.Width;

        var bartonTextTL = new Point(
          ((Profile.Width / 2) - (textWidth / 2)) + pinchWidth,
          ((Profile.Height / 2) - (bartonBmp.Height / 2)) - bumpHeight
        );
        var codesTextTL = new Point(
          (((Profile.Width / 2) - (textWidth / 2)) + bartonBmp.Width) - pinchWidth,
          ((Profile.Height / 2) - (codesBmp.Height / 2)) + bumpHeight
        );

        var bartonLineTL = new Point(
          (bartonTextTL.X - lineSepWidth) - linesExtraWidth,
          (bartonTextTL.Y + bartonBmp.Height) - bartonLineBmp.Height
        );
        var codesLineTL = new Point(
          (codesTextTL.X + lineSepWidth),
          (codesTextTL.Y)
        );

        // Draw lines
        g.DrawImage(bartonLineBmp, bartonLineTL);
        g.DrawImage(codesLineBmp, codesLineTL);

        // Apply dark halo
        darkHaloFilter.Apply(fullBmp);

        // Draw text
        g.DrawImage(bartonBmp, bartonTextTL);
        g.DrawImage(codesBmp, codesTextTL);

      }

      // Gradient Stuff

      var gradGen = new GradientGenerator(
        Profile.Width,
        Profile.Height,
        new Point(Profile.Width / 2, 0),
        new Point(Profile.Width / 2, Profile.Height),
        Color.FromArgb(32, 255, 255, 255),
        Color.FromArgb(0, 0, 0, 0)
      );
      var gradBmp = gradGen.Generate();
      using (var g = Graphics.FromImage(fullBmp)) {
        g.DrawImage(gradBmp, 0, 0);
      }

      return fullBmp;
    }

  }

}
