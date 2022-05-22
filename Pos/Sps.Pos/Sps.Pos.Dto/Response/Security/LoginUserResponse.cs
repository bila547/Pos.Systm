using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response.Security
{
	public class LoginUserResponse
	{
		public string Id { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }

		public string FullName => $"{FirstName} {MiddleName} {LastName}";

		public string Email { get; set; }

		public string UserName { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public List<RoleResponse> Roles { get; set; }

		public string Token { get; set; }

		public DateTime TokenExpiration { get; set; }

		public DateTime? LastLogin { get; set; }

		public int? NationalityId { get; set; }

		public string MobileNumber { get; set; }
	}
}
