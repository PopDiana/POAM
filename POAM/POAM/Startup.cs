using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POAM.Models;

namespace POAM
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<POAMDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, POAMDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

             Initialize(context);
        }

        public static void Initialize(POAMDbContext context)
        {
            context.Database.EnsureCreated();       

            if(context.Owner.FirstOrDefault(o => o.Username == "admin") == null)
            {
                String adminUsername = "admin";
                String adminPassword = "P@$$w0rd";
                String salt = Authentication.Instance.GetRandomSalt();
                String hashedPassword = Authentication.Instance.HashPassword(adminPassword, salt);
                String email = "admin@mail.com";
                String fullName = " System Admin";
                String phone = "0000000000000";

                var admin = new Owner
                {
                    Username = adminUsername,
                    PassSalt = salt,
                    Password = hashedPassword,
                    IsAdmin = true,
                    Email = email,
                    FullName = fullName,
                    Telephone = phone


                };


                context.Add(admin);
                context.SaveChanges();

            }

        }
        
    }
}
