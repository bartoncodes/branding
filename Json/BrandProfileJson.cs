using Branding.Brand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Json {

  public class BrandProfileJson {
    public string Name { get; set; }
    public string Colors { get; set; }
    public string Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public RectangleJson AreaOfInterest { get; set; }
  }

}
