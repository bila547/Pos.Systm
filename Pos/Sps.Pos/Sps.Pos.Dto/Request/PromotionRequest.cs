using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sps.Pos.Dto.Request
{
	public class PromotionRequest:RequestBase
	{
		public int Id { get; set; }

		public string PromotionName { get; set; }

		public string PromotionCode { get; set; }

		public string Enable { get; set; }

		public DateTime FromDate { get; set; }

		public DateTime ToDate { get; set; }

		public string Priority { get; set; }

		public string DiscountType { get; set; }

		[Column(TypeName = "Decimal(18,2)")]
		public decimal DiscountPkr { get; set; } = 0M;

		public string ApplicableOn { get; set; }

		public int CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string? ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}
