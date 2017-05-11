using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Stack_Exchange_Voting_Utility.Startup))]
namespace Stack_Exchange_Voting_Utility
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
