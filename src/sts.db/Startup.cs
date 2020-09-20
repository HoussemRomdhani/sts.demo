// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using IdentityServerAdminUi.Data;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Linq;

namespace identity
{

    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
         var idpconnectionString = Configuration.GetConnectionString("DefaultConnection");
            // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            // configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(idpconnectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

           // var connectionString = Configuration.GetConnectionString("Configuration");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction = new UserInteractionOptions
                {
                    LogoutUrl = "/Account/Logout",
                    LoginUrl = "/Account/Login",
                    LoginReturnUrlParameter = "returnUrl"
                };
            })
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlServer(idpconnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlServer(idpconnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
                });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

            //services.UseAdminUI();
            //services.AddScoped<IdentityExpressDbContext, SqliteIdentityDbContext>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

         //   InitializeDatabase(app);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

          //  app.UseAdminUI();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                //context;
                //if (!context.Clients.Any())
                //{
                //    foreach (var client in Config.Clients)
                //    {
                //        context.Clients.Add(client.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.IdentityResources.Any())
                //{
                //    foreach (var resource in Config.IdentityResources)
                //    {
                //        context.IdentityResources.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.ApiResources.Any())
                //{
                //    foreach (var resource in Config.ap)
                //    {
                //        context.ApiResources.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}
            }
        }
    }
}

    //public class Startup
    //{
    //    public IWebHostEnvironment Environment { get; }
    //    public IConfiguration Configuration { get; }

    //    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    //    {
    //        Environment = environment;
    //        Configuration = configuration;
    //    }

    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        services.AddControllersWithViews();

    //        var idpconnectionString = Configuration.GetConnectionString("DefaultConnection");

    //        //  string connectionString = Configuration["ConnectionString"];
    //        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

    //        services.AddDbContext<ApplicationDbContext>(options =>
    //                options.UseSqlServer(idpconnectionString,
    //                    sql => sql.MigrationsAssembly(migrationsAssembly)));
    //     //   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


    //        services.AddIdentity<ApplicationUser, IdentityRole>()
    //            .AddEntityFrameworkStores<ApplicationDbContext>()

    //            .AddDefaultTokenProviders();

    //        var builder = services.AddIdentityServer(options =>
    //        {
    //            options.Events.RaiseErrorEvents = true;
    //            options.Events.RaiseInformationEvents = true;
    //            options.Events.RaiseFailureEvents = true;
    //            options.Events.RaiseSuccessEvents = true;

    //            // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
    //            options.EmitStaticAudienceClaim = true;
    //        })
    //             .AddConfigurationStore(options =>
    //             {
    //                 options.ConfigureDbContext = builder =>
    //                builder.UseSqlServer(idpconnectionString,
    //                    sql => sql.MigrationsAssembly(migrationsAssembly));
    //             })
    //        .AddOperationalStore(options =>
    //        {
    //            options.ConfigureDbContext = builder =>
    //                 builder.UseSqlServer(idpconnectionString,
    //                     sql => sql.MigrationsAssembly(migrationsAssembly));
    //            options.EnableTokenCleanup = true;
    //        });

    //        // not recommended for production - you need to store your key material somewhere secure
    //        builder.AddDeveloperSigningCredential();

    //        services.AddAuthentication()
    //            .AddGoogle(options =>
    //            {
    //                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

    //                // register your IdentityServer with Google at https://console.developers.google.com
    //                // enable the Google+ API
    //                // set the redirect URI to https://localhost:5001/signin-google
    //                options.ClientId = "copy client ID from Google here";
    //                options.ClientSecret = "copy client secret from Google here";
    //            });
    //    }

    //    public void Configure(IApplicationBuilder app)
    //    {
    //        if (Environment.IsDevelopment())
    //        {
    //            app.UseDeveloperExceptionPage();
    //            app.UseDatabaseErrorPage();
    //        }

    //        InitializeDatabase(app);

    //        app.UseStaticFiles();

    //        app.UseRouting();
    //        app.UseIdentityServer();
    //        app.UseAuthorization();
    //        app.UseEndpoints(endpoints =>
    //        {
    //            endpoints.MapDefaultControllerRoute();
    //        });
    //    }

    //    private void InitializeDatabase(IApplicationBuilder app)
    //    {
    //        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
    //        {
    //            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

    //            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    //            context.Database.Migrate();
    //            if (!context.Clients.Any())
    //            {
    //                foreach (var client in Config.Clients)
    //                {
    //                    context.Clients.Add(client.ToEntity());
    //                }
    //                context.SaveChanges();
    //            }

    //            if (!context.IdentityResources.Any())
    //            {
    //                foreach (var resource in Config.IdentityResources)
    //                {
    //                    context.IdentityResources.Add(resource.ToEntity());
    //                }
    //                context.SaveChanges();
    //            }
    //        }
    //    }
    //}
