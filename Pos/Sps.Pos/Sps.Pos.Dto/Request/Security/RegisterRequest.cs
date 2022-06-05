using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Request.Security
{
	public class RegisterRequest
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Email { get; set; }

		public string WebSite { get; set; }

		[Required]
		public string MobileNumber { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string StreetAddress { get; set; }
	}
}