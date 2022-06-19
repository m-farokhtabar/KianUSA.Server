using Hangfire;
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
            var AutoRetryHangfire = new AutomaticRetryAttribute { Attempts = 3, DelaysInSeconds = new int[3] { 10, 30, 90 } };
            services.AddSingleton(AutoRetryHangfire);
            GlobalJobFilters.Filters.Add(AutoRetryHangfire);
            services.AddHangfire((provider, configuration) => configuration
            .UseInMemoryStorage()
            .UseFilter(provider.GetRequiredService<AutomaticRetryAttribute>())
            );
            services.AddHangfireServer();

            services.AddGrpc(Option =>
            {
                Option.EnableDetailedErrors = true;
                Option.MaxSendMessageSize = 20971520;
                Option.MaxReceiveMessageSize = 8388608;
            });
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));
            ApplicationSettings appSetting = new();
            services.AddSingleton<IApplicationSettings>(appSetting);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider srp)
        {
            ApplicationSettings appSetting = (ApplicationSettings)srp.GetService<IApplicationSettings>();
            appSetting.WwwRootPath = env.WebRootPath;
            appSetting.CatalogEmailSetting = "price-list-email-setting";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseGrpcWeb(); // Must be added between UseRouting and UseEndpoints
            app.UseCors();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGrpcService<CategoryService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGrpcService<ProductService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGrpcService<EmailService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
                endpoints.MapHangfireDashboard();
            });
        }
    }
}