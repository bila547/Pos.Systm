using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sps.Pos.Mvc.Core;
using Microsoft.Extensions.Configuration;
using NToastNotify;
//using System.Configuration;
//using System.Configuration;

namespace Sps.Pos.Mvc
{
	public class Program
	{ 

		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
			{
				options.LoginPath = "/Security/Login";
				options.AccessDeniedPath = "/Home/AccessDenied";
				options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
			});

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			builder.Services.Configure<AppSettingConfiguration>(builder.Configuration.GetSection("AppSettings"));
			builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			//builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

			builder.Services.AddControllersWithViews()
				.AddNToastNotifyToastr(
				new ToastrOptions()
				{
					NewestOnTop = true,
					CloseButton = true,
					CloseDuration = true,
					ProgressBar = true,
					PositionClass = ToastPositions.TopRight,
					TimeOut = 30000
				},
				new NToastNotifyOption()
				{
					ScriptSrc = "/lib/toastr/toastr.min.js",
					StyleHref = "/lib/toastr/toastr.min.css"
				});

			builder.Services.AddMemoryCache();
			builder.Services.AddRouting(options =>
			{
				options.LowercaseUrls = true;
				options.LowercaseQueryStrings = false;
			});

			builder.Services.AddHttpClient<IApiClient, ApiClient>().ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new HttpClientHandler
				{
					ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
				};
			});
		
		var app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			app.UseSession();
			//app.UseNToastNotify();
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}