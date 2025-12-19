using Branding.App.Brand;
using Branding.App.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Branding.App.Json {

  public class ProfileManager {

    public string ProfilesDirPath { get; set; }

    public ProfileManager(string profilesDirPath) {
      ProfilesDirPath = profilesDirPath;
    }

    public BrandProfile ParseProfile(BrandProfileJson json, string jsonFileName) {
      if (string.IsNullOrWhiteSpace(json.Name))
        throw new InvalidOperationException($"Invalid profile name '{json.Name}'.");
      if (json.Width <= 0 || json.Height <= 0)
        throw new InvalidOperationException($"Invalid width or height {json.Width}x{json.Height}.");
      if (!ColorUtil.ColorThemes.TryGetValue(json.Theme, out var colors))
        throw new InvalidOperationException($"No color profile named '{json.Theme}'.");
      if (!Enum.TryParse(json.Type, out BrandType brandType))
        throw new InvalidOperationException($"No brand type named '{json.Type}'.");
      if (json.AreaOfInterest == null)
        throw new InvalidOperationException($"Invalid area of interest: null");
      if (
        json.AreaOfInterest.X < 0 ||
        json.AreaOfInterest.Y < 0 ||
        json.AreaOfInterest.Width <= 0 ||
        json.AreaOfInterest.Height <= 0
      )
        throw new InvalidOperationException(
          $"Invalid area of interest: {json.AreaOfInterest.X},{json.AreaOfInterest.Y},{json.AreaOfInterest.Width}x{json.AreaOfInterest.Height}");
      var areaOfInterest = new Rectangle(json.AreaOfInterest.X, json.AreaOfInterest.Y, json.AreaOfInterest.Width, json.AreaOfInterest.Height);
      return new BrandProfile() {
        Name = json.Name,
        Type = brandType,
        Theme = json.Theme,
        FileName = jsonFileName,
        Colors = colors,
        
        Width = json.Width,
        Height = json.Height,
        AreaOfInterest = areaOfInterest
      };
    }

    public List<BrandProfile> LoadProfiles() {
      var profiles = new List<BrandProfile>();
      try {
        var profilesDir = new DirectoryInfo(ProfilesDirPath);
        var profileFiles = profilesDir.GetFiles();
        foreach (var profileFile in profileFiles) {
          var profileText = File.ReadAllText(profileFile.FullName);
          var profileJson = JsonSerializer.Deserialize<BrandProfileJson>(profileText)!;
          var profile = ParseProfile(profileJson, profileFile.Name);
          profiles.Add(profile);
        }
      } catch (Exception ex) {
        Console.WriteLine($"Problem loading profiles :( - {ex.Message}");
        return new List<BrandProfile>();
      }
      return profiles;
    }

  }

}
