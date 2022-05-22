using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel.Security
{
	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Username/Email")]
		public string Email { get; set; }

		[Required]
		[MaxLength(20)]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		public string UserSecret { get; set; }
	}
}