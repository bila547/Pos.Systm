using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Product))]
	public class Product : EntityBase
	{
		public int Id { get; set; }
		
		public string BarCode { get; set; }

		public string Name { get; set; }

		[ForeignKey(nameof(Unit))]
		public int? UnitId { get; set; }

		public Unit Unit { get; set; }

		[Column(TypeName = "Decimal(18,2)")]
		public decimal PurchaseRate  { get; set; }

		[Column(TypeName = "Decimal(18,2)")]
		public decimal SalesRate { get; set; }

		[Column(TypeName = "Decimal(18,2)")]
		public decimal DiscountAmount { get; set; }

		[ForeignKey(nameof(Tax))]
		public int? TaxId { get; set; }

		public Tax Tax { get; set; }

		[ForeignKey(nameof(Category))]
		public int? CategoryId { get; set; }

		public Category Category { get; set; }

		[ForeignKey(nameof(Supplier))]
		public int? SupplierId { get; set; }

		public Supplier Supplier { get; set; }

		[ForeignKey(nameof(Color))]
		public int? ColorId { get; set; }

		public Color Color { get; set; }


		[ForeignKey(nameof(SubCategory))]
		public int? SubCategoryId { get; set; }

		public SubCategory SubCategory { get; set; }

		[ForeignKey(nameof(Brand))]
		public int? BrandId { get; set; }

		public Brand Brand { get; set; }

		[ForeignKey(nameof(Size))]
		public int? SizeId { get; set; }

		public Size Size { get; set; }

		public string ImageUrl { get; set; }

		public bool DisplayOnPas { get; set; }
		public bool IsBatch { get; set; }
		public bool IsDeal { get; set; }
		public bool AddModifierGroup { get; set; }
	}
}
