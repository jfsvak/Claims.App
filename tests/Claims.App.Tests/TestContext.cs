using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    public class TestContext
    {
        public readonly IServiceProvider ServiceProvider;
        public static IConfigurationRoot Configuration;

        public TestContext()
        {
            LoadConfig("Development");

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            this.ServiceProvider = serviceCollection.BuildServiceProvider();
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

        [CollectionDefinition("TestContextCollection")]
        public class TestContextCollection : ICollectionFixture<TestContext>
        {
            // empty class
        }

        private static void LoadConfig(string environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);

            builder.AddJsonFile($"appsettings.{environment}.json", optional: false);

            Configuration = builder.Build();
        }

    }

    class Converter : TextWriter
    {
        ITestOutputHelper _output;
        public Converter(ITestOutputHelper output)
        {
            _output = output;
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
        public override void WriteLine(string format, params object[] args)
        {
            _output.WriteLine(format, args);
        }
    }
}
