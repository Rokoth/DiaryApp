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
                .AddScoped<IRepositoryAll<DB.Context.Entry>, RepositoryAll>()
                .AddScoped<IRepository<DB.Context.DealEntry>, RepositoryEntry<DB.Context.DealEntry>>()
                .AddScoped<IRepository<DB.Context.MemoEntry>, RepositoryEntry<DB.Context.MemoEntry>>()
                .AddScoped<IRepository<DB.Context.MeetingEntry>, RepositoryEntry<DB.Context.MeetingEntry>>()
                .AddScoped<IRepository<DB.Context.Contact>, Repository<DB.Context.Contact>>()
                .AddScoped<IRepository<DB.Context.ContactInfo>, Repository<DB.Context.ContactInfo>>()
                .AddScoped<IRepository<DB.Context.MeetingPlace>, Repository<DB.Context.MeetingPlace>>()

                //.AddScoped<IDataService, TestDataService>()
                .AddScoped<IDataService, DataService>()
                .AddScoped<IDataService<Models.DealEntry>, DataService<Models.DealEntry, DB.Context.DealEntry>>()
                .AddScoped<IDataService<Models.MemoEntry>, DataService<Models.MemoEntry, DB.Context.MemoEntry>>()
                .AddScoped<IDataService<Models.MeetingEntry>, MeetingDataService>()
                .AddScoped<IContactDataService, ContactDataService>()
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
