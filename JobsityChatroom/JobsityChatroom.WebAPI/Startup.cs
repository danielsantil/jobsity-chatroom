using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Data;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.ExtensionMethods;
using JobsityChatroom.WebAPI.Hubs;
using JobsityChatroom.WebAPI.Models.Authentication;
using JobsityChatroom.WebAPI.MQ;
using JobsityChatroom.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobsityChatroom.WebAPI
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSignalR();

            services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddDbContext<ChatroomDbContext>(options =>
                options.UseLazyLoadingProxies()
                       .UseSqlServer(_configuration.GetConnectionString("JobsityChatroom")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ChatroomDbContext>()
                .AddDefaultTokenProviders();

            // Overriding defaults for password validation. Done on purpose to make testing easier.
            services.Configure<IdentityOptions>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireLowercase = true;
                opts.Password.RequiredLength = 1;
                opts.Password.RequiredUniqueChars = 0;
            });

            services.ConfigureAuthentication(_configuration);

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IChatCommandService, ChatCommandService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddSingleton<IStockMessageSender, StockMessageSender>();
            services.AddHostedService<StockMessageConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ChatroomDbContext dbContext)
        {
            EnsureDbConnection(dbContext);
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatroomHub>("/slr/chatroom");
            });
        }

        private void EnsureDbConnection(ChatroomDbContext dbContext, int retries = 0)
        {
            if (!dbContext.Database.CanConnect())
            {
                if (retries == 10)
                {
                    Console.WriteLine("Max retries reached.");
                    return;
                }
                retries += 1;
                Console.WriteLine("DB unreachable. Attempting connection after {0} ms. Retry count: {1}", 3000, retries);
                Thread.Sleep(3000);
                EnsureDbConnection(dbContext, retries);
            }
        }
    }
}
