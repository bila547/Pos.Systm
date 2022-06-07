using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Security
{
	public class AdminResetPasswordRequest : RequestBase
	{
		[Required]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password can't be more than 20.")]
		public string NewPassword { get; set; }
	}
}