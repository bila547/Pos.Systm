using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel.Security
{
	public class ConfirmEmailViewModel
	{
		public string Code { get; set; }

		public string UserId { get; set; }

		[Display(Name = "Password Question")]
		public int? PasswordQuestionId { get; set; }

		public IEnumerable<SelectListItem> PasswordQuestions { get; set; }

		[MaxLength(300)]
		[Display(Name = "Password Answer")]
		[RegularExpression("^[\\sa-zA-Z0-9._'-]*$", ErrorMessage = "Allowed only alphanumeric, dot, space, apostrophe, underscore and dash.")]
		public string PasswordAnswer { get; set; }

		[MaxLength(128)]
		[Display(Name = "User Name")]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		[StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}