using App.Util;
using Brand.App.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Branding.App.Filters {

  public class LaserFlareFilter : IFilter {
    public Color LaserColor { get; init; }
    public float LargeFlareAngle { get; init; }
    public float SmallFlareAngle { get; init; }
    public float CenterX { get; init; }
    public float CenterY { get; init; }
    public float MinExtent { get; init; }
    public float MaxExtent { get; init; }


    public LaserFlareFilter() {

    }

    public void Apply(Bitmap outBmp) {
      using(var dsp = new Disposer()) {
        var rand = new Random();

        var maxRadRecip = 1.0 / (Math.PI * 2);
        // var maxRadRecip = 1.0 / Math.PI;

        var cX = (int)(CenterX * outBmp.Width);
        var cY = (int)(CenterY * outBmp.Height);
        var largestDim = outBmp.Width > outBmp.Height ? outBmp.Width : outBmp.Height;
        var minExt = (int)(largestDim * MinExtent);
        var maxExt = (int)(largestDim * MaxExtent);

        var extXA = Math.Max(0, (cX - maxExt));
        var extXB = Math.Min((outBmp.Width - 1), (cX + maxExt));
        var extYA = Math.Max(0, (cY - maxExt));
        var extYB = Math.Min((outBmp.Height - 1), (cY + maxExt));


        for(var testAngle = 0.0; testAngle < Math.PI * 2; testAngle += Math.PI / 12) {
          var testX = (int)(Math.Sin(testAngle) * 100);
          var testY = (int)(-Math.Cos(testAngle) * 100);

          var testAtan2 = Math.Atan2(testY, testX);
          var n = 0;
        }


        for(var y = extYA; y <= extYB; y++) {
          for(var x = extXA; x <= extXB; x++) {

            var dx = x - cX;
            var dy = y - cY;
            var d = (int)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if (d > maxExt)
              continue;

            /*
            var adx = dx;
            var ady = dy;
            if (adx > 0)
              adx = -adx;
            if (ady > 0)
              ady = -ady;
            */

            var a = Math.Atan2(-dy, -dx); //+ (Math.PI / 2.0));
            var da = a - LargeFlareAngle;
            if (da < -Math.PI)
              da += Math.PI * 2;
            // da = Math.Abs(da);


            // var angleIntensity = Math.Abs(((da * maxRadRecip) - 0.5) * 2.0);
            var angleIntensity = Math.Pow(Math.Abs(Math.Sin(da)), 12.0);
            

            // var angleIntensity = Math.Abs(((da * maxRadRecip) -0.5) * 2.0);

            var distanceIntensity = 1.0 - ((float)d / (float)maxExt);

            var distanceFactor = 0.1;
            var glowFactor = 4.0;
            var intensity = ((distanceIntensity * distanceFactor) + ((angleIntensity * distanceIntensity) * (1 - distanceFactor))) * glowFactor;
            // var intensity = angleIntensity;

            var srcC = outBmp.GetPixel(x, y);
            var outC = Color.FromArgb(255,
              Math.Min(255, srcC.R + (int)(LaserColor.R * intensity)),
              Math.Min(255, srcC.G + (int)(LaserColor.G * intensity)),
              Math.Min(255, srcC.B + (int)(LaserColor.B * intensity))
            );
            outBmp.SetPixel(x, y, outC);


          }
        }
        

      }
    }

  }

}
