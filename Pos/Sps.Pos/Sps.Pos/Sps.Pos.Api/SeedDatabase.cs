using Microsoft.AspNetCore.Identity;
using Sps.Pos.DataEntities;
using Sps.Pos.DataEntities.DataEntities;

namespace Sps.Pos.Api
{
	public class SeedDatabase
	{
		public async static Task Initialize(IServiceProvider serviceProvider)
		{
			try
			{
				using var serviceScope = serviceProvider.CreateScope();
				var scopeServiceProvider = serviceScope.ServiceProvider;
				var db = scopeServiceProvider.GetService<PosDbContext>();

				if (await db.Database.EnsureCreatedAsync())
				{
					await SeedDataAsync(scopeServiceProvider);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static async Task SeedDataAsync(IServiceProvider serviceProvider)
		{
			var _context = serviceProvider.GetRequiredService<PosDbContext>();
			var _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

			var _slhWebApp = _context.ApplicationSet.FirstOrDefault(x => x.Id == Guid.Parse("1E0E8CE3-32FA-49AA-730D-A101CDF5F0C2"));

			#region Roles

			if (!_roleManager.Roles.ToList().Any(x => x.Name.Equals("Admin", StringComparison.InvariantCultureIgnoreCase)))
			{
				await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin", ApplicationId = _slhWebApp.Id });
			}

			if (!_roleManager.Roles.ToList().Any(x => x.Name.Equals("Customer", StringComparison.InvariantCultureIgnoreCase)))
			{
				await _roleManager.CreateAsync(new ApplicationRole { Name = "Customer", ApplicationId = _slhWebApp.Id });
			}

			if (!_roleManager.Roles.ToList().Any(x => x.Name.Equals("Guest", StringComparison.InvariantCultureIgnoreCase)))
			{
				await _roleManager.CreateAsync(new ApplicationRole { Name = "Guest", ApplicationId = _slhWebApp.Id });
			}

			#endregion

			#region Users

			if (!_context.Users.ToList().Any(x => x.UserName == "admin1"))
			{
				var user = new ApplicationUser()
				{
					FirstName = "John",
					LastName = "Doe",
					Email = "admin1@demo.com",
					EmailConfirmed = true,
					IsApproved = true,
					DateOfBirth = DateTime.UtcNow.AddYears(-35),
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = "admin1",
					CreatedDate = DateTime.UtcNow
				};

				await _userManager.CreateAsync(user, "P@$$w0rd");

				var admin = _userManager.FindByNameAsync("admin1").Result;
				await _userManager.AddToRolesAsync(admin, new string[] { "Admin" });
			}

			if (!_context.Users.ToList().Any(x => x.UserName == "admin2"))
			{
				var user = new ApplicationUser()
				{
					FirstName = "Jhan",
					LastName = "Doe",
					Email = "admin2@demo.com",
					EmailConfirmed = true,
					IsApproved = true,
					DateOfBirth = DateTime.UtcNow.AddYears(-30),
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = "admin2",
					CreatedDate = DateTime.UtcNow
				};

				await _userManager.CreateAsync(user, "P@$$w0rd");

				var admin = _userManager.FindByNameAsync("admin2").Result;
				await _userManager.AddToRolesAsync(admin, new string[] { "Admin" });
			}

			if (!_context.Users.ToList().Any(x => x.UserName == "customer1"))
			{
				var user = new ApplicationUser()
				{
					FirstName = "Default",
					LastName = "Customer",
					Email = "customer1@demo.com",
					EmailConfirmed = true,
					IsApproved = true,
					DateOfBirth = DateTime.UtcNow.AddYears(-25),
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = "customer1",
					MobileNumber = "03006294435",
					CreatedDate = DateTime.UtcNow
				};

				await _userManager.CreateAsync(user, "P@$$w0rd");

				var customer1 = _userManager.FindByNameAsync("customer1").Result;
				await _userManager.AddToRolesAsync(customer1, new string[] { "Customer" });

				var dbStudent = new Customer
				{
					ApplicationUserId = customer1.Id,
					WebSite = "https://www.softpins.com",
					StreetAddress = "Model Town",
					CreatedDate = customer1.CreatedDate,
				};

				_context.CustomerSet.Add(dbStudent);
			}

			if (!_context.Users.ToList().Any(x => x.UserName == "customer2"))
			{
				var user = new ApplicationUser()
				{
					FirstName = "Default",
					LastName = "Customer 2",
					Email = "customer2@demo.com",
					EmailConfirmed = true,
					IsApproved = true,
					DateOfBirth = DateTime.UtcNow.AddYears(-25),
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = "customer2",
					MobileNumber = "03006294435",
					CreatedDate = DateTime.UtcNow
				};

				await _userManager.CreateAsync(user, "P@$$w0rd");

				var customer1 = _userManager.FindByNameAsync("customer2").Result;
				await _userManager.AddToRolesAsync(customer1, new string[] { "Customer" });

				var dbStudent = new Customer
				{
					ApplicationUserId = customer1.Id,
					WebSite = "https://brawon.com",
					StreetAddress = "Model Town",
					CreatedDate = customer1.CreatedDate,
				};

				_context.CustomerSet.Add(dbStudent);
			}

			#endregion

			await _context.SaveChangesAsync();
		}
	}
}