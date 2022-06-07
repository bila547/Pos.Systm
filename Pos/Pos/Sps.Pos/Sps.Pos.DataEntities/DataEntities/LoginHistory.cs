using Sps.Pos.Infrastructure.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sps.Pos.DataEntities.DataEntities
{

	[Table(name: "AspNetLoginHistory")]
	public class LoginHistory
	{
		public LoginHistory()
		{
			DateTimeStamp = DateTime.UtcNow;
		}

		public int Id { get; set; }

		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }

		public UserLoginStatus UserLoginStatus { get; set; }

		public DateTime? LoginTime { get; set; }

		[MaxLength(50)]
		[Column(TypeName = "Varchar(50)")]
		public string ClientIpAddress { get; set; }

		[MaxLength(50)]
		[Column(TypeName = "Varchar(50)")]
		public string UserIpAddress { get; set; }

		public DateTime DateTimeStamp { get; set; }
	}
}
