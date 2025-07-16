using InternalResourceBooking.Data;
using InternalResourceBooking.Models;
using InternalResourceBooking.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.ComponentModel.Design;
using IResourceService = InternalResourceBooking.Services.IResourceService;

namespace InternalResourceBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // Register services
            builder.Services.AddScoped<IResourceService, ResourceService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            // Initialize database
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Seed initial data
                if (!context.Resources.Any())
                {
                    context.Resources.AddRange(
                        new Resource { Name = "Meeting Room Alpha", Description = "Large room with projector and whiteboard", Location = "3rd Floor, West Wing", Capacity = 10, IsAvailable = true },
                        new Resource { Name = "Company Car 1", Description = "Compact sedan", Location = "Parking Bay 5", Capacity = 4, IsAvailable = true },
                        new Resource { Name = "Conference Room Beta", Description = "Small room for team meetings", Location = "2nd Floor, East Wing", Capacity = 6, IsAvailable = true }
                    );
                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
