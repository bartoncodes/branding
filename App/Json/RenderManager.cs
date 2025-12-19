using Branding.App.Brand;
using Branding.App.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Branding.App.Json {

  public class RenderManager {
    private static readonly string OutputsDirPath = @"C:\BartonCodes\Branding\Outputs";

    public ProfileManager ProfileManager { get; init; }

    public RenderManager(ProfileManager profileManager) {
      ProfileManager = profileManager;
    }

    public void RenderAll() {
      var profiles = ProfileManager.LoadProfiles();
      foreach (var profile in profiles) {
        var renderer = Renderer.Create(profile);
        if (renderer == null)
          continue;
        var bmp = renderer.Render();
        var filePath = Path.Combine(OutputsDirPath, profile.Name + ".bmp");
        bmp.Save(filePath);
        bmp.Dispose();
      }
    }

  }

}
