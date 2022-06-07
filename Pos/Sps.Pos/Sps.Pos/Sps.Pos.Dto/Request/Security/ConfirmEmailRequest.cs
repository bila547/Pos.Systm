using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Security
{
	public class ConfirmEmailRequest:  RequestBase

	{
		[Required]
		public string Token { get; set; }

		//[Required]
		[MaxLength(128)]
		public string UserName { get; set; }

		[Required]
		//[Unlike(nameof(UserName), ErrorMessage = "User Name and Password cannot be the same")]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password can't be more than 20.")]
		public string Password { get; set; }

		public int? PasswordQuestionId { get; set; }

		[MaxLength(300)]
		public string PasswordAnswer { get; set; }

	}
}
