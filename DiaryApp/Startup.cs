using AutoMapper;
using DB.Context;
using DB.Repository;
using DiaryApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiaryApp
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
            services.AddControllersWithViews();
            services
                .AddScoped<IRepositoryAll, RepositoryAll>()
                .AddScoped<IRepository<DealEntry>, Repository<DealEntry>>()
                .AddScoped<IRepository<MemoEntry>, Repository<MemoEntry>>()
                .AddScoped<IRepository<MeetingEntry>, Repository<MeetingEntry>>()

                //.AddScoped<IDataService, TestDataService>()
                .AddScoped<IDataService, DataService>()
                .AddScoped<IDataService<Models.DealEntry>, DataService<Models.DealEntry, DealEntry>>()
                .AddScoped<IDataService<Models.MemoEntry>, DataService<Models.MemoEntry, MemoEntry>>()
                .AddScoped<IDataService<Models.MeetingEntry>, DataService<Models.MeetingEntry, MeetingEntry>>()
                .AddAutoMapper(typeof(MappingProfile));
            services.AddDbContext<MainDbContext>((opt) =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DiaryDB"));
                opt.EnableSensitiveDataLogging();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
