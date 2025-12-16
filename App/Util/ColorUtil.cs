using Branding.App.Brand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.App.Util {

  public static class ColorUtil {

    public static Color Lerp(Color a, Color b, double x) {
      x = Math.Min(1.0, Math.Max(0.0, x));
      return Color.FromArgb(
        (int)(a.A + ((b.A - a.A) * x)),
        (int)(a.R + ((b.R - a.R) * x)),
        (int)(a.G + ((b.G - a.G) * x)),
        (int)(a.B + ((b.B - a.B) * x))
      );
    }

    public static readonly Dictionary<string, ColorProfile> ColorThemes = new() {
      {
        "Frame",
        new() {
          // Base = ColorUtil.Lerp(Color.FromArgb(255, 64, 160, 160), Color.DarkGray, 0.5),
          // Strong = ColorUtil.Lerp(Color.FromArgb(255, 112, 208, 208), Color.DarkGray, 0.5),
          Base = Color.DarkGray,
          Strong = Color.Gray,
          Highlight = Color.FromArgb(255, 0, 255, 128),
          Texture = ColorUtil.Lerp(Color.FromArgb(255, 112, 208, 208), Color.Black, 0.25),
          Grit = ColorUtil.Lerp(Color.FromArgb(255, 0, 96, 128), Color.DarkGray, 0.5)
        }
      },
      {
        "X",
        new() {
          Base = Color.FromArgb(255, 64, 160, 160),
          Strong = Color.FromArgb(255, 112, 208, 208),
          Highlight = Color.FromArgb(255, 0, 255, 128),
          Texture = Color.FromArgb(255, 112, 208, 208),
          Grit = Color.FromArgb(255, 0, 96, 128)
        }
      },
      {
        "Youtube",
        new() {
          Base = ColorUtil.Lerp(Color.Red, Color.Black, 0.25),
          Strong = ColorUtil.Lerp(Color.Red, Color.Orange, 0.25),
          Highlight = Color.Yellow,
          Texture = Color.Orange,
          Grit = Color.DarkRed
        }
      },
      {
        "Twitch",
        new() {
          Base = ColorUtil.Lerp(Color.Purple, Color.White, 0.25),
          Strong = ColorUtil.Lerp(Color.Pink, Color.Black, 0.25),
          Highlight = Color.Teal,
          Texture = Color.Pink,
          Grit = Color.Purple
        }
      },
      {
        "Patreon",
        new() {
          Base = Color.Orange,
          Strong = Color.OrangeRed,
          Highlight = Color.Yellow,
          Texture = Color.Yellow,
          Grit = Color.Brown
        }
      }
    };
  }

}
