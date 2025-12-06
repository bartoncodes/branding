using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Brand {

  public class BrandProfile {
    public string Name { get; private set; }
    public BrandType Type { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Rectangle AreaOfInterest { get; private set; }

    public BrandProfile(string name, BrandType type, int width, int height, int aoiX, int aoiY, int aoiW, int aoiH) {
      Name = name;
      Type = type;
      Width = width;
      Height = height;
      AreaOfInterest = new Rectangle(aoiX, aoiY, aoiW, aoiH);
    }

    public static readonly BrandProfile YoutubeBanner = new BrandProfile("Youtube Banner", BrandType.Banner, 2560, 1440, 662, 551, 1235, 338);
    // TODO: Add other brand profiles

    public static readonly List<BrandProfile> Profiles = [
      YoutubeBanner,
      // TODO: Add other brand profiles
    ];
  }

}
