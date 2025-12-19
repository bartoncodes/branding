using Branding.App.Brand;
using System;
using System.Collections.Generic;
using System.Text;

namespace Branding.App.Renderers {

  public class FrameRenderer : Renderer {

    protected override bool NeedsBgNoiseLayer => true;
    protected override bool NeedsBinaryLayer => true;
    protected override bool NeedsDarkHaloLayer => true;
    protected override bool NeedsBcLogoLayer => false;
    protected override bool NeedsProPicLayer => false;
    protected override bool NeedsGlossGradientLayer => false;

    public FrameRenderer(BrandProfile profile) : base(profile) {

    }

  }

}
