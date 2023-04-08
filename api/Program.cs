using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RoomScannerWeb.ActionFilters;
using RoomScannerWeb.Data;
using RoomScannerWeb.Data.Models;
using SQLite;

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
            builder.Services.AddScoped<IPValidationActionFilter>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
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

            collection.AddSingleton(db);
        }
    }
}