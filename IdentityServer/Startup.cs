// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Services.Users;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace IdentityServer
{
    public class Startup
    {
        private void InitDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            }
        }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string connectionString = "Server=.;Initial Catalog=PersistedGrantIdentity;Trusted_Connection=False;Integrated Security=True;MultipleActiveResultSets=True;Max Pool Size=50;Min Pool Size=5;Connection Timeout=30";
            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbContext => dbContext.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients)
                .AddProfileService<ProfileServices>()
                .AddJwtBearerClientAuthentication();
                //.AddTestUsers(TestUsers.Users);

            builder.Services.AddTransient<IProfileService, ProfileServices>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            InitDatabase(app);
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

}
