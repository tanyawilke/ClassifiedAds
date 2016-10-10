using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClassifiedAdsApp.Startup))]
namespace ClassifiedAdsApp
{
    
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
