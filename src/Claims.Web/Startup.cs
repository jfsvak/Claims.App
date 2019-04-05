using Claims.Business.Util;
using Claims.Web.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Claims.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc(options =>
            {
                options.InputFormatters.Insert(0, new TextPlainInputFormatter());
            });

            services.AddScoped<IClaimService, ClaimService>(
                s => new ClaimService(CultureInfo.CreateSpecificCulture(ApplicationConstants.DEFAULT_CULTURE_CODE))
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
