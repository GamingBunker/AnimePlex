using Cesxhin.AnimeSaturn.Application.CronJob;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Application.Services;
using Cesxhin.AnimeSaturn.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using Quartz;
using System;

namespace Cesxhin.AnimeSaturn.Api
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
            //interfaces
            //services
            services.AddSingleton<IAnimeService, AnimeService>();
            services.AddSingleton<IEpisodeService, EpisodeService>();
            services.AddSingleton<IEpisodeRegisterService, EpisodeRegisterService>();

            //repositories
            services.AddSingleton<IAnimeRepository, AnimeRepository>();
            services.AddSingleton<IEpisodeRepository, EpisodeRepository>();
            services.AddSingleton<IEpisodeRegisterRepository, EpisodeRegisterRepository>();

            //init repoDb
            RepoDb.PostgreSqlBootstrap.Initialize();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cesxhin.AnimeSaturn.Api", Version = "v1" });
            });

            //cronjob for check health
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.ScheduleJob<HealthJob>(trigger => trigger
                    .StartNow()
                    .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(60)), job => job.WithIdentity("api"));
            });

            //setup nlog
            var level = Environment.GetEnvironmentVariable("LOG_LEVEL").ToLower() ?? "info";
            LogLevel logLevel = NLogManager.GetLevel(level);
            NLogManager.Configure(logLevel);

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cesxhin.AnimeSaturn.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
