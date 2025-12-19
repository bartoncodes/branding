using Branding.App.Brand;
using System;
using System.Collections.Generic;
using System.Text;

namespace Branding.App.Renderers {

  public class ProfileRenderer : Renderer {

    protected override bool NeedsBgNoiseLayer => true;
    protected override bool NeedsBinaryLayer => false;
    protected override bool NeedsDarkHaloLayer => true;
    protected override bool NeedsBcLogoLayer => false;
    protected override bool NeedsProPicLayer => true;
    protected override bool NeedsGlossGradientLayer => false;

    public ProfileRenderer(BrandProfile profile) : base(profile) {

    }

  }

}
