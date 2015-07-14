using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using System.Web.Routing;

[assembly: OwinStartup(typeof(ns.Plugin.Web.OwinStartup))]

namespace ns.Plugin.Web {
    public class OwinStartup {
        public void Configuration(IAppBuilder app) {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //HttpConfiguration config = new HttpConfiguration();
            //config.Routes.MapHttpRoute(
            //    name: "DefaultWebApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //app.UseWebApi(config);

            var physicalFileSystem = new PhysicalFileSystem(@"./Plugins");
            var options = new FileServerOptions {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "Index.html"
            };

            app.UseFileServer(options);

            app.MapSignalR();
        }
    }
}
