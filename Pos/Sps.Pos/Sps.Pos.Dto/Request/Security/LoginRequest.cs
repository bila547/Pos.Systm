using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Request.Security
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "User Name is required")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		[MaxLength(50, ErrorMessage = "Maximum Length can't be more than 50.")]
		public virtual string UserIpAddress { get; set; }
	}
}