using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies; 
using Owin;
using System.Web;
using System.Web.Mvc;

namespace TestApp.App_Start
{
    public partial class StartUp 
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/LogOff"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30.0)
            });
        }
    }
}