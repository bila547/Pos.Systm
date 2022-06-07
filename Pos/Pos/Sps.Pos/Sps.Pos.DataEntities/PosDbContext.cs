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
		public DbSet<Customer> CustomerSet { get; set; }
		public DbSet<Area> AreaSet { get; set; }
		public DbSet<Channel> ChannelSet { get; set; }
		public DbSet<Counter> CounterSet { get; set; }
		public DbSet<Shift> ShiftSet { get; set; }
		public DbSet<Tax> TaxSet { get; set; }
		public DbSet<Unit> UnitSet { get; set; }
		public DbSet<Size> SizeSet { get; set; }
		public DbSet<Color> ColorSet { get; set; }
		public DbSet<Brand> BrandSet { get; set; }
		public DbSet<Supplier> SupplierSet { get; set; }
		public DbSet<Category> CategorySet { get; set; }
		public DbSet<SubCategory> SubCategorySet { get; set; }
		public DbSet<Promotion> PromotionSet { get; set; }
		public DbSet<Costomer> CostomerSet { get; set; }
		public DbSet<Product> ProductSet { get; set; }
		public DbSet<SalesMan> SalesManSet { get; set; }


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
					ApplicationUrl = "https://localhost:44357",
					ApplicationVersion = "Version 1.0.0.0",
					Description = "Kitchen World Web Application.",
				});
			base.OnModelCreating(builder);
		}
	}
}