using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Security
{
	public class ChangePasswordRequest : RequestBase
	{
		[Required]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password can't be more than 20.")]
		public string CurrentPassword { get; set; }

		[Required]
		[MaxLength(20, ErrorMessage = "Maximum Length for Password can't be more than 20.")]
		public string NewPassword { get; set; }
	}
}