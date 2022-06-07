using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class AreaListSearchViewModel
	{
		[Display(Name = "Area Name")]
		public string Name { get; set; }

		public List<AreaViewModel> Areas { get; set; }
	}
	public class AreaViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Area Code")]
		[Required(ErrorMessage = "Please enter AreaCode.")]
		//[MaxLength(60)]
		public int AreaCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Area Name")]
		[Required(ErrorMessage = "Please enter Area Name.")]
		public string AreaName { get; set; }
	}
}
