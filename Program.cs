using Branding.Brand;
using Branding.Json;
using Branding.Util;
using System.Drawing;
using System.Text.Json;

namespace Branding {

  public class Program {
    private static readonly string ProfilesDirPath = @"C:\BartonCodes\Branding\Profiles";
    private static readonly string OutputsDirPath = @"C:\BartonCodes\Branding\Outputs";

    private static BrandProfile ParseProfile(BrandProfileJson json) {
      if (string.IsNullOrWhiteSpace(json.Name))
        throw new InvalidOperationException($"Invalid profile name '{json.Name}'.");
      if(json.Width <= 0 || json.Height <= 0)
        throw new InvalidOperationException($"Invalid width or height {json.Width}x{json.Height}.");
      if (!ColorProfiles.TryGetValue(json.Colors, out var colorProfile))
        throw new InvalidOperationException($"No color profile named '{json.Colors}'.");
      if (!Enum.TryParse(json.Type, out BrandType brandType))
        throw new InvalidOperationException($"No brand type named '{json.Type}'.");
      if(json.AreaOfInterest == null)
        throw new InvalidOperationException($"Invalid area of interest: null");
      if(
        json.AreaOfInterest.X <= 0 ||
        json.AreaOfInterest.Y <= 0 ||
        json.AreaOfInterest.Width <= 0 ||
        json.AreaOfInterest.Height <= 0
      )
        throw new InvalidOperationException(
          $"Invalid area of interest: {json.AreaOfInterest.X},{json.AreaOfInterest.Y},{json.AreaOfInterest.Width}x{json.AreaOfInterest.Height}");
      var areaOfInterest = new Rectangle(json.AreaOfInterest.X, json.AreaOfInterest.Y, json.AreaOfInterest.Width, json.AreaOfInterest.Height);
      return new BrandProfile() {
        Name = json.Name,
        Colors = colorProfile,
        Type = brandType,
        Width = json.Width,
        Height = json.Height,
        AreaOfInterest = areaOfInterest
      };
    }

    private static List<BrandProfile> LoadProfiles() {
      var profiles = new List<BrandProfile>();
      try {
        var profilesDir = new DirectoryInfo(ProfilesDirPath);
        var profileFiles = profilesDir.GetFiles();
        foreach (var profileFile in profileFiles) {
          var profileText = File.ReadAllText(profileFile.FullName);
          var profileJson = JsonSerializer.Deserialize<BrandProfileJson>(profileText)!;
          var profile = ParseProfile(profileJson);
          profiles.Add(profile);
        }
      } catch (Exception ex) {
        Console.WriteLine($"Problem loading profiles :( - {ex.Message}");
        return new List<BrandProfile>();
      }
      return profiles;
    }

    static void Main(string[] args) {
      var profiles = LoadProfiles();
      foreach(var profile in profiles) {
        var renderer = new BrandRenderer(profile);
        var bmp = renderer.Render();
        var filePath = Path.Combine(OutputsDirPath, profile.Name + ".bmp");
        bmp.Save(filePath);
      }
    }

    private static readonly Dictionary<string, ColorProfile> ColorProfiles = new() {
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
