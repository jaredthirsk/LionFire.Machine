using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.StaticFiles;
using Microsoft.AspNet.StaticFiles.Infrastructure;
using LionFire.PullAgent;
using Microsoft.Extensions.OptionsModel;
using System.IO;
using LionFire.RevisionControl;
using LionFire.Machine;
using LionFire.Machine.Services;

namespace LionFire.Web.PullAgent.Mvc
{
    public class Startup
    {
        public string BasePath {
            get {
                return Configuration.GetSection("basePath").Value ?? "";
            }
        }
        public string RouteBasePath {
            get {
                var trimmed = BasePath.Trim('/');

                return trimmed.Length == 0 ? "" : trimmed + "/";
            }
        }

        IHostingEnvironment env;
        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine(env.EnvironmentName);
            this.env = env;
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings." + env.EnvironmentName + ".json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            env.WebRootPath += @"\hooks";
            services.AddOptions();
            services.Configure<PullAgentOptions>(Configuration.GetSection("PullAgent"));

            services.Add(new ServiceDescriptor(typeof(IServiceControlService), new ServiceControlService()));

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<PullAgentOptions> pullAgentOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseIISPlatformHandler();

            var basePath = BasePath;

            var sharedOptions = new SharedOptions()
            {
                RequestPath = basePath
            };

            app.UseStaticFiles(new StaticFileOptions(sharedOptions));


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: RouteBasePath + "{controller=Home}/{action=Index}/{id?}",
                    defaults: new
                    {

                    }
                    );
            });

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ =>
            {
                System.Threading.Thread.Sleep(4000);
                foreach (var repo in pullAgentOptions.Value.Repositories.Select(kvp => kvp.Value))
                {
                    if (Directory.Exists(repo.Path))
                    {
                        continue;
                    }

                    var tag = String.IsNullOrWhiteSpace(repo.Tag) ? null : repo.Tag;
                    var branch = String.IsNullOrWhiteSpace(repo.Branch) ? null : repo.Branch;

                    bool result = Git.Clone(repo.SshUrl, Path.GetDirectoryName(repo.Path), Path.GetFileName(repo.Path)); // REFACTOR
                    if (result)
                    { // REFACTOR
                        if (tag != null)
                        {
                            result &= Git.CheckoutTag(repo.Path, tag);
                        }
                        else if (branch != null)
                        {
                            result &= Git.CheckoutBranch(repo.Path, branch);
                        }
                    }
                }
            }));
        }

        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    }
}
