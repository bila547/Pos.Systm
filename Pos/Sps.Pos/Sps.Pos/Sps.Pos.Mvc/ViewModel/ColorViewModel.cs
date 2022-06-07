using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class ColorListSearchViewModel
	{
		[Display(Name = "Color Name")]
		public string Name { get; set; }

		public List<ColorViewModel> Colors { get; set; }
	}
	public class ColorViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Color Code")]
		[Required(ErrorMessage = "Please enter Color Code.")]
		//[MaxLength(60)]
		public int ColorCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Color Name")]
		[Required(ErrorMessage = "Please enter Color Name.")]
		public string ColorName { get; set; }
	}
}
