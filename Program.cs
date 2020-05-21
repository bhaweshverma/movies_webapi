using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const int retainedFileCountLimit = 5; // The maximum number of log files that will be retained (default is 31)
            const long fileSizeLimitBytes = 1L * 1024 * 1024; // 1MB (size is in bytes & default is 1 GB)
            const string defaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
            const RollingInterval rollingInterval = RollingInterval.Infinite;
            const bool rollOnFileSizeLimit = true;

            Log.Logger = new LoggerConfiguration()
                //.ReadFrom.Configuration(IConfiguration)
                .Enrich.FromLogContext()
                .WriteTo.File(new RenderedCompactJsonFormatter(), "Logs\\moviesapi.ndjson", 
                        LogEventLevel.Debug,fileSizeLimitBytes, null, false, false, null, 
                        rollingInterval, rollOnFileSizeLimit, retainedFileCountLimit, null )
                .WriteTo.File("Logs\\moviesapi.txt", LogEventLevel.Debug, defaultOutputTemplate, 
                        null, fileSizeLimitBytes, null, false, false)
                .WriteTo.Console()
                .CreateLogger();

                try
                {
                    Log.Information("Starting up application");
                    CreateHostBuilder(args).Build().Run();
                }
                catch(Exception ex)
                {
                    Log.Fatal(ex, "Application start-up failed");
                }
                finally
                {
                    Log.CloseAndFlush();
                }  
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
