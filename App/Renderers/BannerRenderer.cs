using Branding.App.Brand;
using System;
using System.Collections.Generic;
using System.Text;

namespace Branding.App.Renderers {

  public class BannerRenderer : Renderer {

    protected override bool NeedsBgNoiseLayer => true;
    protected override bool NeedsBinaryLayer => true;
    protected override bool NeedsDarkHaloLayer => true;
    protected override bool NeedsBcLogoLayer => true;
    protected override bool NeedsGlossGradientLayer => true;

    public BannerRenderer(BrandProfile profile) : base(profile) {

    }



  }

}
