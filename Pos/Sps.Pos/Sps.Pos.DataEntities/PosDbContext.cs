using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sps.Pos.DataEntities.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities
{
	public class PosDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		public PosDbContext(DbContextOptions<PosDbContext> options) : base(options)
		{

		}

		public DbSet<Application> ApplicationSet { get; set; }
		public DbSet<SecurityQuestion> SecurityQuestionSet { get; set; }
		public DbSet<LoginHistory> LoginHistorySet { get; set; }
		public DbSet<PasswordHistory> PasswordHistorySet { get; set; }
		public DbSet<Permission> PermissionSet { get; set; }
		public DbSet<RolePermission> RolePermissionSet { get; set; }

		//public DbSet<Country> CountrySet { get; set; }

		//public DbSet<ContactUs> ContactUsSet { get; set; }
		public DbSet<Customer> CustomerSet { get; set; }
		//public DbSet<Category> CategorySet { get; set; }
		//public DbSet<Order> OrderSet { get; set; }
		//public DbSet<OrderDetail> OrderDetailSet { get; set; }
		//public DbSet<Product> ProductSet { get; set; }
		//public DbSet<ShoppingCart> ShoppingCartSet { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<RolePermission>()
				.HasKey(c => new { c.RoleId, c.PermissionId });

			foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}

			builder.Entity<Application>().HasData(
				new Application()
				{
					Id = Guid.Parse("1E0E8CE3-32FA-49AA-730D-A101CDF5F0C2"),
					ApplicationName = "Kitchen World Web Application",
					ApplicationUrl = "https://localhost:44370",
					ApplicationVersion = "Version 1.0.0.0",
					Description = "Kitchen World Web Application.",
				});
		}
	}
}