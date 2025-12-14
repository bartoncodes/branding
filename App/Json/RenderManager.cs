using Branding.App.Brand;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brand.App.Json {

  public class RenderManager {
    private static readonly string OutputsDirPath = @"C:\BartonCodes\Branding\Outputs";

    public ProfileManager ProfileManager { get; init; }

    public RenderManager(ProfileManager profileManager) {
      ProfileManager = profileManager;
    }

    public void RenderAll() {
      var profiles = ProfileManager.LoadProfiles();
      foreach (var profile in profiles) {
        var renderer = new BrandRenderer(profile);
        var bmp = renderer.Render();
        var filePath = Path.Combine(OutputsDirPath, profile.Name + ".bmp");
        bmp.Save(filePath);
      }
    }

  }

}
