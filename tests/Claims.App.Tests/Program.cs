using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Claims.App.Tests
{
    class Program
    {

        public static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {

            LoadConfig("Development");

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();


            string text = @"a sdfa sdf<total>123.65</total>a sdfa sdf";
            //string pattern = @"</total>"; //.+\d+.+total";
            string pattern = @"<total>(?<totalindex>.*)</total>"; //.+\d+.+total";

            //string text = "abc123def";
            //string pattern = @"\D+(?<digit>\d+)\D+(?<digit>\d+)?";

            logger.LogDebug("Looking in [{0}]", text);
            Match m = Regex.Match(text, pattern, RegexOptions.IgnoreCase);

            if (m.Success)
            {
                Group group = m.Groups["totalindex"];

                logger.LogDebug("Group [{0}]: [{1}]", group.Index, group.Value);

                for (int cc = 0; cc < group.Captures.Count; cc++)
                {
                    Capture cap = group.Captures[cc];
                    logger.LogDebug("   Capture [{0}]: [{1}]", cc, cap.Value);
                }
                //for (int c = 0; c < m.Groups.Count; c++)
                //{
                //    Group group = m.Groups[c];
                //    logger.LogDebug("Group [{0}]: [{1}]", c, group.Value);

                //    for (int cc = 0; cc < group.Captures.Count; cc++)
                //    {
                //        Capture cap = group.Captures[cc];
                //        logger.LogDebug("   Capture [{0}]: [{1}]", cc, cap.Value);
                //    }
                //}
            }
            else
            {
                logger.LogDebug("Pattern [{0}] not found in [{1}]", pattern, text);
            }


            logger.LogDebug("Press any key...");
            Console.ReadKey();
        }

        private static void LoadConfig(string environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);

            builder.AddJsonFile($"appsettings.{environment}.json", optional: false);

            Configuration = builder.Build();
        }

        static private void ConfigureServices(IServiceCollection services)
        {
            
            services.AddLogging(configure =>
            {
                configure.AddConfiguration(Configuration.GetSection("Logging"));
                configure.AddConsole();
                configure.AddDebug();
            });
        }
    }
}
