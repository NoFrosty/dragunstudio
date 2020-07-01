using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LettuceEncrypt;

namespace Evopeons.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
#if RELEASE
                        .UseKestrel()
                        .UseUrls("https://*:443", "http://*:80")
#endif
                        .UseStartup<Startup>();
                });
    }
}
