using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Brand.App.Json {

  public class BrandProfileJson {
    public string Name { get; set; }
    public string Theme { get; set; }
    public string Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public RectangleJson AreaOfInterest { get; set; }
  }

}
