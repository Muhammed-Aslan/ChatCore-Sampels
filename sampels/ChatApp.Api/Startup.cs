using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ChatApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Validation;
using AspNet.Security.OpenIdConnect.Primitives;
using OpenIddict.Abstractions;
using System;
using ChatApp.Abstractions;

namespace ChatApp.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 10;

                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOpenIddict()
                .AddCore(builder =>
                {
                    builder.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
                })
                .AddServer(builder =>
                {
                    builder.UseMvc();
                    builder.EnableTokenEndpoint("/connect/token");
                    builder.AllowPasswordFlow();
                    builder.AllowRefreshTokenFlow();
                    builder.AcceptAnonymousClients();
                    builder.DisableHttpsRequirement();
                    builder.RegisterScopes(
                        OpenIdConnectConstants.Scopes.OpenId,
                        OpenIdConnectConstants.Scopes.Profile,
                        OpenIdConnectConstants.Scopes.OfflineAccess);
                })
                .AddValidation();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = OpenIddictValidationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationDefaults.AuthenticationScheme;
            });

            services.AddChatCore<IdentityProvider>()
                .UseEntityFrameworkCore<ApplicationDbContext>()
                .AddChatApp();

            // Add and Configure Cors
            services.AddCors(options => {
                var origins = Configuration["Origins"].Trim().Split(",");//new string[] { "http://localhost:4200" };
                for (int i = 0; i < origins.Length; i++)
                    origins[i] = origins[i].Trim();

                options.AddPolicy("ChatAppPolicy", policybuilder =>
                {
                    policybuilder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(origins);
                });
            });

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //Configure Cors
            app.UseCors("ChatAppPolicy");

            //prepare signalR Connection to be able for authentication *** this line is very important 
            //and must be ordered before "app.UseAuthentication();" otherwise signalR connections will be unAuthorized
            app.FixChatAppSignalRHeaders();

            app.UseAuthentication();
            
            // add chatApp Middleware in case you want to user the built-in Api Controllers
            app.UseChatApp();
            app.UseMvc();
        }
    }
}