using App.Util;
using Brand.App.Filters;
using Branding.App.Brand;
using Branding.App.Filters;
using Branding.App.Generators;
using Branding.App.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Branding.App.Renderers {

  public abstract class Renderer {
    protected const string InputsDirPath = @"C:\BartonCodes\Branding\Inputs";

    protected BrandProfile Profile { get; set; }

    protected abstract bool NeedsBgNoiseLayer { get; }
    protected abstract bool NeedsBinaryLayer { get; }
    protected abstract bool NeedsDarkHaloLayer { get; }
    protected abstract bool NeedsBcLogoLayer { get; }
    protected abstract bool NeedsProPicLayer { get;}
    protected abstract bool NeedsGlossGradientLayer { get; }

    public Renderer(BrandProfile profile) {
      Profile = profile;
    }

    public Bitmap Render() {

      using (var dsp = new Disposer()) {

        var outBmp = new Bitmap(Profile.Width, Profile.Height); // out bmp, do not dispose
        using (var g = Graphics.FromImage(outBmp)) {
          g.Clear(Color.Black);
        }

        if(NeedsBgNoiseLayer)
          RenderBgNoiseLayer(outBmp);

        if(NeedsBinaryLayer)
          RenderBinaryLayer(outBmp);

        if(NeedsDarkHaloLayer)
          RenderDarkHaloLayer(outBmp);

        if(NeedsBcLogoLayer)
          RenderBcLogoLayer(outBmp);

        if (NeedsProPicLayer)
          RenderProPicLayer(outBmp);

        if(NeedsGlossGradientLayer)
          RenderGlossGradientLayer(outBmp);

        return outBmp;
      }

    }

    protected virtual void RenderBgNoiseLayer(Bitmap outBmp) {
      using(var dsp = new Disposer()) {
        var noiseColor = ColorUtil.Lerp(Profile.Colors.Grit, Color.Black, 0.9);
        for (var i = 3; i <= 8; i++) {
          var blockSize = (int)Math.Pow(2, i);
          var noiseFilter = new NoiseFilter(noiseColor, blockSize); // Block size should depend on total image size
          noiseFilter.Apply(outBmp);
        }
      }
    }

    protected virtual void RenderBinaryLayer(Bitmap outBmp) {
      using(var dsp = new Disposer()) {
        var binaryGen = new BinaryGenerator() {
          Width = outBmp.Width,
          Height = outBmp.Height,
          FontName = "ByteBounce",
          LowFontSize = 30f, // Profile.Height * 0.025f,
          HighFontSize = 60f, // Profile.Height * 0.05f,
          LowColor = ColorUtil.Lerp(Profile.Colors.Grit, Color.Black, 0.75),
          HighColor = ColorUtil.Lerp(Profile.Colors.Texture, Color.Black, 0.50),
          Seed = 0
        };
        var binaryBmp = dsp.Add(binaryGen.Generate());
        using (var g = Graphics.FromImage(outBmp)) {
          g.DrawImage(binaryBmp, 0, 0);
        }
      }
    }

    protected virtual void RenderDarkHaloLayer(Bitmap outBmp) {
      using(var dsp = new Disposer()) {
        var haloPadding = 0.15;
        if (Profile.Type == BrandType.Frame) {
          haloPadding = 0.0;
        }
        var darkHaloFilter = new DarkHaloFilter() {
          DarkAlpha = 128,
          JaggedFactor = 0.15,
          HaloLayers = 3,
          HaloPadding = haloPadding,
          MaxLineHeight = Math.Max(10, (int)(Profile.Height / 64))
        };
        darkHaloFilter.Apply(outBmp);
      }
    }

    public virtual void RenderBcLogoLayer(Bitmap outBmp) {
      using(var dsp = new Disposer()) {
        // Text Stuff

        var bartonFontSize = (int)(Profile.AreaOfInterest.Height * 0.50);
        var bartonFont = dsp.Add(new Font("ByteBounce", bartonFontSize));
        var bartonBrush = dsp.Add(new SolidBrush(Profile.Colors.Base));
        var bartonTextGen = new TextGenerator(bartonFont, bartonBrush, "Barton", 4);
        var bartonBmp = dsp.Add(bartonTextGen.Generate());

        var resizeGen = new ResizeGenerator(bartonBmp, 0.75, 1.0);
        bartonBmp = dsp.Add(resizeGen.Generate());

        var codesFontHeight = (int)(Profile.AreaOfInterest.Height * 0.65);
        var codesFont = dsp.Add(new Font("ByteBounce", codesFontHeight));
        var codesBrush = dsp.Add(new SolidBrush(Profile.Colors.Strong));
        var codesTextGen = new TextGenerator(codesFont, codesBrush, "Codes", 4);
        var codesBmp = dsp.Add(codesTextGen.Generate());


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
        var bartonLineBmp = dsp.Add(bartonLineGen.Generate());
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
        var codesLineBmp = dsp.Add(codesLineGen.Generate());
        lineNoise.Apply(codesLineBmp);
        linePixelator.Apply(codesLineBmp);

        // Layout

        using (var g = Graphics.FromImage(outBmp)) {
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

          // TODO: Generalize and fix the other one
          if (Profile.Type == BrandType.Frame) {
            var Aoi = Profile.AreaOfInterest;
            bartonTextTL = new Point(
              Aoi.X + (((Aoi.Width / 2) - (textWidth / 2)) + pinchWidth),
              Aoi.Y + (((Aoi.Height / 2) - (bartonBmp.Height / 2)) - bumpHeight)
            );
            codesTextTL = new Point(
              Aoi.X + ((((Aoi.Width / 2) - (textWidth / 2)) + bartonBmp.Width) - pinchWidth),
              Aoi.Y + (((Aoi.Height / 2) - (codesBmp.Height / 2)) + bumpHeight)
            );
          }

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

          // Draw text
          g.DrawImage(bartonBmp, bartonTextTL);
          g.DrawImage(codesBmp, codesTextTL);

        }
      }
    }

    public virtual void RenderProPicLayer(Bitmap outBmp) {
      using(var dsp = new Disposer()) {
        // Raw image
        var rawPath = Path.Combine(InputsDirPath, "BartonDrawing.png");
        var raw = Image.FromFile(rawPath);

        // Line art
        var lineArtGen = new LineArtGenerator() {
          SourceBmp = raw,
          OutWidth = Profile.AreaOfInterest.Width,
          OutHeight = Profile.AreaOfInterest.Height,
          WhiteBg = true,
          SourceMinIntensity = 0,
          SourceMaxIntensity = 96,
          MidIntensity = 128,
          LowColor = Profile.Colors.Grit,
          MidColor = Profile.Colors.Base,
          HighColor = Profile.Colors.Strong
        };
        var lineArt = dsp.Add(lineArtGen.Generate());

        // Draw line art
        var outG = dsp.Add(Graphics.FromImage(outBmp));
        outG.DrawImage(lineArt, Profile.AreaOfInterest.X, Profile.AreaOfInterest.Y);

        // Laser flare 1
        var laserFlare1Filter = new LaserFlareFilter() {
          LaserColor = Profile.Colors.Highlight,
          LargeFlareAngle = (float)(Math.PI / 4),
          CenterX = 0.27f,
          CenterY = 0.50f,
          MinExtent = 0.02f,
          MaxExtent = 0.15f
        };
        laserFlare1Filter.Apply(outBmp);

        // Laser flare 2
        var laserFlareFilter2 = new LaserFlareFilter() {
          LaserColor = Profile.Colors.Highlight,
          LargeFlareAngle = (float)(Math.PI / 4),
          CenterX = 0.50f,
          CenterY = 0.49f,
          MinExtent = 0.02f,
          MaxExtent = 0.15f
        };
        laserFlareFilter2.Apply(outBmp);
      }
    }

    public virtual void RenderGlossGradientLayer(Bitmap outBmp) {
      using (var dsp = new Disposer()) {
        var gradGen = new GradientGenerator(
          Profile.Width,
          Profile.Height,
          new Point(Profile.Width / 2, 0),
          new Point(Profile.Width / 2, Profile.Height),
          Color.FromArgb(32, 255, 255, 255),
          Color.FromArgb(0, 0, 0, 0)
        );
        var gradBmp = dsp.Add(gradGen.Generate());
        using (var g = Graphics.FromImage(outBmp)) {
          g.DrawImage(gradBmp, 0, 0);
        }
      }
    }

    public static Renderer? Create(BrandProfile profile) {
      switch(profile.Type) {
        case BrandType.Banner:
          return new BannerRenderer(profile);
        case BrandType.Profile:
          return new ProfileRenderer(profile);
        case BrandType.Frame:
          return new FrameRenderer(profile);
        default:
          return null;
      }
    }

  }

}
