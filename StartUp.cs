using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestApp.App_Start.StartUp))]
namespace TestApp.App_Start
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}