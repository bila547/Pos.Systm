using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(name: "AspNetPasswordHistory")]
	public class PasswordHistory : EntityBase
	{
		public PasswordHistory()
		{
			CreatedDate = DateTime.UtcNow;
		}

		[Key]
		public string PasswordHash { get; set; }

		[ForeignKey("ApplicationUser")]
		public string UserId { get; set; }

		public virtual ApplicationUser ApplicationUser { get; set; }
	}
}
