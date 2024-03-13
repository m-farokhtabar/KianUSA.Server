using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.PostgreSQL;
using System.Collections.Generic;

namespace KianUSA.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
                    Host.CreateDefaultBuilder(args)
                    .UseSerilog((context, services, configuration) =>
                    {
                        var Connection = context.Configuration.GetConnectionString("Database");
                        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
                            {
                                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                                { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                                { "raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
                                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                                { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                                { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                                { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
                            };
                        configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .ReadFrom.Services(services)
                            .Enrich.FromLogContext()
                            .Enrich.WithExceptionDetails()
                            .WriteTo.Console().MinimumLevel.Information()
                            .WriteTo.PostgreSQL(Connection, "logs", columnWriters, needAutoCreateTable: true).MinimumLevel.Error();
                    })
                //.ConfigureLogging((context, logging) =>
                //{
                //    logging.ClearProviders();
                //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                //    /*
                //    خروجی های لاگ برنامه را در پنجره 
                //    Output
                //    نمایش داده شود  
                //    */
                //    logging.AddDebug();
                //    /*                      
                //    خروجی های لاگ برنامه را در پنجره 
                //    Console
                //    نمایش داده شود  
                //    */
                //    logging.AddConsole();
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
