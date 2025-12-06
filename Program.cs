using Branding.Brand;
using System.Drawing;

namespace Branding {

  public class Program {
    public static readonly string OutputDirPath = @"C:\BartonCodes\Branding\out";

    static void Main(string[] args) {
      foreach(var profile in BrandProfile.Profiles) {
        var renderer = new BrandRenderer(profile);
        var bmp = renderer.Render();
        var filePath = Path.Combine(OutputDirPath, profile.Name + ".bmp");
        bmp.Save(filePath);
      }
    }

  }
}
