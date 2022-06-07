using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class SizeListSearchViewModel
	{
		[Display(Name = "Size Name")]
		public string Name { get; set; }

		public List<SizeViewModel> Sizes { get; set; }
	}
	public class SizeViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Size Code")]
		[Required(ErrorMessage = "Please enter Size Code.")]
		//[MaxLength(60)]
		public int SizeCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Size Name")]
		[Required(ErrorMessage = "Please enter Size Name.")]
		public string SizeName { get; set; }
	}
}
