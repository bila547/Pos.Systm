using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class SaleManListSearchViewModel
	{
		[Display(Name = "SaleMan Name")]
		public string Name { get; set; }

		public List<SaleManViewModel> SaleMans { get; set; }
	}
	public class SaleManViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "SaleMan Code")]
		[Required(ErrorMessage = "Please enter SaleMan Code.")]
		//[MaxLength(60)]
		public string SaleManCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "SaleMan Name")]
		[Required(ErrorMessage = "Please enter SaleMan Name.")]
		public string SaleManName { get; set; }
	}
}
