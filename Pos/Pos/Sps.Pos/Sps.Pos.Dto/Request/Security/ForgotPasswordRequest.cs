using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Request.Security
{
	public class ForgotPasswordRequest : RequestBase
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}