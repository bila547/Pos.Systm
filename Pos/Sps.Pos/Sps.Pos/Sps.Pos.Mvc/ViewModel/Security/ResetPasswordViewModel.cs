using System.ComponentModel.DataAnnotations;
namespace Sps.Pos.Mvc.ViewModel.Security
{
	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password is 20.")]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password is 20.")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}
}