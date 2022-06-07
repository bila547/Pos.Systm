using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class BrandListSearchViewModel
	{
		[Display(Name = "Brand Name")]
		public string Name { get; set; }

		public List<BrandViewModel> Brands { get; set; }
	}
	public class BrandViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Brand Code")]
		[Required(ErrorMessage = "Please enter Brand Code.")]
		//[MaxLength(60)]
		public int BrandCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Brand Name")]
		[Required(ErrorMessage = "Please enter Brnad Name.")]
		public string BrandName { get; set; }
	}
}
