using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class TaxListSearchViewModel
	{
		[Display(Name = "Tax Name")]
		public string Name { get; set; }

		public List<TaxViewModel> Taxes { get; set; }
	}
	public class TaxViewModel 
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Tax Name")]
		[Required(ErrorMessage = "Please enter Tax Name.")]
		//[MaxLength(60)]
		public string TaxName { get; set; }

		////[MaxLength(26)]
		[Display(Name = "Tax Percentage")]
		[Required(ErrorMessage = "Please enter Tax Percentage.")]
		public decimal TaxPercentage { get; set; }
	}
}
