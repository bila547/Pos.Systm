using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sps.Pos.DataEntities;
using Sps.Pos.DataEntities.DataEntities;

namespace Sps.Pos.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.Configure<ApiBehaviorOptions>(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddDbContext<PosDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.Configure<EmailConfirmationTokenProviderOptions>(
						x => x.TokenLifespan = System.TimeSpan.FromDays(7));

			builder.Services.Configure<PasswordResetTokenProviderOptions>(
						x => x.TokenLifespan = System.TimeSpan.FromDays(1));

			builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.Password.RequiredLength = 6;
				options.Password.RequireDigit = true;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;

				options.Tokens.PasswordResetTokenProvider = "CustomPasswordResetTokenProvider";
				options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmationTokenProvider";

				options.SignIn.RequireConfirmedAccount = true;
				options.SignIn.RequireConfirmedEmail = true;
				options.User.RequireUniqueEmail = true;

				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(20);

			})
			.AddRoles<ApplicationRole>()
			.AddEntityFrameworkStores<PosDbContext>()
			.AddPasswordValidator<UsernameAsPasswordValidator>()
			.AddDefaultTokenProviders()
			.AddTokenProvider<CustomPasswordResetTokenProvider<ApplicationUser>>("CustomPasswordResetTokenProvider")
			.AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmationTokenProvider");

			builder.Services.AddRouting(o => { o.LowercaseUrls = true; o.LowercaseQueryStrings = true; });
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pos.Api", Version = "v1" });
			});

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				SeedDatabase.Initialize(app.Services
					.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider).Wait();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pos.Api v1"));

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}