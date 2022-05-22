using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Infrastructure.Enum
{
	public enum UserLoginStatus
	{
		[Description("Success")]
		Success = 0,

		[Description("Incorrect Password")]
		IncorrectPassword = 1,

		[Description("Invalid User Name")]
		InvalidUserName = 2,

		[Description("Inactive User")]
		InactiveUser = 3,

		[Description("Deleted User")]
		DeletedUser = 4,

		[Description("Locked Out User")]
		LockedOutUser = 5,

		[Description("Email Unconfirmed User")]
		EmailUnconfirmedUser = 6,

		[Description("Unapproved User")]
		UnapprovedUser = 7,

		[Description("Invalid Application Id User")]
		InvalidApplicationIdUser = 8,

		[Description("Dormant User")]
		DormantUser = 9,

		[Description("Bad Request")]
		BadRequest = 10,
	}
}
