using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Request.Security
{
	public class ResetPasswordRequest : RequestBase
	{
		[Required]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password can't be more than 20.")]
		public string NewPassword { get; set; }
	}
}