using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class UnitListSearchViewModel
	{
		[Display(Name = "Unit Name")]
		public string Name { get; set; }

		public List<UnitViewModel> Units { get; set; }
	}
	public class UnitViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Unit Code")]
		[Required(ErrorMessage = "Please enter AreaCode.")]
		//[MaxLength(60)]
		public int UnitCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Unit Name")]
		[Required(ErrorMessage = "Please enter Unit Name.")]
		public string UnitName { get; set; }
	}
}
