using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Brand.App.Filters {

  public interface IFilter {
    public void Apply(Bitmap bmp);
  }

}
