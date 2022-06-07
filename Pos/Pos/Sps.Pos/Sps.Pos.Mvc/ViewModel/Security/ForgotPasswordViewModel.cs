using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel.Security
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Registered Email")]
		public string Email { get; set; }
	}
}