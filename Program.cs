using Branding.Brand;
using Branding.Util;
using System.Drawing;

namespace Branding {

  public class Program {
    public static readonly string OutputDirPath = @"C:\BartonCodes\Branding\out";

    static void Main(string[] args) {
      var profiles = GetProfiles();

      foreach(var profile in profiles) {
        var renderer = new BrandRenderer(profile);
        var bmp = renderer.Render();
        var filePath = Path.Combine(OutputDirPath, profile.Name + ".bmp");
        bmp.Save(filePath);
      }
    }

    static List<BrandProfile> GetProfiles() {
      var xColors = new ColorProfile() {
        Base = Color.FromArgb(255, 64, 160, 160),
        Strong = Color.FromArgb(255, 112, 208, 208),
        Highlight = Color.FromArgb(255, 0, 255, 128),
        Texture = Color.FromArgb(255, 112, 208, 208),
        Grit = Color.FromArgb(255, 0, 96, 128)
      };

      var youtubeColors = new ColorProfile() {
        Base = Color.Red,
        Strong = ColorUtil.Lerp(Color.Red, Color.White, 0.25),
        Highlight = Color.Yellow,
        Texture = Color.Orange,
        Grit = Color.DarkRed
      };

      var twitchColors = new ColorProfile() {
        Base = Color.Purple,
        Strong = Color.Pink,
        Highlight = Color.Teal,
        Texture = Color.Red,
        Grit = Color.Purple
      };

      return [
        new BrandProfile() {
          Name = "X Banner",
          Colors = xColors,
          Type = BrandType.Banner,
          Width = 1500,
          Height = 500,
          AreaOfInterest = new Rectangle(325, 125, 700, 250)
        },

        new BrandProfile() {
          Name = "Youtube Banner",
          Colors = youtubeColors,
          Type = BrandType.Banner,
          Width = 2560,
          Height = 1440,
          AreaOfInterest = new Rectangle(662, 551, 1235, 338)
        },

        new BrandProfile() {
          Name = "Twitch Banner",
          Colors = twitchColors,
          Type = BrandType.Banner,
          Width = 1200,
          Height = 480,
          AreaOfInterest = new Rectangle(300, 120, 600, 240)
        },

      ];
    }

  }
}
