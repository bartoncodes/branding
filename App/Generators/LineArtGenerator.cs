using App.Util;
using Branding.App.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Branding.App.Generators {

  public class LineArtGenerator : IGenerator {
    private const float GrayR = 0.299f;
    private const float GrayG = 0.587f;
    private const float GrayB = 0.114f;

    public Image SourceBmp { get; init; }
    public int OutWidth { get; init; }
    public int OutHeight { get; init; }
    public bool WhiteBg { get; init; } = true;
    public int SourceMinIntensity { get; init; }
    public int SourceMaxIntensity { get; init; }
    public int MidIntensity { get; init; }
    public Color LowColor { get; init; }
    public Color MidColor { get; init; }
    public Color HighColor { get; init; }

    public Bitmap Generate() {
      using(var dsp = new Disposer()) {
        var outBmp = new Bitmap(OutWidth, OutHeight); // do not dispose
        var outG = dsp.Add(Graphics.FromImage(outBmp));

        // Scale down onto output
        outG.DrawImage(SourceBmp, new Rectangle(0, 0, OutWidth, OutHeight));

        // Invert if necessary
        if (WhiteBg) {
          for(var y = 0; y < OutHeight; y++) {
            for(var x = 0; x < OutWidth; x++) {
              var c = outBmp.GetPixel(x, y);
              outBmp.SetPixel(x, y, Color.FromArgb(255, (255 - c.R), (255 - c.G), (255 - c.B)));
            }
          }
        }

        var outColorLookup = new Color[256];
        var lowToMidScale = 1.0 / MidIntensity;
        var midToHighScale = 1.0 / (256 - MidIntensity);
        for(var i = 0; i < MidIntensity; i++) {
          outColorLookup[i] = Color.FromArgb(i, ColorUtil.Lerp(LowColor, MidColor, (i * lowToMidScale)));
        }
        for(var i = MidIntensity; i < 256; i++) {
          outColorLookup[i] = Color.FromArgb(i, ColorUtil.Lerp(MidColor, HighColor, ((i - MidIntensity) * midToHighScale)));
        }

        var srcMinIntensity = SourceMinIntensity / 256.0f;
        var srcMaxIntensity = SourceMaxIntensity / 256.0f;
        var srcIntensityMult = 1.0f / (srcMaxIntensity - srcMinIntensity);

        for (var y = 0; y < OutHeight; y++) {
          for (var x = 0; x < OutWidth; x++) {
            var pixSrcColor = outBmp.GetPixel(x, y);
            var pixSrcIntensity = ((pixSrcColor.R * GrayR) + (pixSrcColor.G * GrayG) + (pixSrcColor.B * GrayB)) / 256.0f;

            var pixOutIntensity = Math.Max(0.0f, Math.Min(1.0f, (pixSrcIntensity - srcMinIntensity) * srcIntensityMult));
            var pixOutGray = Math.Min(255, (int)(pixOutIntensity * 256));

            var pixOutColor = outColorLookup[pixOutGray];

            // TESTING
            outBmp.SetPixel(x, y, pixOutColor);
          }
        }

        return outBmp;
      }
    }

  }

}
