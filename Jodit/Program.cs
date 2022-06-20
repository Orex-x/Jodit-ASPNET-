using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Jodit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseUrls("http://localhost:5001");
                        webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                        webBuilder.UseIISIntegration();
                        webBuilder.UseStartup<Startup>();
                    });
    }
}