using Hedgehogcat.Web.Admin.Entities;
using Hedgehogcat.Web.Admin.Helpers;
using Hedgehogcat.Web.Admin.Services;
using Serilog;

namespace Hedgehogcat.Web.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .WriteTo.Exceptionless()
                        .CreateLogger();

            Log.Information("日志");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DataContext>();
			// configure strongly typed settings object
			builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
			// configure DI for application services
			builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddSession();

			var app = builder.Build();
            // add hardcoded test user to db on startup
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                var testAccount = new Account
                {
                    Id = 1,
                    Username = "test",
                    Password = "test"
                };
                context.Accounts.Add(testAccount);
                context.SaveChanges();
            }

            // Configure the HTTP request pipeline.
         /*   if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }*/
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}");

            app.Run();
        }
    }
}