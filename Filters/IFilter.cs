using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Branding.Filters {

  public interface IFilter {
    public void Apply(Bitmap bmp);
  }

}
