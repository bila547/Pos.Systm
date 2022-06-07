using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response.Security
{
	public class UserResponse
	{
		public UserResponse()
		{
			UserRoles = new List<RoleResponse>();
		}

		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public string MobileNumber { get; set; }
		public bool IsApproved { get; set; }
		public DateTime? LastLogin { get; set; }
		public DateTime? DateOfBirth { get; set; }

		public List<RoleResponse> UserRoles { get; set; }
	}
}
