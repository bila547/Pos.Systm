using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Customer))]
	public class Customer : EntityBase
	{
		[Key, ForeignKey(nameof(ApplicationUser))]
		public string ApplicationUserId { get; set; }

		public ApplicationUser ApplicationUser { get; set; }

		[MaxLength(100)]
		public string StreetAddress { get; set; }

		public string WebSite { get; set; }

	}
}
