using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class ShiftListSearchViewModel
	{
		[Display(Name = "Shift Name")]
		public string Name { get; set; }

		public List<ShiftViewModel> Shifts { get; set; }
	}
	public class ShiftViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Shift Code")]
		[Required(ErrorMessage = "Please enter Shift Code.")]
		//[MaxLength(60)]
		public int ShiftCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Shift Name")]
		[Required(ErrorMessage = "Please enter Shift Name.")]
		public string ShiftName { get; set; }
	}
}
