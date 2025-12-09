using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Brand {

  public class BrandProfile {
    public string Name { get; init; }
    public ColorProfile Colors { get; init; }
    public BrandType Type { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public Rectangle AreaOfInterest { get; init; }
  }

}
