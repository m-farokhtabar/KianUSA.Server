using KianUSA.API.Configuration;
using KianUSA.API.Services;
using KianUSA.Application.SeedWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace KianUSA.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(Option =>
            {
                Option.EnableDetailedErrors = true;
                Option.MaxSendMessageSize = 20971520;
                Option.MaxReceiveMessageSize = 8388608;
            });
            ApplicationSettings appSetting = new();
            services.AddSingleton<IApplicationSettings>(appSetting);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider srp)
        {
            ApplicationSettings appSetting = (ApplicationSettings)srp.GetService<IApplicationSettings>();
            appSetting.WwwRootPath = env.WebRootPath;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<CategoryService>();
                endpoints.MapGrpcService<ProductService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
