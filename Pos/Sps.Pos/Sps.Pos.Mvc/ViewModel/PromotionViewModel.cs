using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sps.Pos.Mvc.ViewModel
{
	public class PromotionListSearchViewModel
	{
		[Display(Name = "Promotion Name")]
		public string Name { get; set; }

		public List<PromotionViewModel> PromotionsS { get; set; }
	}
	public class PromotionViewModel
	{
		[Key]
		public int Id { get; set; }
		//[MaxLength(256)]
		[Display(Name = "Promotion Name")]
		[Required(ErrorMessage = "Please enter Promotion Name.")]
		public string PromotionName { get; set; }

		[Display(Name = "Promotion Code")]
		[Required(ErrorMessage = "Please enter Promotion Code.")]
		//[MaxLength(60)]
		public string PromotionCode { get; set; }

		[Display(Name = "Enable")]
		[Required(ErrorMessage = "Please enter Enable or Not.")]
		public string Enable { get; set; }

		//[MaxLength(256)]
		[Display(Name = "Start Date")]
		[Required(ErrorMessage = "Please enter Start Date.")]
		public DateTime FromDate { get; set; }

		//[MaxLength(256)]
		[Display(Name = "End Date")]
		[Required(ErrorMessage = "Please enter End Date.")]
		public DateTime ToDate { get; set; }

		//[MaxLength(256)]
		[Display(Name = "Priority Name")]
		[Required(ErrorMessage = "Please enter Priority Name.")]
		public string Priority { get; set; }

		[Display(Name = "Discount Type ")]
		[Required(ErrorMessage = "Please enter Discount Type.")]
		public string DiscountType { get; set; }

		[Display(Name = "Discount Pkr")]
		[Required(ErrorMessage = "Please enter Discount Pkr.")]
		[Column(TypeName = "Decimal(18,2)")]
		public decimal DiscountPkr { get; set; } = 0M;


		[Display(Name = "Applicable On")]
		[Required(ErrorMessage = "Please enter Applicable On.")]
		public string ApplicableOn { get; set; }
	}
}
