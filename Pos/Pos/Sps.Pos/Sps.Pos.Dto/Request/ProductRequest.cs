using Sps.Pos.Dto.Request.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request
{
	public class ProductRequest:RequestBase
	{
		public int Id { get; set; }

		public string BarCode { get; set; }

		public string Name { get; set; }

		public int? UnitId { get; set; }

		public decimal PurchaseRate { get; set; }

		public decimal SalesRate { get; set; }

		public decimal DiscountAmount { get; set; }

		public int? TaxId { get; set; }

		public int? CategoryId { get; set; }

		public int? SupplierId { get; set; }

		public int? ColorId { get; set; }

		public int? SubCategoryId { get; set; }

		public int? BrandId { get; set; }

		public int? SizeId { get; set; }

		public string ImageUrl { get; set; }

		public bool DisplayOnPas { get; set; }
		public bool IsBatch { get; set; }
		public bool IsDeal { get; set; }
		public bool AddModifierGroup { get; set; }
	}
}
