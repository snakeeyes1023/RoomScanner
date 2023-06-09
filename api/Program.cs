using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using RoomScannerWeb.ActionFilters;
using RoomScannerWeb.Data.Entitites;
using RoomScannerWeb.Data.Helpers;
using RoomScannerWeb.Data.Services;
using SQLite;
using System.Reflection;

namespace RoomScannerWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.InitialiseLocalDatabase();

            // Add services to the container.
            builder.Services.AddSingleton<IScanService, ScanService>();
            builder.Services.Configure<ScanSetting>(builder.Configuration.GetSection("ScannerInfo"));

            builder.Services.AddScoped<IPValidationActionFilter>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHangfireServer();
            builder.Services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseRouting();

            // Add This
            app.UseHangfireDashboard();

            app.EnableLocalDatabaseArchivation();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RoomScanner API V1");
            });

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }

    public static class ProgramExtensions
    {
        public static void InitialiseLocalDatabase(this IServiceCollection collection)
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "roomscanner.db");
            var db = new SQLiteConnection(databasePath);
            
            db.CreateTable<ScanResultEntity>();
            db.CreateTable<ScanIntrusionEntity>();

            collection.AddSingleton(db);
        }

        public static void EnableLocalDatabaseArchivation(this WebApplication app)
        {
            var manager = new RecurringJobManager();
            manager.AddOrUpdate<ScanService>("db-archive", x => x.ArchiveData(), Cron.Daily());
        }
    }
}