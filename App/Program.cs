using Branding.App.Gui;

namespace Branding.App {

  public static class Program {
    
    [STAThread]
    static void Main() {
      
      ApplicationConfiguration.Initialize();
      Application.Run(new AppForm());
    }

  }

}