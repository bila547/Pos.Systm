using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sps.Pos.DataEntities.DataEntities
{
	public class ApplicationRole : IdentityRole
	{
		[ForeignKey(nameof(Application))]
		public Guid ApplicationId { get; set; }

		public Application Application { get; set; }

		public bool IsDeleted { get; set; }

		public int? CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public int? ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }

		[NotMapped]
		public bool Checked { get; set; }
	}
}